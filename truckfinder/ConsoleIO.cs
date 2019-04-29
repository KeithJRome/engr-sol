using System;

namespace TruckFinder
{
    public class ConsoleIO : ITextConsole
    {
        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public string ReadString(string prompt, string defValue = null)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                return defValue ?? string.Empty;
            }
            return input;
        }

        public decimal ReadDecimal(string prompt, decimal minValue, decimal maxValue, decimal? defValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (string.IsNullOrEmpty(input) && defValue.HasValue)
                {
                    return defValue.Value;
                }
                if (decimal.TryParse(input, out decimal output))
                {
                    if (output >= minValue && output <= maxValue)
                    {
                        return output;
                    }
                }
                Console.WriteLine($"'{input}' is not a valid value. Please try again.");
            }
        }

        public decimal ReadInteger(string prompt, int minValue, int maxValue, int? defValue)
        {
            while (true)
            {
                var input = ReadString(prompt);
                if (string.IsNullOrEmpty(input) && defValue.HasValue)
                {
                    return defValue.Value;
                }
                if (int.TryParse(input, out int output))
                {
                    if (output >= minValue && output <= maxValue)
                    {
                        return output;
                    }
                }
                Console.WriteLine($"'{input}' is not a valid value. Please try again.");
            }
        }
    }
}
