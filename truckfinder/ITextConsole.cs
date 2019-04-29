namespace TruckFinder
{
    public interface ITextConsole
    {
        void WriteLine();

        void WriteLine(string value);

        string ReadString(string prompt, string defValue = null);

        decimal ReadDecimal(string prompt, decimal minValue, decimal maxValue, decimal? defValue);

        decimal ReadInteger(string prompt, int minValue, int maxValue, int? defValue);
    }
}
