public class TestData
{
    public int ID;
    public string Name;
    public float Power;
    public float[] Scores;
    public int[] Attributes;
    public string[] Prefix;
    public long LongValue;
    public long[] LongListValues;

    public static TestData[] Read(string dsv, char delimiter = '\t')
    {
        var lines = dsv.Split('\n');
        var results = new System.Collections.Generic.List<TestData>();
        for (int i = 2; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            var cols = line.Split(delimiter);
            var obj = new TestData();
            obj.ID = int.Parse(cols[0]);
            obj.Name = cols[1];
            obj.Power = float.Parse(cols[2], System.Globalization.CultureInfo.InvariantCulture);
            obj.Scores = System.Array.ConvertAll(cols[3].Split(','), s => float.Parse(s, System.Globalization.CultureInfo.InvariantCulture));
            obj.Attributes = System.Array.ConvertAll(cols[4].Split(','), int.Parse);
            obj.Prefix = cols[5].Split(',');
            obj.LongValue = long.Parse(cols[6]);
            obj.LongListValues = System.Array.ConvertAll(cols[7].Split(','), long.Parse);
            results.Add(obj);
        }
        return results.ToArray();
    }
}
