using System;

public class TestObjectData : ModelData
{
    public PrimData<int> TestingData;
    public ArrData<PrimData<int>> ArrData;

    public TestObjectData()
    {
        TestingData = new PrimData<int>();
        TestingData.Root=new WeakReference<ModelData>(this);
        ArrData = new ArrData<PrimData<int>>();
        ArrData.Root=new WeakReference<ModelData>(this);
    }
}
