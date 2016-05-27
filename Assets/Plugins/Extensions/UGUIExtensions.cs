using UnityEngine;
using UnityEngine.UI;

public static partial class UGUIExtensions
{
    /// <summary>
    /// Disable the button and return whether it was already disabled. Used to
    /// prevent buttons performing their actions multiple times.
    /// </summary>
    public static bool FirstClick(this Button button)
    {
        bool disabled = !button.interactable;

        button.interactable = false;

        return disabled;
    }
}
