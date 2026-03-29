using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManipulatorStarter : MonoBehaviour
{
    [Serializable]
    public struct UIResEntry
    {
        public string UIID;
        public AssetReference Asset;
    }

    [Header("UIManipulator 프리팹 (비어있으면 빈 오브젝트로 생성)")]
    [SerializeField] private GameObject _uiManipulatorPrefab;

    [Header("등록할 UI 목록")]
    [SerializeField] private UIResEntry[] _uiResEntries;

    private void Awake()
    {
        var holder = SystemHolder.Get();

        if (holder.UIManipulator == null)
        {
            GameObject go = _uiManipulatorPrefab != null
                ? Instantiate(_uiManipulatorPrefab)
                : new GameObject("UIManipulator");

            if (go.GetComponent<UIManipulator>() == null)
                go.AddComponent<UIManipulator>();
            // UIManipulator.Awake()에서 holder.UIManipulator = this 로 자동 등록됨
        }

        foreach (var entry in _uiResEntries)
        {
            if (!string.IsNullOrEmpty(entry.UIID) && entry.Asset != null)
                holder.UIManipulator.RegisterUI(entry.UIID, entry.Asset);
        }
    }
}
