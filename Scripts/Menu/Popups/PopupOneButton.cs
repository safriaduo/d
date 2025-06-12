// Copyright (C) 2016-2022 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using Dawnshard.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupOneButton : Popup
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] protected Button button;
    [SerializeField] private TMP_Text popupButtonText;
    private const string DEFAULT_BUTTON_TEXT = "Close";

    public void SetUpPopupButton(string text, string buttonText=DEFAULT_BUTTON_TEXT, Action onClick=null, string titleText="")
    {
        popupText.text = text;
        popupButtonText.text = buttonText;
        button.onClick.AddListener(() => onClick());
        if (!string.IsNullOrEmpty(titleText))
        {
            this.titleText.text = titleText;
        }
    }
}