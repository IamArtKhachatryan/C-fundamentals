using System;
using System.Text;

namespace BigNumbersApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Overflow ---");
            int a = int.MaxValue;
            Console.WriteLine(unchecked(a + 1));
            long b = long.MaxValue;
            Console.WriteLine(unchecked(b + 1));
            Console.WriteLine();

            Console.WriteLine("--- Big Numbers Arithmetic ---");
            Console.WriteLine($"Add: {Add("9999", "1")}");
            Console.WriteLine($"Subtract: {Subtract("10000", "1")}");
            Console.WriteLine($"Multiply: {Multiply("123", "456")}");

            Console.WriteLine("\n--- Negative numbers ---");
            Console.WriteLine($"Add (-5 + -10): {Add("-5", "-10")}");
            Console.WriteLine($"Subtract (5 - 10): {Subtract("5", "10")}");
            Console.WriteLine($"Multiply (-12 * 10): {Multiply("-12", "10")}");
        }

        static string Validate(string num)
        {
            if (string.IsNullOrWhiteSpace(num)) return "0";

            int start = 0;
            if (num[0] == '-') start = 1;

            for (int i = start; i < num.Length; i++)
            {
                if (!char.IsDigit(num[i])) return "0";
            }
            return num;
        }

        static void PadStrings(ref string a, ref string b)
        {
            int maxLen = Math.Max(a.Length, b.Length);
            a = a.PadLeft(maxLen, '0');
            b = b.PadLeft(maxLen, '0');
        }

        static string Add(string a, string b)
        {
            a = Validate(a);
            b = Validate(b);

            if (a[0] == '-' && b[0] != '-') return Subtract(b, a.Substring(1));
            if (a[0] != '-' && b[0] == '-') return Subtract(a, b.Substring(1));

            bool isNegative = false;
            if (a[0] == '-' && b[0] == '-')
            {
                isNegative = true;
                a = a.Substring(1);
                b = b.Substring(1);
            }

            PadStrings(ref a, ref b);

            StringBuilder result = new StringBuilder();
            int carry = 0;

            for (int i = a.Length - 1; i >= 0; i--)
            {
                int digitA = a[i] - '0';
                int digitB = b[i] - '0';
                int sum = digitA + digitB + carry;
                result.Insert(0, sum % 10);
                carry = sum / 10;
            }

            if (carry > 0) result.Insert(0, carry);

            string final = result.ToString().TrimStart('0');
            if (final == "") return "0";
            return isNegative ? "-" + final : final;
        }
        static string Subtract(string a, string b)
        {
            a = Validate(a);
            b = Validate(b);

            if (a[0] == '-' && b[0] == '-') return Subtract(b.Substring(1), a.Substring(1));
            if (a[0] == '-' && b[0] != '-') return "-" + Add(a.Substring(1), b);
            if (a[0] != '-' && b[0] == '-') return Add(a, b.Substring(1));

            if (string.Compare(a.PadLeft(Math.Max(a.Length, b.Length), '0'),
                               b.PadLeft(Math.Max(a.Length, b.Length), '0')) < 0)
            {
                return "-" + Subtract(b, a);
            }

            PadStrings(ref a, ref b);

            StringBuilder result = new StringBuilder();
            int borrow = 0;

            for (int i = a.Length - 1; i >= 0; i--)
            {
                int digitA = a[i] - '0';
                int digitB = b[i] - '0' + borrow;

                if (digitA < digitB)
                {
                    digitA += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.Insert(0, digitA - digitB);
            }

            string final = result.ToString().TrimStart('0');
            if (final == "") return "0";
            return final;
        }
        static string Multiply(string a, string b)
        {
            a = Validate(a);
            b = Validate(b);

            bool isNegative = false;
            if (a[0] == '-') { isNegative = !isNegative; a = a.Substring(1); }
            if (b[0] == '-') { isNegative = !isNegative; b = b.Substring(1); }

            if (a == "0" || b == "0") return "0";

            int[] result = new int[a.Length + b.Length];

            for (int i = a.Length - 1; i >= 0; i--)
            {
                for (int j = b.Length - 1; j >= 0; j--)
                {
                    int mul = (a[i] - '0') * (b[j] - '0');
                    int p1 = i + j, p2 = i + j + 1;
                    int sum = mul + result[p2];

                    result[p2] = sum % 10;
                    result[p1] += sum / 10;
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (int digit in result)
            {
                if (!(sb.Length == 0 && digit == 0))
                    sb.Append(digit);
            }

            string final = sb.Length == 0 ? "0" : sb.ToString();
            return (isNegative && final != "0") ? "-" + final : final;
        }
    }
}
