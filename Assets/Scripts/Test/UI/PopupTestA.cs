public class PopupTestA : UIPopup
{

    public void OnYes()
    {
        PopupTestData castedData=CurrentData as PopupTestData;
        castedData.InvokeYes();
        SystemHolder.Get().UIManipulator.ClosePopup(castedData.LayerIndex,castedData.PopupIndex);
    }

    public void OnNo()
    {
        PopupTestData castedData=CurrentData as PopupTestData;
        castedData.InvokeNo();
        SystemHolder.Get().UIManipulator.ClosePopup(castedData.LayerIndex,castedData.PopupIndex);
    }
}
