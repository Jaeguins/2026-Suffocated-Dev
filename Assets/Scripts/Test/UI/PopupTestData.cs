using System;

public class PopupTestData : UIData
{
    public event Action OnYes;
    public event Action OnNo;
    
    public void InvokeYes()
    {
        OnYes?.Invoke();
    }

    public void InvokeNo()
    {
        OnNo?.Invoke();
    }
}
