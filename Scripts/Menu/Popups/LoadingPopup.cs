using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class LoadingPopup : Popup
    {
        [SerializeField] private float timeoutTimer = 60f;
        [SerializeField] private MMFeedbacks loadingAnimation;
        private float timer = 0f;
        
        public static LoadingPopup Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }

        private void Update()
        {
            if(parentObject.activeSelf)
                timer+=Time.deltaTime;
            if (timer >= timeoutTimer)
            {
                ErrorPopup.Instance.Open();
                ErrorPopup.Instance.SetErrorText("It seems like you are not connected.\n Check your connection and try again.");
                timer = 0f;
                Close();
            }
        }

        public override void Open()
        {
            base.Open();
            timer = 0f;
        }

        public override void Close()
        {
            loadingAnimation.StopFeedbacks();
            base.Close();
            timer = 0f;
        }
    }
}