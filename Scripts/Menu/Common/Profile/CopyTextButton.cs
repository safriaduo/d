using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CopyTextButton : MonoBehaviour
{
    [SerializeField] private TMP_Text copyText;
    [SerializeField] private Button copyButton;
    [FormerlySerializedAs("privateKeyLabel")] [SerializeField] private TMP_Text textLabel; 

    private enum StringInfo
    {
        Address,
        PrivateKey,
        None
    }
    
    [SerializeField] private StringInfo stringInfo;
    private void OnEnable()
    {
        SetUpCopyButton();
    }

    private void Start()
    {
        SetUpCopyButton();
    }

    private void SetUpCopyButton()
    {
        copyButton.gameObject.SetActive(true);
        if(textLabel!=null)
            textLabel.gameObject.SetActive(true);
        if (stringInfo == StringInfo.PrivateKey && string.IsNullOrEmpty(GameController.Instance.PrivateKey))
        {
            copyButton.gameObject.SetActive(false);
            if(textLabel!=null)
                textLabel.gameObject.SetActive(false);
            return;
        }
        if (stringInfo == StringInfo.Address && string.IsNullOrEmpty(GameController.Instance.Address))
        {
            copyButton.gameObject.SetActive(false);
            if(textLabel!=null)
                textLabel.gameObject.SetActive(false);
            return;
        }
        if (stringInfo == StringInfo.None && string.IsNullOrEmpty(GameController.Instance.Session.UserId))
        {
            copyButton.gameObject.SetActive(false);
            if(textLabel!=null)
                textLabel.gameObject.SetActive(false);
            return;
        }
        copyButton.onClick.RemoveAllListeners();
        copyButton.onClick.AddListener(OnAddressButtonClick);
        if(stringInfo == StringInfo.PrivateKey)
            copyText.text = MaskAddress(GameController.Instance.PrivateKey);
        else if (stringInfo == StringInfo.Address)
            copyText.text = MaskAddress(GameController.Instance.Address);
        else
            copyText.text = MaskAddress(GameController.Instance.Session.UserId);
    }

    /// <summary>
    /// Called when the button is clicked.
    /// Copies the full wallet address to the clipboard.
    /// </summary>
    public void OnAddressButtonClick()
    {
        // Copy the full wallet address to the clipboard.
        switch (stringInfo)
        {
            case StringInfo.Address:
                ClipboardHelper.CopyToClipboard(GameController.Instance.Address);
                break;
            case StringInfo.PrivateKey:
                ClipboardHelper.CopyToClipboard(GameController.Instance.PrivateKey);
                break;
            case StringInfo.None:
                ClipboardHelper.CopyToClipboard(GameController.Instance.Session.UserId);
                break;
        }
    }

    /// <summary>
    /// Returns a masked version of the wallet address.
    /// Keeps the first 6 and last 4 characters; replaces the middle with ellipses.
    /// </summary>
    private string MaskAddress(string clear)
    {
        if (stringInfo == StringInfo.Address)
        {
            if (string.IsNullOrEmpty(clear))
                return "";

            // Check that the address is long enough to mask.
            if (clear.Length <= 10)
                return clear;

            string firstPart = clear.Substring(0, 6);
            string lastPart = clear.Substring(clear.Length - 4);
            return $"{firstPart}...{lastPart}";
        }
        else if (stringInfo == StringInfo.PrivateKey)
        {
            return $"**************";
        }
        else
        {
            return clear;
        }
    }
}
