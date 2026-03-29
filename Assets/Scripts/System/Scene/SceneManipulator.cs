using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManipulator
{
    private readonly HashSet<string> _registry = new();
    private string _loadingSceneName;

    public event Action<string> OnSceneLoadStart;
    public event Action<string> OnSceneLoadComplete;
    public event Action<string> OnSceneUnloadStart;
    public event Action<string> OnSceneUnloadComplete;

    public string LoadingSceneName => _loadingSceneName;

    // -------------------------------------------------------
    // 로딩 씬 설정
    // -------------------------------------------------------

    /// <summary>
    /// 트랜지션용 로딩 씬을 지정합니다.
    /// 이 씬은 TransitionAsync를 통해서만 Load/Unload되며,
    /// LoadAsync/UnloadAsync로 직접 조작할 수 없습니다.
    /// </summary>
    public void SetLoadingScene(string sceneName)
    {
        _loadingSceneName = sceneName;
    }

    // -------------------------------------------------------
    // 씬 레지스트리 (이름 기반 관리)
    // -------------------------------------------------------

    /// <summary>씬 이름을 레지스트리에 등록합니다.</summary>
    public void Register(string sceneName) => _registry.Add(sceneName);

    /// <summary>씬 이름을 레지스트리에서 제거합니다.</summary>
    public void Unregister(string sceneName) => _registry.Remove(sceneName);

    /// <summary>씬 이름이 레지스트리에 등록되어 있는지 확인합니다.</summary>
    public bool IsRegistered(string sceneName) => _registry.Contains(sceneName);

    /// <summary>씬이 현재 로드된 상태인지 확인합니다.</summary>
    public bool IsLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;

    // -------------------------------------------------------
    // 직접 조작 (트랜지션 시퀀스와 무관하게 사용 가능)
    // -------------------------------------------------------

    /// <summary>
    /// 씬을 비동기로 로드합니다.
    /// 로딩 씬으로 지정된 씬은 직접 조작할 수 없습니다.
    /// </summary>
    public async Task LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        if (sceneName == _loadingSceneName)
        {
            Debug.LogWarning($"[SceneManipulator] '{sceneName}'은 로딩 씬입니다. TransitionAsync를 사용하세요.");
            return;
        }

        OnSceneLoadStart?.Invoke(sceneName);
        await WaitForOperation(SceneManager.LoadSceneAsync(sceneName, mode));
        OnSceneLoadComplete?.Invoke(sceneName);
    }

    /// <summary>
    /// 씬을 비동기로 언로드합니다.
    /// 로딩 씬으로 지정된 씬은 직접 조작할 수 없습니다.
    /// </summary>
    public async Task UnloadAsync(string sceneName)
    {
        if (sceneName == _loadingSceneName)
        {
            Debug.LogWarning($"[SceneManipulator] '{sceneName}'은 로딩 씬입니다. TransitionAsync를 사용하세요.");
            return;
        }

        if (!IsLoaded(sceneName))
            return;

        OnSceneUnloadStart?.Invoke(sceneName);
        await WaitForOperation(SceneManager.UnloadSceneAsync(sceneName));
        OnSceneUnloadComplete?.Invoke(sceneName);
    }

    // -------------------------------------------------------
    // 통합 로딩 트랜지션
    // -------------------------------------------------------

    /// <summary>
    /// 로딩 씬을 올린 뒤 현재 씬들을 모두 내리고,
    /// 파라미터로 전달된 씬들을 로드한 후 로딩 씬을 내립니다.
    /// </summary>
    public async Task TransitionAsync(params string[] sceneNames)
    {
        if (string.IsNullOrEmpty(_loadingSceneName))
        {
            Debug.LogError("[SceneManipulator] 로딩 씬이 설정되지 않았습니다. SetLoadingScene()을 먼저 호출하세요.");
            return;
        }

        if (sceneNames.Length == 0)
        {
            Debug.LogError("[SceneManipulator] 전환 대상 씬이 존재하지 않습니다.");
        }

        // 1. 로딩 씬 올리기
        await LoadLoadingSceneAsync();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_loadingSceneName));
        // 2. 로딩 씬 제외, 목적지에 없는 씬만 내리기
        var targetSet = new HashSet<string>(sceneNames);
        var toUnload = GetLoadedSceneNames()
            .Where(n => n != _loadingSceneName && !targetSet.Contains(n))
            .ToList();
        foreach (var name in toUnload)
        {
            OnSceneUnloadStart?.Invoke(name);
            await WaitForOperation(SceneManager.UnloadSceneAsync(name));
            OnSceneUnloadComplete?.Invoke(name);
        }

        bool activeSceneChanged = false;
        // 3. 아직 로드되지 않은 목적지 씬들만 로드
        foreach (var name in sceneNames)
        {
            if (!IsLoaded(name))
            {
                OnSceneLoadStart?.Invoke(name);
                await WaitForOperation(SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive));
                OnSceneLoadComplete?.Invoke(name);    
            }
            if(!activeSceneChanged)
            { 
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
                activeSceneChanged = true;
            }
            
        }

        // 4. 로딩 씬 내리기
        await UnloadLoadingSceneAsync();
    }

    // -------------------------------------------------------
    // 내부
    // -------------------------------------------------------

    private async Task LoadLoadingSceneAsync()
    {
        OnSceneLoadStart?.Invoke(_loadingSceneName);
        await WaitForOperation(SceneManager.LoadSceneAsync(_loadingSceneName, LoadSceneMode.Additive));
        OnSceneLoadComplete?.Invoke(_loadingSceneName);
    }

    private async Task UnloadLoadingSceneAsync()
    {
        if (!IsLoaded(_loadingSceneName))
            return;

        OnSceneUnloadStart?.Invoke(_loadingSceneName);
        await WaitForOperation(SceneManager.UnloadSceneAsync(_loadingSceneName));
        OnSceneUnloadComplete?.Invoke(_loadingSceneName);
    }

    private List<string> GetLoadedSceneNames()
    {
        var names = new List<string>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
            names.Add(SceneManager.GetSceneAt(i).name);
        return names;
    }

    private static Task WaitForOperation(AsyncOperation op)
    {
        var tcs = new TaskCompletionSource<bool>();
        op.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }
}
