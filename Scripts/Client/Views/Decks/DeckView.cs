using System;
using Dawnshard.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a player deck view
    /// </summary>
    public class DeckView : MonoBehaviour
    {
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] private AWorldView[] worldViews;
        [SerializeField] private Image artworkImage;
        [SerializeField] private AIncandescenseView incandescenseImage;
        [SerializeField] private AIncandescenseView incandescenseImageHighlight;
        [SerializeField] private Button infoButton;

        private void Awake()
        {
            if (infoButton != null)
            {
                infoButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Set the name of a deck
        /// </summary>
        public void SetName(string deckName)
        {
            nameText.text = deckName;
        }

        /// <summary>
        /// Sets the world of a deck
        /// </summary>
        public void SetWorlds(string[] worldIds) {
            for (int i = 0; i < worldIds.Length; i++) {
                worldViews[i].SetWorld(worldIds[i]);
            }
        }

        public void SetArtwork(Sprite sprite)
        {
            artworkImage.sprite = sprite;
        }

        public void SetIncandescense(string incandescense)
        {
            incandescenseImage.SetIncandescense(incandescense);
            incandescenseImageHighlight.SetIncandescense(incandescense);
        }
        
        public void AddButtonInfoListener(Action OnClickAction)
        {
            if(infoButton==null)
                return;
            infoButton.gameObject.SetActive(true);
            infoButton.onClick.AddListener(() => OnClickAction());
        }

        public void RemoveButtonInfoListener(Action OnClickAction)
        {
            if(infoButton==null)
                return;
            infoButton.gameObject.SetActive(false);
            infoButton.onClick.RemoveListener(() => OnClickAction());
        }
    }
}