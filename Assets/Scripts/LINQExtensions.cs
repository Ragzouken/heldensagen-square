using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static partial class LINQExtensions
{
    public static T RandomElement<T>(this IEnumerable<T> sequence)
    {
        int count = sequence.Count();
        int index = UnityEngine.Random.Range(0, count);

        return sequence.ElementAt(index);
    }
}
