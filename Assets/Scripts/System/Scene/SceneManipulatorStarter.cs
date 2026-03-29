using UnityEngine;

public class SceneManipulatorStarter : MonoBehaviour
{
    [Header("로딩 트랜지션 씬")]
    [SerializeField] private string _loadingSceneName;

    [Header("등록할 씬 목록")]
    [SerializeField] private string[] _sceneNames;

    private void Awake()
    {
        var holder = SystemHolder.Get();

        if (holder.SceneManipulator == null)
            holder.SceneManipulator = new SceneManipulator();

        var manipulator = holder.SceneManipulator;

        if (!string.IsNullOrEmpty(_loadingSceneName))
            manipulator.SetLoadingScene(_loadingSceneName);

        foreach (var sceneName in _sceneNames)
        {
            if (!string.IsNullOrEmpty(sceneName))
                manipulator.Register(sceneName);
        }
    }
}
