using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WalletAddressButton : MonoBehaviour
{
    [SerializeField] private TMP_Text addressText;
    [SerializeField] private Button copyButton;

    private void OnEnable()
    {
        copyButton.onClick.RemoveAllListeners();
        copyButton.onClick.AddListener(OnAddressButtonClick);
        addressText.text = MaskAddress(GameController.Instance.Address);
    }

    public void UpdateAddressText(string address)
    {
        addressText.text = MaskAddress(address);
    }

    /// <summary>
    /// Called when the button is clicked.
    /// Copies the full wallet address to the clipboard.
    /// </summary>
    public void OnAddressButtonClick()
    {
        // Copy the full wallet address to the clipboard.
        GUIUtility.systemCopyBuffer = GameController.Instance.Address;
    }

    /// <summary>
    /// Returns a masked version of the wallet address.
    /// Keeps the first 6 and last 4 characters; replaces the middle with ellipses.
    /// </summary>
    private string MaskAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return "";

        // Check that the address is long enough to mask.
        if (address.Length <= 10)
            return address;

        string firstPart = address.Substring(0, 6);
        string lastPart = address.Substring(address.Length - 4);
        return $"{firstPart}...{lastPart}";
    }
}