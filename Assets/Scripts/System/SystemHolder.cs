using UnityEngine;

public class SystemHolder
{
    private static SystemHolder _instance = null;

    public SystemHolder()
    {
        SaveManager = new GameSaveManager();
    }

    public static SystemHolder Get() => _instance;

    public GameSaveManager SaveManager;
    public SceneManipulator SceneManipulator;
    public UIManipulator UIManipulator;
    public ModelManipulatorBase ModelManipulator;
    public TableManipulator TableManipulator;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitHolder()
    {
        if (_instance == null)
        {
            _instance = new SystemHolder();
            Debug.Log("SystemHolder Init");
        }
        
    }
}
