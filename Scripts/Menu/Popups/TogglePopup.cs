using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePopup : PopupOneButton
{
    private string currentToggleValue = "";

    public override void Open()
    {
        base.Open();
        currentToggleValue = "";
        if(button!=null)
            button.interactable = false;
    }

    public string GetActiveToggle()
    {
        return currentToggleValue;
    }

    public void SetActiveToggle(string toggle)
    {
        currentToggleValue = toggle;
        if(button!=null)
            button.interactable = true;
    }
}
