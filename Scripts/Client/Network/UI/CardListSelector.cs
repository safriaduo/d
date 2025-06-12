using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Dawnshard.Network;

namespace Dawnshard.Network
{
    public class CardListSelector : MonoBehaviour
    {
        [SerializeField] private int nOfSelectableCards;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private GameObject selectedCardParent;
        [SerializeField] private GameObject highlightSelectedSlotTemplate;

        private Action<List<ICardPresenter>> OnTargetSelected;
        private Action OnBack;
        private List<ICardPresenter> selectedCardPresenters = new List<ICardPresenter>();
        private List<GameObject> highlightSlots = new();

        public void Initialize(int nOfSelectableCards, Action OnBack, Action<List<ICardPresenter>> OnTargetSelected = null, string titleText="")
        {
            /*if (nOfSelectableCards > 0) {
                foreach (ICardPresenter cardPresenter in cardPresenters) {
                    cardPresenter.RegisterInputCallback(Dawnshard.Presenters.UserInput.MouseUp, () => SelectCard(cardPresenter));
                }
            }*/
            if (this.titleText != null && titleText!="")
            {
                this.titleText.text = titleText;
            }
            this.nOfSelectableCards = nOfSelectableCards;
            this.OnTargetSelected = OnTargetSelected;
            this.OnBack = OnBack;
            InitializeButton();
        }

        public void SelectCard(ICardPresenter cardPresenter)
        {
            if (selectedCardPresenters.Find((card) => card.Model.Name == cardPresenter.Model.Name) != null)
            {
                RemoveSelectedCard(cardPresenter);
            }
            else
            {
                if (selectedCardPresenters.Count < nOfSelectableCards)
                {
                    AddSelectedCard(cardPresenter);
                }
                else if (selectedCardPresenters.Count == nOfSelectableCards)
                {
                    ReplaceSelectedCard(cardPresenter);
                }
            }

            if (selectedCardPresenters.Count == nOfSelectableCards)
            {
                EnableConfirmButton();
            }
            else if (selectedCardPresenters.Count == nOfSelectableCards - 1)
            {
                EnableBackButton();
            }
        }

        private void EnableBackButton()
        {
            confirmButtonText.text = "Back";
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { OnBack(); ClearUI(); });
        }

        private void EnableConfirmButton()
        {
            Debug.Log("Card selected: " + selectedCardPresenters.Count);
            confirmButtonText.text = "Confirm";
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { OnTargetSelected(selectedCardPresenters); ClearUI(); });
        }

        private void ReplaceSelectedCard(ICardPresenter cardPresenter)
        {
            Debug.Log("subcard");
            var oldSelected = selectedCardPresenters[0];
            selectedCardPresenters.RemoveAt(0);
            oldSelected.Destroy();
            selectedCardPresenters.Add(CardFactory.Instance.CreateGameCard(cardPresenter.Model, null, GetFirstFreeSlot()));
        }

        private void AddSelectedCard(ICardPresenter cardPresenter)
        {
            selectedCardPresenters.Add(CardFactory.Instance.CreateGameCard(cardPresenter.Model, null, GetFirstFreeSlot()));
        }

        private void RemoveSelectedCard(ICardPresenter cardPresenter)
        {
            var oldSelected = selectedCardPresenters.Find((card) => card.Model.Name == cardPresenter.Model.Name);
            selectedCardPresenters.Remove(oldSelected);
            oldSelected.Destroy();
        }

        public void InitializeButton()
        {
            confirmButton.onClick.AddListener(() => { OnBack(); ClearUI(); });
            confirmButtonText.text = "Back";

            //Create the highlightedSlot
            for (int i = 0; i < nOfSelectableCards; i++)
            {
                highlightSlots.Add(Instantiate(highlightSelectedSlotTemplate, selectedCardParent.transform));
            }
        }

        public void ClearUI()
        {
            confirmButton.onClick.RemoveAllListeners();
            foreach (GameObject go in highlightSlots)
            {
                Destroy(go);
            }
        }

        public Transform GetFirstFreeSlot()
        {
            foreach (GameObject go in highlightSlots)
            {
                if (go.transform.GetChild(0).childCount == 0)
                    return go.transform.GetChild(0);
            }
            return highlightSlots[0].transform.GetChild(0);
        }
    }
}
