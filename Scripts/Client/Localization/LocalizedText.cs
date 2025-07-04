using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;

    private TMP_Text tmpText;
    private Text uiText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            return;
        }

        string value = LocalizationManager.Instance.Get(key);
        if (tmpText != null)
        {
            tmpText.text = value;
        }
        else if (uiText != null)
        {
            uiText.text = value;
        }
    }

    public void SetKey(string newKey)
    {
        key = newKey;
        UpdateText();
    }
}

