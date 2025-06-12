using Google.Protobuf.WellKnownTypes;
using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a stat of a player or card
    /// </summary>
    public class ToggleStatView : StatView
    {
        public GameObject obj;
        public bool toggleWhenAbove;
        public bool toggleWhenMoreThanZero = false;

        /// <summary>
        /// Updates the stat value to a new one
        /// </summary>
        public override void SetStat(int originalValue, int value, int maxValue=0)
        {
            if(toggleWhenMoreThanZero)
                obj.SetActive(value>0);
            else if (toggleWhenAbove)
            {
                obj.SetActive(value > originalValue);
            }
            else
            {
                obj.SetActive(value <= originalValue);

            }
        }
    }
}