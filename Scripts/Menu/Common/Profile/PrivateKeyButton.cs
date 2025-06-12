using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class PrivateKeyButton : MonoBehaviour
{
    [SerializeField] private TMP_Text privateKeyText;
    [SerializeField] private Button copyButton;
    [SerializeField] private TMP_Text privateKeyLabel; 

    private void OnEnable()
    {
        copyButton.gameObject.SetActive(true);
        privateKeyLabel.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(GameController.Instance.PrivateKey))
        {
            copyButton.gameObject.SetActive(false);
            privateKeyLabel.gameObject.SetActive(false);
            return;
        }
        copyButton.onClick.RemoveAllListeners();
        copyButton.onClick.AddListener(OnAddressButtonClick);
        privateKeyText.text = MaskAddress(GameController.Instance.PrivateKey);
    }

    public void UpdateAddressText(string address)
    {
        privateKeyText.text = MaskAddress(address);
    }

    /// <summary>
    /// Called when the button is clicked.
    /// Copies the full wallet address to the clipboard.
    /// </summary>
    public void OnAddressButtonClick()
    {
        // Copy the full wallet address to the clipboard.
        GUIUtility.systemCopyBuffer = GameController.Instance.PrivateKey;
    }

    /// <summary>
    /// Returns a masked version of the wallet address.
    /// Keeps the first 6 and last 4 characters; replaces the middle with ellipses.
    /// </summary>
    private string MaskAddress(string address)
    {
        return $"**************";
    }
}