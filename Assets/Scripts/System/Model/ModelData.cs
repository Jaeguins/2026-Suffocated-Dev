using System;

public class ModelData
{
    public ModelChangeType ChangeFlag = ModelChangeType.Created;

    public event Action<ModelData> OnChanged;

    public WeakReference<ModelData> Root;

    public void Modify(ModelChangeType changeType = ModelChangeType.Modified)
    {
        ChangeFlag = changeType;

        if (Root != null && Root.TryGetTarget(out var root))
        {
            root.Modify(changeType);
        }

        SystemHolder.Get()?.ModelManipulator?.Reserve(this);
    }

    public void Run()
    {
        OnChanged?.Invoke(this);
    }

    public void Flush()
    {
        ChangeFlag = ModelChangeType.Pure;
    }
}
