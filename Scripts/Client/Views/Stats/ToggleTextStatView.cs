using System;
using TMPro;
using UnityEngine;

namespace Dawnshard.Views
{
    public class ToggleTextStatView : ToggleStatView
    {
        [SerializeField] private TMP_Text statText;

        public override void SetStat(int originalValue, int value, int maxValue=0)
        {
            base.SetStat(originalValue, value);
            statText.text = value.ToString();
        }

    }
}