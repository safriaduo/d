using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using Dawnshard.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class CardSetIncandescenseView : AIncandescenseView
    {
        [SerializeField] private Image defaultCardSetImage;

        protected override void UpdateView(IncandescenseRecord incandescenseRecord)
        {
            defaultCardSetImage.sprite = incandescenseRecord.cardSetFrame;
        }
    }
}
