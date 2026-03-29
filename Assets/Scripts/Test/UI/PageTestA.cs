using System;
using TMPro;
using UnityEngine;

public class PageTestA : UIPage
{
    public TextMeshProUGUI IntentView;
    public TextMeshProUGUI ValueView;
    public TextMeshProUGUI TableView;

    public virtual string GetNextPage()
    {
        return "TestPageB";
    }
    public override void Open()
    {
        PageTestData castedData = CurrentData as PageTestData;
        IntentView.SetText($"Intent : {castedData.IntentTestValue}");
        TestRootModel modelData = (TestRootModel)SystemHolder.Get().ModelManipulator?.GetRoot();
        if (modelData == null)
        {
            return;
        }
        ChangeValueView(modelData.ObjectData.TestingData);
        modelData.ObjectData.TestingData.OnChanged += ChangeValueView;

        if (TableView != null)
        {
            string buffer = "TableArrTest : ";
            TableContainer container = SystemHolder.Get()?.TableManipulator?.Container;
            if (container != null)
            {
                TestData testData = container.Table_TestData[1234];
                if (testData != null)
                    foreach (long val in testData.LongListValues)
                        buffer += $" {val}";
                TableView.SetText(buffer);
            }
        }
    }

    public override void Close()
    {
        TestRootModel modelData = (TestRootModel)SystemHolder.Get().ModelManipulator?.GetRoot();
        if (modelData == null)
        {
            return;
        }

        modelData.ObjectData.TestingData.OnChanged -= ChangeValueView;
    }

    public void OnRerollBtn()
    {
        TestRootModel modelData = (TestRootModel)SystemHolder.Get().ModelManipulator?.GetRoot();
        if (modelData == null)
        {
            return;
        }
        
        modelData.ObjectData.TestingData.Set(UnityEngine.Random.Range(0,100));
    }

    public void OnSwitchBtn()
    {
        PopupTestData toOpen = new PopupTestData();
        toOpen.UIID = "TestPopupA";
        toOpen.OnYes += OpenTestB;

        SystemHolder.Get().UIManipulator?.OpenPopup(toOpen);
    }

    public void ChangeValueView(ModelData newValue)
    {
        PrimData<int> casted = newValue as PrimData<int>;
        ValueView.SetText($"Value : {casted.Get()}");
    }

    public void OpenTestB()
    {
        PageTestData toOpen = new PageTestData();
        
        toOpen.IntentTestValue = UnityEngine.Random.Range(0, 100);
        Debug.Log($"Inserted Intent Value : {toOpen.IntentTestValue}");
        toOpen.UIID = GetNextPage();

        SystemHolder.Get().UIManipulator.OpenPage(toOpen);
    }
}