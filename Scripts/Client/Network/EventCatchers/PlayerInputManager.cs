using System.Collections;
using System.Collections.Generic;
using Dawnshard.Presenters;
using Dawnshard.Network;
using UnityEngine;
using System;

namespace Dawnshard.Network
{
    public class PlayerInputManager : EventCatcher
    {
        private float timeOut = 2f;
        private float timer = 0f;
        private bool isLocked = false;

        private void Update() {
            if (!isLocked)
                return;
            timer += Time.deltaTime;
            if(timer > timeOut) {
                timer = 0f;
                LockPlayerInteractions(false);
            }
        }

        private void LockPlayerInteractions(bool enable)
        {
            Cursor.visible = !enable;
            if (enable) {
                Cursor.lockState = CursorLockMode.Locked;
                isLocked = true;
            }
            else {
                Cursor.lockState = CursorLockMode.None;
                isLocked = false;
                timer = 0f;
            }
        }

        public override void OnGameUpdate(EventDispatcher.GameEventType gameEvent, GameUpdate gameUpdate)
        {
            if (gameEvent == EventDispatcher.GameEventType.GameUpdateStart)
            {
                LockPlayerInteractions(true);
            }
            else if (gameEvent == EventDispatcher.GameEventType.GameUpdateEnd)
            {
                LockPlayerInteractions(false);
            }
        }
    }
}
