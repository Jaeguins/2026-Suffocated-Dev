using System.Collections.Generic;
using AutoGroupGenerator;
using UnityEngine;

public class ModelManipulatorBase : MonoBehaviour
{
    private HashSet<ModelData> _updateData = new();

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SystemHolder.Get().ModelManipulator = this;
    }

    public void Reserve(ModelData data)
    {
        _updateData.Add(data);
    }

    protected virtual void Update()
    {
        if (_updateData.Count > 0)
        {
            Debug.Log($"{_updateData.Count} new model changes");
        }
        foreach (var data in _updateData)
            data.Run();

        foreach (var data in _updateData)
            data.Flush();

        _updateData.Clear();
    }


    public virtual ModelData GetRoot()
    {
        return null;
    } 
}

public class ModelManipulator<T> : ModelManipulatorBase where T : ModelData
{
    public T RootData;

    public override ModelData GetRoot()
    {
        return RootData;
    } 
}

public enum ModelChangeType
{
    None = 0,
    Pure = 1,
    Created = 2,
    Modified = 3,
    Removed = 4,
    LengthChanged = 5,
}
