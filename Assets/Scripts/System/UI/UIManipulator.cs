using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;

public class UIManipulator : MonoBehaviour
{
    private Dictionary<string, AssetReference> _uiRes = new();

    public UILayerData[] LayerData = System.Array.Empty<UILayerData>();

    public UIPage CurrentPage;
    public UIPopup[] CurrentPopups = System.Array.Empty<UIPopup>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SystemHolder.Get().UIManipulator = this;
    }

    public void RegisterUI(string uiId, AssetReference assetReference)
    {
        _uiRes[uiId] = assetReference;
    }

    public void UnregisterUI(string uiId)
    {
        _uiRes.Remove(uiId);
    }

    public async Task OpenPage(UIData data)
    {
        // 현재 열려있는 팝업, 페이지 객체 제거
        foreach (var popup in CurrentPopups)
        {
            if (popup != null)
            {
                popup.Close();
                Destroy(popup.gameObject);
            }
        }
        CurrentPopups = System.Array.Empty<UIPopup>();

        if (CurrentPage != null)
        {
            CurrentPage.Close();
            Destroy(CurrentPage.gameObject);
            CurrentPage = null;
        }

        // 새 UILayerData 생성 후 LayerData에 추가
        int layerIndex = LayerData.Length;
        var layerData = new UILayerData
        {
            LayerIndex = layerIndex,
            PageData = data,
            PopupData = System.Array.Empty<UIData>()
        };
        data.LayerIndex = layerIndex;
        LayerData = LayerData.Append(layerData).ToArray();

        // SpawnUI로 페이지 생성 후 CurrentPage 등록
        var go = await SpawnUI(layerData.PageData);
        if (go == null) return;

        CurrentPage = go.GetComponent<UIPage>();
        if (CurrentPage == null)
        {
            Debug.LogWarning($"[UIManipulator] '{data.UIID}' 프리팹에 UIPage 컴포넌트가 없습니다.");
            return;
        }
        CurrentPage.CurrentData = data;
        CurrentPage.Open();
    }

    public async Task OpenPopup(UIData data)
    {
        if (LayerData.Length == 0)
        {
            Debug.LogWarning("[UIManipulator] 열려있는 레이어가 없습니다. OpenPage()를 먼저 호출하세요.");
            return;
        }

        // 가장 마지막 UILayerData의 PopupData에 추가
        var layerData = LayerData[LayerData.Length - 1];
        int popupIndex = layerData.PopupData.Length;
        data.LayerIndex = layerData.LayerIndex;
        data.PopupIndex = popupIndex;
        layerData.PopupData = layerData.PopupData.Append(data).ToArray();

        // SpawnUI로 팝업 생성 후 CurrentPopups에 추가
        var go = await SpawnUI(data);
        if (go == null) return;

        var popup = go.GetComponent<UIPopup>();
        if (popup == null)
        {
            Debug.LogWarning($"[UIManipulator] '{data.UIID}' 프리팹에 UIPopup 컴포넌트가 없습니다.");
            return;
        }
        popup.CurrentData = data;
        popup.Open();

        CurrentPopups = CurrentPopups.Append(popup).ToArray();
    }

    public void ClosePopup(int layerIndex, int popupIndex)
    {
        bool isCurrentLayer = LayerData.Length > 0 && LayerData[LayerData.Length - 1].LayerIndex == layerIndex;
        if (isCurrentLayer)
        {
            if (popupIndex >= 0 && popupIndex < CurrentPopups.Length && CurrentPopups[popupIndex] != null)
            {
                CurrentPopups[popupIndex].Close();
                Destroy(CurrentPopups[popupIndex].gameObject);
                var list = CurrentPopups.ToList();
                list.RemoveAt(popupIndex);
                CurrentPopups = list.ToArray();
            }
        }

        var layer = LayerData.FirstOrDefault(l => l.LayerIndex == layerIndex);
        if (layer == null) return;

        if (popupIndex >= 0 && popupIndex < layer.PopupData.Length)
        {
            var list = layer.PopupData.ToList();
            list.RemoveAt(popupIndex);
            layer.PopupData = list.ToArray();
        }
    }

    public void CloseAllPopupAt(int layerIndex)
    {
        var layer = LayerData.FirstOrDefault(l => l.LayerIndex == layerIndex);
        if (layer == null) return;

        for (int i = layer.PopupData.Length - 1; i >= 0; i--)
            ClosePopup(layerIndex, i);
    }

    public void CloseAllPopup()
    {
        if (LayerData.Length == 0) return;

        CloseAllPopupAt(LayerData[LayerData.Length - 1].LayerIndex);
    }

    public async Task ClosePage()
    {
        if (LayerData.Length == 0) return;

        // 현재 열려있는 팝업, 페이지 객체 제거 (LayerData는 건드리지 않음)
        foreach (var popup in CurrentPopups)
        {
            if (popup != null)
            {
                popup.Close();
                Destroy(popup.gameObject);
            }
        }
        CurrentPopups = System.Array.Empty<UIPopup>();

        if (CurrentPage != null)
        {
            CurrentPage.Close();
            Destroy(CurrentPage.gameObject);
            CurrentPage = null;
        }

        // LayerData 마지막 항목 삭제
        var list = LayerData.ToList();
        list.RemoveAt(list.Count - 1);
        LayerData = list.ToArray();

        if (LayerData.Length == 0) return;

        // 이전 레이어의 PageData, PopupData로 현재 UI 재구축
        var prevLayer = LayerData[LayerData.Length - 1];

        var pageGo = await SpawnUI(prevLayer.PageData);
        if (pageGo != null)
        {
            CurrentPage = pageGo.GetComponent<UIPage>();
            if (CurrentPage == null)
                Debug.LogWarning($"[UIManipulator] '{prevLayer.PageData.UIID}' 프리팹에 UIPage 컴포넌트가 없습니다.");
            else
            {
                CurrentPage.CurrentData = prevLayer.PageData;
                CurrentPage.Open();
            }
        }

        var popups = new List<UIPopup>();
        foreach (var popupData in prevLayer.PopupData)
        {
            var popupGo = await SpawnUI(popupData);
            if (popupGo == null) continue;

            var popup = popupGo.GetComponent<UIPopup>();
            if (popup == null)
            {
                Debug.LogWarning($"[UIManipulator] '{popupData.UIID}' 프리팹에 UIPopup 컴포넌트가 없습니다.");
                continue;
            }
            popup.CurrentData = popupData;
            popup.Open();

            popups.Add(popup);
        }
        CurrentPopups = popups.ToArray();
    }

    public async Task<GameObject> SpawnUI(UIData data)
    {
        if (!_uiRes.TryGetValue(data.UIID, out var assetRef))
        {
            Debug.LogWarning($"[UIManipulator] UI ID '{data.UIID}'가 UIRes에 등록되어 있지 않습니다.");
            return null;
        }

        var handle = assetRef.InstantiateAsync(transform);
        await handle.Task;

        var go = handle.Result;
        go.transform.SetParent(transform, false);

        return go;
    }
}
