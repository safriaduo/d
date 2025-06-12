using Dawnshard.Menu;
using Dawnshard.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Presenters;
using Dawnshard.Views;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreateDeckPopup : Popup
{
    [SerializeField] protected Button saveDeckButton;
    [SerializeField] protected Button cancelDeckButton;
    [SerializeField] private MMFeedbacks openAnimation;
    [SerializeField] private MMFeedbacks openPartialAnimation;
    [SerializeField] private MMFeedbacks waitingAnimation;
    [SerializeField] private Transform closePosition;
    [SerializeField] private GameObject setParent;
    [SerializeField] private TMP_InputField deckName;

    private List<InteractableCardSetPresenter> setPresenters = new();
    private int setIndex = 0;
    
    private bool isOpen = false;
    
    private Action OnPopupClosed;
    private Action<DeckModel> OnDeckSaved;

    protected override void Start()
    {
        setPresenters = new(3);
        base.Start();
        saveDeckButton.onClick.AddListener(SaveDeck);
        cancelDeckButton.onClick.AddListener(GoBack);
    }

    private void Update()
    {
        if(setIndex < 3)
            saveDeckButton.interactable = false;
        else
            saveDeckButton.interactable = true;
    }

    private void SaveDeck()
    {
        var deck = new DeckModel();
        deck.Name = deckName.text;
        deck.CardSetIds = new List<int> {setPresenters[0].Model.ItemId, setPresenters[1].Model.ItemId, setPresenters[2].Model.ItemId};
        OnDeckSaved?.Invoke(deck);
    }

    public void AddSet(CardSetModel cardSetModel, Action OnRemoveSet)
    {
        var miniSet = CardSetFactory.Instance.CreateMiniInteractableCardSetView(cardSetModel, setParent.transform);
        miniSet.ToggleButtonListener(true, ()=>
        {
            miniSet.ToggleVisibility(false, ()=>
            {
                RemoveSet(miniSet);
                OnRemoveSet?.Invoke();
            });
        });
        setPresenters.Add(miniSet);
        setIndex++;
    }

    public void RemoveSet(InteractableCardSetPresenter cardSetPresenter)
    {
        setIndex--;
        setPresenters.Remove(cardSetPresenter);
        cardSetPresenter.Destroy();
    }
    
    public void RemoveAllSet()
    {
        foreach (var setPresenter in setPresenters.ToList())
        {
            setIndex--;
            setPresenters.Remove(setPresenter);
            setPresenter.Destroy();
        }
    }

    private void GoBack()
    {
        OnPopupClosed?.Invoke();
    }
    public void SetOnSaved(Action<DeckModel> OnSaved)
    {
        OnDeckSaved = OnSaved;
    }

    public override void Open()
    {
        if (isOpen)
            return;
        base.Open();
        for (int j = setPresenters.Count-1; j>=0; j--)
        {
            RemoveSet(setPresenters[j]);
        }

        deckName.text = GetUniqueDeckName(GameController.Instance.Decks.ConvertAll(deckModel => deckModel.Name));
        deckName.selectionStringFocusPosition=deckName.text.Length-1;
        openAnimation.PlayFeedbacks();
        isOpen = true;
    }
    
    private string GetUniqueDeckName(List<string> existingNames)
    {
        int i = 0;
        string candidateName;
        while (true)
        {
            candidateName = "Deck " + i;
            if (!existingNames.Contains(candidateName))
            {
                return candidateName;
            }
            i++;
        }
    }

    public override void Close()
    {
        if (!isOpen)
            return;
        StartCoroutine(CloseCoroutine(openAnimation));
    }

    private IEnumerator CloseCoroutine(MMFeedbacks closeAnimation)
    {
        closeAnimation.PlayFeedbacks();
        yield return new WaitWhile(() => closeAnimation.IsPlaying);
        isOpen = false;
        base.Close();
        //OnPopupClosed();
    }

    public void ClosePartial()
    {
        if (!isOpen)
            return;
        StartCoroutine(CloseCoroutine(openPartialAnimation));
    }
    
}
