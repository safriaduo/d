using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class NotificationPopup : TextPopup
    {
        public override void Close()
        {
            base.Close();
            Destroy(gameObject);
        }
    }
}