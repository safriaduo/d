// Copyright (C) 2016-2022 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using Dawnshard.Menu;
using Febucci.UI;
using TMPro;
using UnityEngine.UI;

public class PopupInputField : Popup
{
    public TMP_InputField inputField;
    public TextMeshProUGUI text;
    public Button button;
    public TextMeshProUGUI buttonText;

    /// <summary>
    /// Display a message in the popup using the built-in error text.
    /// </summary>
    public void ShowMessage(string message)
    {
        ShowError(message);
    }

    public void SetTextAnimation(string anim)
    {
        text.GetComponent<TextAnimator_TMP>().SetText($"<{anim}>{text.text}");
    }
}