public partial class TableContainer
{
    public System.Collections.Generic.Dictionary<int, TestData> Table_TestData;

    public void LoadTestData(TestData[] items)
    {
        Table_TestData = new System.Collections.Generic.Dictionary<int, TestData>();
        foreach (var item in items)
        {
            Table_TestData[item.ID] = item;
        }
    }

    public void Load(string folderPath)
    {
        LoadTestData(TestData.Read(System.IO.File.ReadAllText(System.IO.Path.Combine(folderPath, "TestData.tsv"))));
    }
}
