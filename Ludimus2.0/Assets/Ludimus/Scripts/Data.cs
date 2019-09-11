public class Data
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Region { get; set; }

    public static Data ConvertToData(string data)
    {
        string[] split = data.Split('|');
        return new Data
        {
            Key = split[0],
            Value = split[1],
            Region = split[2]
        };
    }

    public static string ConvertFromData(Data data)
    {
        string str = data.Key + "|" + data.Value + "|" + data.Region + "|";

        for (int i = str.Length; i < 255; i++)
        {
            str += " ";
        }
        return str;
    }

    public override string ToString()
    {
        return Region + ":  " + Key + " | " + Value;
    }
}