using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class ErrorPopup : Popup
    {
        public static ErrorPopup Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void SetErrorText(string text)
        {
            text = Regex.Replace(text, "custom id", "Wallet Address", RegexOptions.IgnoreCase);
            ShowError(text);
        }

        public override void Close()
        {
            base.Close();
            LoadingPopup.Instance.Close();
        }
    }
}