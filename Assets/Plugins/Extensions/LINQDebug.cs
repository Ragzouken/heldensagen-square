using System;
using System.Linq;
using System.Collections.Generic;

public static partial class LINQDebug
{
    /// Output a line of debug for each item in a LINQ
    public static IEnumerable<T> Debug<T>(this IEnumerable<T> sequence,
                                          Func<T, string> output,
                                          string header="LINQ Debug")
    {
        var builder = new System.Text.StringBuilder(string.Format("{0}:\n", header));

        foreach (T item in sequence)
        {
            builder.AppendLine(output(item));

            yield return item;
        }

        UnityEngine.Debug.Log(builder.ToString());
    }

    // Force evaluation of a LINQ (so that other debugs actually happen)
    public static IEnumerable<T> Debug<T>(this IEnumerable<T> sequence)
    {
        var items = sequence.ToArray();

        return items;
    }
}
