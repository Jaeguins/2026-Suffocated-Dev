public class TableManipulator
{
    public TableContainer Container;

    public void Initialize(string tablePath)
    {
        Container = new TableContainer();
        Container.Load(tablePath);
    }
}