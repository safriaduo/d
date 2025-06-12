using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] protected TMP_Text errorText;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected GameObject parentObject;
        [SerializeField] protected bool darkenBackground;

        protected virtual void Start()
        {
            if(closeButton!=null)
                closeButton.onClick.AddListener(Close);

            HideError();
        }

        public virtual void Close()
        {
            parentObject.SetActive(false);
            ToggleOverlay(false);

        }

        public virtual void Open()
        {
            parentObject.SetActive(true);
            ToggleOverlay(true);
            HideError();
        }

        protected void ToggleOverlay(bool enabled)
        {
            if (enabled && darkenBackground)
            {
                DarkenBackground.Instance.Open();
            }
            else if(!enabled && darkenBackground)
            {
                DarkenBackground.Instance.Close();
            }
        }

        protected void HideError()
        {
            if(errorText!=null)
                errorText.gameObject.SetActive(false);
        }

        protected void ShowError(string message)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = message;
        }
    }
}