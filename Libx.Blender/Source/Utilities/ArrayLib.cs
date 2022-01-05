namespace Libx.Blender.Utilities;

using System;

#nullable enable


public static class ArrayLib
{

    public static int BinarySearch<T> (Array array, int index, int count, Func<T, int> predicate)
    {
        int lo = index;
        int hi = index + count - 1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo >> 1);
            int num = predicate ((T) array.GetValue (mid)!);

            if (num == 0)
                    return mid;

            if (num < 0)
                    lo = mid + 1;
            else
                    hi = mid - 1;
        }
        return ~lo;
    }

    public static void Sort<T> (T[ ] array, int index, int count, Comparison<T> comparer)
    {
        var span = new Span <T> (array, index, count);
        MemoryExtensions.Sort (span, comparer);
    }
}
