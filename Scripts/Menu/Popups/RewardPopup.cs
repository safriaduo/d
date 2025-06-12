using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using Dawnshard.Menu;
using Dawnshard.Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{ 
    public class RewardPopup : Popup
    {
        protected override void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                Close();
            });
            HideError();
            SetupPopup();
        }

        private void SetupPopup()
        {

        }

        private void Update()
        {
            if (!parentObject.activeSelf)
            {
                SetupPopup();
            }
        }
        

        private void GoToReward()
        {
            Close();
        }
    }
}
