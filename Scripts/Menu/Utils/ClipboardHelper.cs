using System.Runtime.InteropServices;
using UnityEngine;

public class ClipboardHelper : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CopyTextToClipboard(string text);
#endif

    /// <summary>
    /// Copies the specified text to the clipboard.
    /// For WebGL builds, this uses a JavaScript plugin; in other builds it uses systemCopyBuffer.
    /// </summary>
    public static void CopyToClipboard(string text)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        CopyTextToClipboard(text);
#else
        GUIUtility.systemCopyBuffer = text;
#endif
    }
}