namespace AndrejKrizan.DotNet.Extensions;

public static class IntegerExtensions
{
    #region GetDigitsCount
    public static byte GetDigitsCount(this sbyte n)
    {
        n = Math.Abs(n);
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        return 3;
    }

    public static byte GetDigitsCount(this byte n)
    {
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        return 3;
    }


    public static byte GetDigitsCount(this short n)
    {
        n = Math.Abs(n);
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        return 5;
    }

    public static byte GetDigitsCount(this ushort n)
    {
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        return 5;
    }


    public static byte GetDigitsCount(this int n)
    {
        n = Math.Abs(n);
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        if (n <= 99999) return 5;
        if (n <= 999999) return 6;
        if (n <= 9999999) return 7;
        if (n <= 99999999) return 8;
        if (n <= 999999999) return 9;
        return 10;
    }

    public static byte GetDigitsCount(this uint n)
    {
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        if (n <= 99999) return 5;
        if (n <= 999999) return 6;
        if (n <= 9999999) return 7;
        if (n <= 99999999) return 8;
        if (n <= 999999999) return 9;
        return 10;
    }


    public static byte GetDigitsCount(this long n)
    {
        n = Math.Abs(n);
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        if (n <= 99999) return 5;
        if (n <= 999999) return 6;
        if (n <= 9999999) return 7;
        if (n <= 99999999) return 8;
        if (n <= 999999999) return 9;
        if (n <= 9999999999) return 10;
        if (n <= 99999999999) return 11;
        if (n <= 999999999999) return 12;
        if (n <= 9999999999999) return 13;
        if (n <= 99999999999999) return 14;
        if (n <= 999999999999999) return 15;
        if (n <= 9999999999999999) return 16;
        if (n <= 99999999999999999) return 17;
        if (n <= 999999999999999999) return 18;
        return 19;
    }

    public static byte GetDigitsCount(this ulong n)
    {
        if (n <= 9) return 1;
        if (n <= 99) return 2;
        if (n <= 999) return 3;
        if (n <= 9999) return 4;
        if (n <= 99999) return 5;
        if (n <= 999999) return 6;
        if (n <= 9999999) return 7;
        if (n <= 99999999) return 8;
        if (n <= 999999999) return 9;
        if (n <= 9999999999) return 10;
        if (n <= 99999999999) return 11;
        if (n <= 999999999999) return 12;
        if (n <= 9999999999999) return 13;
        if (n <= 99999999999999) return 14;
        if (n <= 999999999999999) return 15;
        if (n <= 9999999999999999) return 16;
        if (n <= 99999999999999999) return 17;
        if (n <= 999999999999999999) return 18;
        if (n <= 9999999999999999999) return 19;
        return 20;
    }
    #endregion
}
