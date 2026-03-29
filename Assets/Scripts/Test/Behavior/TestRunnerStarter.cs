using UnityEngine;

public class TestRunnerStarter : MonoBehaviour
{
    public void Start()
    {
        SystemHolder.Get().SceneManipulator?.TransitionAsync("Scenes/Scene_Test");
    }
}
