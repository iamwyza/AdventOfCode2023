namespace AdventOfCode2023;

public static class MathUtils
{
    public static ulong GreatestCommonFactor(ulong a, ulong b)
    {
        while (b != 0)
        {
            ulong temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static ulong LeastCommonMultiple(ulong a, ulong b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }

    public static ulong GreatestCommonDivisor(ulong a, ulong b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }

    public static long Factorial(long f)
    {
        if (f == 0)
            return 1L;
        else
            return f * Factorial(f - 1);
    }

}