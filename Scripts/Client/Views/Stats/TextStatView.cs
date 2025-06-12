using System;
using TMPro;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a stat of a player or card as text
    /// </summary>
    public class TextStatView : StatView
    {
        [SerializeField] private TMP_Text statText;
        [SerializeField] private Color aboveOriginalValueColor;
        [SerializeField] private Color belowOriginalValueColor;

        private Color defaultColor;

        private void OnEnable()
        {
            //TODO: fix invisible text
            //           defaultColor = statText.color;
            defaultColor = Color.white;
            statText.color = defaultColor;
        }

        /// <summary>
        /// Updates the stat value to a new one
        /// </summary>
        public override void SetStat(int originalValue, int value, int maxValue = 0)
        {
            if (maxValue == 0)
            {
                maxValue = originalValue;
            }
            statText.text = value.ToString();

            if (value < maxValue)
            {
                statText.color = belowOriginalValueColor;
            }
            else if (value > originalValue)
            {
                statText.color = aboveOriginalValueColor;
            }
            else
            {
                statText.color = defaultColor;
            }
        }
    }
}