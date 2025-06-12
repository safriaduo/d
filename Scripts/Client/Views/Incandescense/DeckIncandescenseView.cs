using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using Dawnshard.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class DeckIncandescenseView : AIncandescenseView
    {
        [SerializeField] private Image defaultDeckImage;

        protected override void UpdateView(IncandescenseRecord incandescenseRecord)
        {
            defaultDeckImage.sprite = incandescenseRecord.deckFrame;
        }
    }
}
