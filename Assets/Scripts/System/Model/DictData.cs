using System;
using System.Collections.Generic;

public class DictData<T, K> : ModelData where K : ModelData
{
    private Dictionary<T, K> _value = new();

    public void Add(T key, K item)
    {
        item.Root = new WeakReference<ModelData>(this);

        _value[key] = item;

        Modify(ModelChangeType.LengthChanged);
    }

    public void Remove(T key)
    {
        _value.Remove(key);

        Modify(ModelChangeType.LengthChanged);
    }

    public int Length()
    {
        return _value.Count;
    }

    public K Get(T key)
    {
        return _value[key];
    }

    public void Empty()
    {
        _value.Clear();
        Modify(ModelChangeType.LengthChanged);
    }
}
