using System;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class ResumeSessionPopup : Popup
    {
        protected override void Start()
        {
            base.Start();
            Open();
            ResumeSessionAsync();
        }

        private async void ResumeSessionAsync()
        {
            try
            {
                await GameController.Instance.Connect();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            Close();
        }
    }
}
