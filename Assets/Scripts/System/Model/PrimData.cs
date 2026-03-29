public class PrimData<T> : ModelData
{
    private T _value;

    public T Get()
    {
        return _value;
    }

    public void Set(T value)
    {
        _value = value;
        Modify();
    }
}
