using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Views;
using UnityEngine;

namespace Dawnshard.Views
{
    public class DeckPackInteractor : MonoBehaviour
    {
        [SerializeField] private DeckPackView deckPackView;

        private void OnMouseOver()
        {
            deckPackView.DeckPackHovered();
        }

        private void OnMouseDown()
        {
            deckPackView.DeckPackClicked();
        }
    }
}