using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public static partial class UnityExtensions 
{
    /// <summary>
    /// Force an item to be contained in a collection or not based on a bool
    /// input.
    /// </summary>
    public static void SetMembership<T>(this ICollection<T> collection,
                                        T item,
                                        bool membership)
    {
        if (membership) collection.Add(item);
        else            collection.Remove(item);
    }

    /// <summary>
    /// Return a Color with specific values replaced. Convenience method.
    /// </summary>
    public static Color With(this Color color,
                             float r = -1,
                             float g = -1,
                             float b = -1,
                             float a = -1)
    {
        if (r >= 0) color.r = r;
        if (g >= 0) color.g = g;
        if (b >= 0) color.b = b;
        if (a >= 0) color.a = a;

        return color;
    }

    /// <summary>
    /// Return a Vector3 with specific values replaced. Convenience method.
    /// </summary>
    public static Vector3 With(this Vector3 vector,
                               float x = float.NaN,
                               float y = float.NaN,
                               float z = float.NaN)
    {
        // NaN != NaN, check if the arguments are default or not
        #pragma warning disable 1718
        if (x == x) vector.x = x;
        if (y == y) vector.y = y;
        if (z == z) vector.z = z;
        #pragma warning restore 1718

        return vector;
    }

    /// <summary>
    /// Coroutine to play an animator state by name and wait for it to finish.
    /// Probably not very reliable, I don't understand animators.
    /// </summary>
    public static IEnumerator PlayAndWait(this Animator animator,
                                          string name,
                                          bool skip=false)
    {
        animator.Play(name);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (skip)
        {
            animator.playbackTime = state.length;

            yield break;
        }
        else
        {
            yield return new WaitForSeconds(state.length);
        }
    }
}
