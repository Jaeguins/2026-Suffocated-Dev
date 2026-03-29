using System;

public class TestRootModel : ModelData
{
    public TestObjectData ObjectData;
    public DictData<string,PrimData<int>> DictData;
    
    public TestRootModel()
    {
        ObjectData = new TestObjectData();
        ObjectData.Root=new WeakReference<ModelData>(this);
        DictData = new DictData<string, PrimData<int>>();
        DictData.Root=new WeakReference<ModelData>(this);
    }
}
