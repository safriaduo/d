using Dawnshard.Network;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// This zone view component has the responsability to show the cards in a particular 
    /// zone and order them.
    /// </summary>
    public class DynamicZoneView : ZoneView, IZoneView
    {
        [Header("Dummy Objects")]
        [SerializeField]
        protected GameObject cardDummyPrefab;

        [SerializeField] protected RectTransform cardsDummyParent;

        protected readonly List<ICardPresenter> cardPresenters = new();
        protected readonly List<GameObject> spawnedDummyObjects = new();

        public override void SetCards(List<ICardPresenter> cardPresenters)
        {
            var presenters = new List<ICardPresenter>();
            presenters.AddRange(this.cardPresenters);

            //Remove presenters no longer in the zone
            foreach (var cardPresenter in presenters)
            {
                //If no model found for the presenter, remove the presenter
                if (cardPresenters.Find(m => m.Model.InstanceId == cardPresenter.Model.InstanceId) == null)
                {
                    RemoveCard(cardPresenter);
                }
            }

            //Add missing presenters
            foreach (var cardPresenter in cardPresenters)
            {
                //If no presenter found for that model, add it
                if (this.cardPresenters.Find(p => p.Model.InstanceId == cardPresenter.Model.InstanceId) == null)
                    AddCard(cardPresenter);
            }

            DestroyNotFoundPresenterZoneTransform();
            StartCoroutine(CoroutineReorderCards());
        }

        protected override void AddCard(ICardPresenter cardPresenter)
        {
            InitializeCardPresenter(cardPresenter);
            StartCoroutine(CoroutineReorderCards());
        }

        protected override void RemoveCard(ICardPresenter cardPresenter)
        {
            DestroyCardPresenter(cardPresenter);
            StartCoroutine(CoroutineReorderCards());
        }

        public IEnumerator CoroutineReorderCards()
        {
            yield return new WaitForEndOfFrame();
            ReorderCards();
        }

        public override void SetMaxCards(int maxCards)
        {
        }

        public override void SetNumCards(int numCards)
        {
        }

        protected virtual void ReorderCards()
        {
            foreach (var presenter in cardPresenters)
            {
                presenter.AnimateTo(presenter.ZoneTransform.position);
            }
        }

        protected virtual void InitializeCardPresenter(ICardPresenter cardPresenter)
        {
            if (cardPresenters.Contains(cardPresenter) == false)
            {
                cardPresenters.Add(cardPresenter);
                Debug.Log($"[ZONEVIEW] Added card presenter to my list: {gameObject.name}", gameObject);
                CreateZoneTransform(cardPresenter);
            }
        }

        protected virtual void DestroyCardPresenter(ICardPresenter cardPresenter)
        {
            if (cardPresenters.Contains(cardPresenter))
            {
                Debug.Log($"[ZONEVIEW] Removed card presenter to my list: {gameObject.name}", gameObject);
                cardPresenters.Remove(cardPresenter);
                DestroyZoneTransform(cardPresenter);
            }

        }

        /// <summary>
        /// Creates a empty object ordered and sets it as zone target for the card
        /// </summary>
        protected virtual void CreateZoneTransform(ICardPresenter cardPresenter)
        {
            var dummyTarget = Instantiate(cardDummyPrefab, cardsDummyParent);
            cardPresenter.ZoneTransform = dummyTarget.transform;
            spawnedDummyObjects.Add(dummyTarget);

            LayoutRebuilder
                .ForceRebuildLayoutImmediate(
                    cardsDummyParent); // We need to wait one frame so the Zone transform position is updated

            Debug.Log($"Zone transform created for card presenter: {cardPresenter.Model.InstanceId}");
        }

        /// <summary>
        /// Destroy a zone transform of a card
        /// </summary>
        protected virtual void DestroyZoneTransform(ICardPresenter cardPresenter)
        {
            if(cardPresenter.ZoneTransform == null || !spawnedDummyObjects.Contains(cardPresenter.ZoneTransform.gameObject))
                return;
            var dummy = spawnedDummyObjects.Find(p => p == cardPresenter.ZoneTransform.gameObject);

            if (spawnedDummyObjects.Remove(dummy))
            {
                Destroy(dummy);
                cardPresenter.ZoneTransform = null;

                LayoutRebuilder
                    .ForceRebuildLayoutImmediate(
                        cardsDummyParent); // We need to wait one frame so the Zone transform position is updated

                Debug.Log($"Zone transform destroyed for card presenter: {cardPresenter.Model.InstanceId}");
            }
            else
                Debug.LogWarning("Cannot destroy dummy object. This can lead to view errors");
        }

        /*protected virtual void DestroyNotFoundPresenterZoneTransform(List<ICardPresenter> cardPresenters)
        {
            var spawnedDummyObjectsTmp = spawnedDummyObjects;
            foreach (var cardPresenter in this.cardPresenters)
            {
                var dummy = spawnedDummyObjectsTmp.Find(p => p == cardPresenter.ZoneTransform.gameObject);

                spawnedDummyObjectsTmp.Remove(dummy);
            }

            foreach (var spawnedDummyObject in spawnedDummyObjectsTmp)
            {
                var dummy = spawnedDummyObject;
                if (spawnedDummyObjects.Remove(dummy))
                {
                    Destroy(dummy);

                    LayoutRebuilder
                        .ForceRebuildLayoutImmediate(
                            cardsDummyParent); // We need to wait one frame so the Zone transform position is updated

                    Debug.Log($"Zone transform destroyed for null card presenter");
                }
                
            }
        }*/
        
        protected virtual void DestroyNotFoundPresenterZoneTransform()
        {
            foreach (var cardPresenter in this.cardPresenters)
            {
                DestroyZoneTransform(cardPresenter);
            }
            foreach (var spawnedDummyObject in spawnedDummyObjects.ToList())
            {
                Destroy(spawnedDummyObject);
            }
            foreach (var cardPresenter in this.cardPresenters)
            {
                CreateZoneTransform(cardPresenter);
            }

        }
    }
}