using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditInteractableDeckView : InteractableDeckView
{
    [SerializeField] private TMP_InputField editText;
    [SerializeField] private Button enableEditButton;

    private void Start()
    {
        editText.enabled = false;
        editText.onDeselect.AddListener(text => editText.enabled = false);
        enableEditButton.onClick.AddListener(()=>
        {
            editText.enabled = !editText.enabled;
            if (editText.enabled)
            {
                editText.text=nameText.text;
                editText.Select();
            }
        });
    }

    public void AddEditListener(Action<string> OnClickAction)
    {
        editText.onSubmit.AddListener(text => { OnClickAction(text); });
        editText.onDeselect.AddListener(text => { OnClickAction(text); });
    }

    public void RemoveEditListener(Action<string> OnClickAction)
    {
        editText.onSubmit.RemoveListener( text => { OnClickAction(text); });
        editText.onDeselect.RemoveListener(text => { OnClickAction(text); });
    }

    /*public Action<UserInput> OnUserInput { get; set; }

    #region Interactions
    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        OnUserInput?.Invoke(UserInput.HoverStarted);
    }

    private void OnMouseExit()
    {
        OnUserInput?.Invoke(UserInput.HoverEnded);
    }

    private void OnMouseDown()
    {
        OnUserInput?.Invoke(UserInput.MouseDown);
    }

    #endregion*/

    public void EnableEditInteraction(bool enable)
    {
        enableEditButton.interactable = enable;
    }
}
