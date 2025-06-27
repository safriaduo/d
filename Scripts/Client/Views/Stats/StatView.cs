using System;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a stat of a player or card
    /// </summary>
    public class StatView : MonoBehaviour
    {
        /// <summary>
        /// Updates the stat value to a new one
        /// </summary>
        public virtual void SetStat(int originalValue, int value, int maxValue=0)
        {

        }
    }
}