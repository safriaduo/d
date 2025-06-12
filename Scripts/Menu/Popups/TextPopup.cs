using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class TextPopup : Popup
    {
        [SerializeField] private TMP_Text bodyText;

        public void SetBodyText(string text)
        {
            bodyText.text = text;
        }
    } 
}

