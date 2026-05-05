using System;
using System.Text;

public class Ieee754Converter
{

    private static int[] StrToArr(string bin)
    {
        int[] bits = new int[bin.Length];
        for (int i = 0; i < bin.Length; i++)
            bits[i] = bin[i] - '0';
        return bits;
    }

    private static string ArrToStr(int[] bits) =>
        string.Concat(bits);

    private static int[] ConvertIntToBin(int value, int length)
    {
        int[] bin = new int[length];
        for (int i = length - 1; i >= 0; i--)
            bin[length - 1 - i] = (value & (1 << i)) != 0 ? 1 : 0;
        return bin;
    }

    public static string FloatToBinStr(float value, bool beauty = false)
    {
        if (value == 0)
            return beauty ? "0 | 00000000 | 00000000000000000000000" : new string('0', 32);

        int sign = value < 0 ? 1 : 0;
        value = Math.Abs(value);

        int exp = 0;
        if (value >= 2.0f)
            while (value >= 2.0f) { value /= 2.0f; exp++; }
        else if (value < 1.0f)
            while (value < 1.0f && value > 0) { value *= 2.0f; exp--; }

        int biasedExp = exp + 127;
        int[] expBits = ConvertIntToBin(biasedExp, 8);

        value -= 1.0f;
        int[] mant = new int[23];
        for (int i = 0; i < 23; i++)
        {
            value *= 2.0f;
            if (value >= 1.0f) { mant[i] = 1; value -= 1.0f; }
            else { mant[i] = 0; }
        }

        int[] result = new int[32];
        result[0] = sign;
        Array.Copy(expBits, 0, result, 1, 8);
        Array.Copy(mant, 0, result, 9, 23);

        string res = ArrToStr(result);
        if (beauty)
            return $"{res[0]} | {res.Substring(1, 8)} | {res.Substring(9)}";
        return res;
    }

    public static float BinStrToFloat(string bin)
    {
        bin = bin.Replace(" ", "").Replace("|", "");
        if (bin.Length != 32) throw new ArgumentException("Must be 32 bits");
        if (bin == new string('0', 32)) return 0.0f;

        int[] bits = StrToArr(bin);

        int sign = bits[0] == 1 ? -1 : 1;

        int exp = 0;
        for (int i = 0; i < 8; i++)
            if (bits[i + 1] == 1) exp += (int)Math.Pow(2, 7 - i);
        exp -= 127;

        double mant = 1.0;
        for (int i = 0; i < 23; i++)
            if (bits[i + 9] == 1) mant += Math.Pow(2, -(i + 1));

        return (float)(sign * mant * Math.Pow(2, exp));
    }

    public static string DoubleToBinStr(double value, bool beauty = false)
    {
        if (value == 0)
            return beauty ? "0 | 00000000000 | " + new string('0', 52) : new string('0', 64);

        long sign = value < 0 ? 1 : 0;
        value = Math.Abs(value);

        int exp = 0;
        while (value >= 2.0) { value /= 2.0; exp++; }
        while (value < 1.0 && value > 0) { value *= 2.0; exp--; }

        long biasedExp = exp + 1023;
        int[] expBits = new int[11];
        for (int i = 10; i >= 0; i--)
            expBits[10 - i] = (biasedExp & (1L << i)) != 0 ? 1 : 0;

        value -= 1.0;
        int[] mant = new int[52];
        for (int i = 0; i < 52; i++)
        {
            value *= 2.0;
            if (value >= 1.0) { mant[i] = 1; value -= 1.0; }
            else { mant[i] = 0; }
        }

        int[] result = new int[64];
        result[0] = (int)sign;
        Array.Copy(expBits, 0, result, 1, 11);
        Array.Copy(mant, 0, result, 12, 52);

        string res = ArrToStr(result);
        if (beauty)
            return $"{res[0]} | {res.Substring(1, 11)} | {res.Substring(12)}";
        return res;
    }

    public static double BinStrToDouble(string bin)
    {
        bin = bin.Replace(" ", "").Replace("|", "");
        if (bin.Length != 64) throw new ArgumentException("Must be 64 bits");
        if (bin == new string('0', 64)) return 0.0;

        int[] bits = StrToArr(bin);

        int sign = bits[0] == 1 ? -1 : 1;

        int expVal = 0;
        for (int i = 1; i <= 11; i++)
            if (bits[i] == 1) expVal += (1 << (11 - i));
        int exp = expVal - 1023;

        double mant = 0.0;
        double fraction = 0.5;
        for (int i = 12; i < 64; i++)
        {
            if (bits[i] == 1) mant += fraction;
            fraction /= 2.0;
        }

        return sign * (1.0 + mant) * Math.Pow(2, exp);
    }


    public static void Main()
    {
        float num = 13.785f;
        string bin = FloatToBinStr(num, true);
        Console.WriteLine($"Float: {num} -> {bin}");
        Console.WriteLine($"Back to float: {BinStrToFloat(bin)}");

        double dNum = -0.775;
        string dbin = DoubleToBinStr(dNum, true);
        Console.WriteLine($"Double: {dNum} -> {dbin}");
        Console.WriteLine($"Back to double: {BinStrToDouble(dbin)}");
    }
}
