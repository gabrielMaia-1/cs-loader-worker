using System.Text;
namespace Tests.Data;

public class CsvGenerator
{
    public char Delimiter { get; }
    public char Quote { get; }
    private StringBuilder builder { get; }

    public CsvGenerator(char delimiter = ';', char quote = '"')
    {
        Delimiter = delimiter;
        Quote = quote;
        builder = new StringBuilder();
    }
    
    public string GenerateCsv(string[] columnNames, object[][] values)
    {
        builder.AppendLine(string.Join(Delimiter, columnNames));

        foreach (var value in values)
        {
            builder.AppendLine(string.Join(Delimiter, value));
        }

        return builder.ToString();
    }

    public static class DataSets
    {
        public static class Default
        {
            public static string[] Header = {"column_1", "column_2", "column_3"};
            public static object[][] Data = 
            {
                new object[]{"c0r0", "c1r0", "c2r0"},
                new object[]{"c0r1", "c1r1", "c2r1"},
                new object[]{"c0r2", "c1r2", "c2r2"},
                new object[]{"c0r3", "c1r3", "c2r3"},
            };
        }
    }


}