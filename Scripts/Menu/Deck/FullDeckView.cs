using Dawnshard.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class FullDeckView : DeckView
    {
        [SerializeField] private SpriteRenderer artworkSprite;
        [SerializeField] private Button selectButton;
        [SerializeField] private AIncandescenseView incandescenseView;

        private void OnDestroy()
        {
            selectButton.onClick.RemoveAllListeners();
        }

        public void AddOnClick(Action action)
        {
            selectButton.onClick.AddListener(action.Invoke);
        }

        public void SetIncandescense(string incandescense)
        {
            incandescenseView.SetIncandescense(incandescense);
        }

        public void SetArtwork(Sprite sprite)
        {
            artworkSprite.sprite = sprite;
        }
    }
}
