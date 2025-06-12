using Dawnshard.Network;
using Dawnshard.Presenters;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// This zone view component has the responsability to show the number of cards in a zone
    /// </summary>
    public class OpponentHandZoneView : StaticZoneView
    {
        [Header("Dummy Objects")]
        [SerializeField] protected GameObject cardDummyPrefab;
        [SerializeField] protected Transform cardsDummyParent;
        [SerializeField] protected Transform mobileCardsDummyParent;

        private List<GameObject> spawnedCards = new();

        public override void SetNumCards(int newNumCards)
        {
            int cardsDifference = Mathf.Abs(newNumCards - numCards);

            bool toAdd = numCards < newNumCards;

            for (int i = 0; i < cardsDifference; i++)
            {
                if (toAdd)
                {
                    GameObject dummyCard;
#if UNITY_ANDROID || UNITY_IOS
                    dummyCard = Instantiate(cardDummyPrefab, mobileCardsDummyParent);
#elif UNITY_WEBGL
                    dummyCard = Instantiate(cardDummyPrefab, cardsDummyParent);
#else
                    dummyCard = Instantiate(cardDummyPrefab, cardsDummyParent);
#endif
                    spawnedCards.Add(dummyCard);
                }
                else
                {
                    Destroy(spawnedCards[0]);
                    spawnedCards.RemoveAt(0);
                }
            }
            base.SetNumCards(newNumCards);
        }
    }
}
