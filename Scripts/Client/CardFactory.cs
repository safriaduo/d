using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using System;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [SerializeField] private BaseCardView baseCardPrefab;
    [SerializeField] private CardView gameCardPrefab;
    [SerializeField] private UserCardView userCardPrefab;
    [SerializeField] private BaseCardUIView baseCardUIPrefab;

    public static CardFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public IBaseCardPresenter CreateBaseUICard(CardModel cardModel, Transform parent = null)
    {
        var view = Instantiate(baseCardUIPrefab, parent, false);
        return new BaseCardPresenter(view, cardModel);
    }

    public IBaseCardPresenter CreateBaseCard(CardModel cardModel, Transform parent = null)
    {
        var view = Instantiate(baseCardPrefab, parent, false);
        return new BaseCardPresenter(view, cardModel);
    }

    public ICardPresenter CreateGameCard(CardModel cardModel, EventBusManager eventBusManager, Transform parent = null)
    {
        var view = Instantiate(gameCardPrefab, parent, false);
        return new CardPresenter(view, cardModel, eventBusManager);
    }

    public ICardPresenter CreateUserCard(CardModel cardModel, EventBusManager eventBusManager, Transform parent = null)
    {
        var view = Instantiate(userCardPrefab, parent, false);
        return new UserCardPresenter(view, cardModel, eventBusManager);
    }
}
