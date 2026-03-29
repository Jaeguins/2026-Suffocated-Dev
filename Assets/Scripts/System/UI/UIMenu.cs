using UnityEngine;

public abstract class UIMenu : MonoBehaviour
{
    public UIData CurrentData;

    public virtual void Open() { }
    public virtual void Close() { }
}
