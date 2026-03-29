using UnityEngine;

public class TestRunner : MonoBehaviour
{
    public void Start()
    {
        PageTestData toOpen = new PageTestData();
        toOpen.UIID = "TestPageA";
        
        toOpen.IntentTestValue = Random.Range(0,100);
        
        Debug.Log($"Inserted Intent Value : {toOpen.IntentTestValue}");
        SystemHolder.Get().UIManipulator.OpenPage(toOpen);
    }
}
