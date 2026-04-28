using System;
using System.Text;

public class Ieee754Converter
{

    public static string FloatToBinStr(float value, bool beauty = false)
    {
        if (value == 0) return beauty ? "0 | 00000000 | 00000000000000000000000" : "00000000000000000000000000000000";

        int sign = value < 0 ? 1 : 0;
        value = Math.Abs(value);

        int exp = 0;
        if (value >= 2.0f)
        {
            while (value >= 2.0f) { value /= 2.0f; exp++; }
        }
        else if (value < 1.0f)
        {
            while (value < 1.0f && value > 0) { value *= 2.0f; exp--; }
        }

        int biasedExp = exp + 127;
        string expBin = ConvertIntToBin(biasedExp, 8);

        value -= 1.0f;
        string mantBin = "";
        for (int i = 0; i < 23; i++)
        {
            value *= 2.0f;
            if (value >= 1.0f)
            {
                mantBin += "1";
                value -= 1.0f;
            }
            else
            {
                mantBin += "0";
            }
        }

        string result = sign + expBin + mantBin;

        if (beauty)
            return $"{result[0]} | {result.Substring(1, 8)} | {result.Substring(9)}";

        return result;
    }

    public static float BinStrToFloat(string bin)
    {
        bin = bin.Replace(" ", "").Replace("|", "");
        if (bin.Length != 32) throw new ArgumentException("Must be 32 bits");
        if (bin == new string('0', 32))
            return 0.0f;
        int sign = bin[0] == '1' ? -1 : 1;
        int exp = 0;
        for (int i = 0; i < 8; i++)
        {
            if (bin[i + 1] == '1') exp += (int)Math.Pow(2, 7 - i);
        }
        exp -= 127;

        double mant = 1.0;
        for (int i = 0; i < 23; i++)
        {
            if (bin[i + 9] == '1') mant += Math.Pow(2, -(i + 1));
        }

        return (float)(sign * mant * Math.Pow(2, exp));
    }

    private static string ConvertIntToBin(int value, int length)
    {
        string bin = "";
        for (int i = length - 1; i >= 0; i--)
        {
            bin += (value & (1 << i)) != 0 ? "1" : "0";
        }
        return bin;
    }

    public static string DoubleToBinStr(double value, bool beauty = false)
    {
        if (value == 0) return beauty ? "0 | 00000000000 | 0000000000000000000000000000000000000000000000000000" : new string('0', 64);

        long sign = value < 0 ? 1 : 0;
        value = Math.Abs(value);

        int exp = 0;
        while (value >= 2.0) { value /= 2.0; exp++; }
        while (value < 1.0 && value > 0) { value *= 2.0; exp--; }

        long biasedExp = exp + 1023;
        string expBin = "";
        for (int i = 10; i >= 0; i--)
            expBin += (biasedExp & (1L << i)) != 0 ? "1" : "0";

        value -= 1.0;
        string mantBin = "";
        for (int i = 0; i < 52; i++)
        {
            value *= 2.0;
            if (value >= 1.0) { mantBin += "1"; value -= 1.0; }
            else { mantBin += "0"; }
        }

        string result = sign + expBin + mantBin;

        if (beauty)
            return $"{result[0]} | {result.Substring(1, 11)} | {result.Substring(12)}";

        return result;
    }

    public static double BinStrToDouble(string bin)
    {
        bin = bin.Replace(" ", "").Replace("|", "");

        if (bin.Length != 64)
            throw new ArgumentException("Must be 64 bits");

        if (bin == new string('0', 64))
            return 0.0;

        int sign = bin[0] == '1' ? -1 : 1;

        int expBin = 0;
        for (int i = 1; i <= 11; i++)
        {
            if (bin[i] == '1')
                expBin += (1 << (11 - i));
        }

        int exp = expBin - 1023;

        double mant = 0.0;
        double fraction = 0.5;

        for (int i = 12; i < 64; i++)
        {
            if (bin[i] == '1')
                mant += fraction;

            fraction /= 2.0;
        }

        double result = sign * (1.0 + mant) * Math.Pow(2, exp);

        return result;
    }

    public static void Main()
    {
        float num = 13.785f;
        string bin = FloatToBinStr(num, true);
        Console.WriteLine($"Float: {num} -> {bin}");
        Console.WriteLine($"Back to float: {BinStrToFloat(bin)}");

        double dNum = -0.775;
        Console.WriteLine($"Double: {dNum} -> {DoubleToBinStr(dNum, true)}");
        bin = DoubleToBinStr(dNum, true);
        Console.WriteLine($"Back to double: {BinStrToDouble(bin)}");
    }
}
