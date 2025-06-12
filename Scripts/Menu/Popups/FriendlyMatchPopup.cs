using Dawnshard.Menu;
using Dawnshard.Presenters;
using Dawnshard.Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyMatchPopup : Popup
{
    [SerializeField] private Button playButton;
    [SerializeField] private Transform deckParent;

    private readonly List<InteractableDeckPresenter> deckPresenters = new();
    private DeckModel selectedDeck;
    private string friendId;
    private string matchId;
    private bool isSender;

    protected override void Start()
    {
        base.Start();
        playButton.onClick.AddListener(OnPlayClicked);
    }

    public void OpenAsSender(string friendId)
    {
        this.friendId = friendId;
        isSender = true;
        BuildDeckList();
        playButton.interactable = false;
        base.Open();
    }

    public void OpenAsReceiver(string matchId)
    {
        this.matchId = matchId;
        isSender = false;
        BuildDeckList();
        playButton.interactable = false;
        base.Open();
    }

    private void BuildDeckList()
    {
        foreach (var presenter in deckPresenters)
        {
            Object.Destroy(presenter);
        }
        deckPresenters.Clear();

        foreach (var deck in GameController.Instance.Decks)
        {
            var presenter = DeckFactory.Instance.CreateInteractableDeckView(deck, deckParent);
            presenter.ToggleButtonListener(true, () => OnDeckSelected(presenter));
            deckPresenters.Add(presenter);
        }
    }

    private void OnDeckSelected(InteractableDeckPresenter presenter)
    {
        selectedDeck = presenter.Model;
        playButton.interactable = true;
    }

    private void OnPlayClicked()
    {
        if (selectedDeck == null)
            return;

        if (isSender)
        {
            GameController.Instance.SendFriendlyMatchRequest(selectedDeck.Name, friendId);
        }
        else
        {
            GameController.Instance.AcceptFriendlyMatch(matchId, selectedDeck.Name);
        }

        Close();
    }
}
