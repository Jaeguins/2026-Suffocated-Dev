using System;

public class ArrData<T> : ModelData where T : ModelData
{
    private T[] _value = Array.Empty<T>();

    public void Add(T item)
    {
        item.Root = new WeakReference<ModelData>(this);

        var newValue = new T[_value.Length + 1];
        Array.Copy(_value, newValue, _value.Length);
        newValue[_value.Length] = item;
        _value = newValue;

        Modify(ModelChangeType.LengthChanged);
    }

    public void RemoveAt(int index)
    {
        var newValue = new T[_value.Length - 1];
        Array.Copy(_value, 0, newValue, 0, index);
        Array.Copy(_value, index + 1, newValue, index, _value.Length - index - 1);
        _value = newValue;

        Modify(ModelChangeType.LengthChanged);
    }

    public int Length()
    {
        return _value.Length;
    }

    public T Get(int index)
    {
        return _value[index];
    }

    public void Empty()
    {
        _value = Array.Empty<T>();
        Modify(ModelChangeType.LengthChanged);
    }
}
