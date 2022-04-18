namespace Application.Commons.Adapters
{
    public struct CsvConfiguration
    {
        public char Delimiter;
        public char Quote;
        public bool HasHeader;
        public CsvConfiguration()
        {
            Delimiter = ';';
            Quote = '"';
            HasHeader = true;
        }
    }
}