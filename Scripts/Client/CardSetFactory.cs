using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using UnityEngine;

public class CardSetFactory : MonoBehaviour
{
    [SerializeField] private InteractableCardSetView interactableCardSetViewPrefab;
    [SerializeField] private InteractableCardSetView interactableCardSetViewAlternatePrefab;
    [SerializeField] private InteractableCardSetView miniInteractableCardSetViewPrefab;
    [SerializeField] private InteractableCardSetView interactableCardSetViewInfoPrefab;
    [SerializeField] private CardSetView cardSetViewPrefab;
    [SerializeField] private CardSetView miniCardSetViewPrefab;

    public static CardSetFactory Instance { get; private set; }

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
    
    public InteractableCardSetPresenter CreateInteractableCardSetView(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(interactableCardSetViewPrefab, parent, false);
        return new InteractableCardSetPresenter(view, cardSetModel);
    }
    
    public InteractableCardSetPresenter CreateMiniInteractableCardSetView(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(miniInteractableCardSetViewPrefab, parent, false);
        return new InteractableCardSetPresenter(view, cardSetModel, true);
    }
    
    public InteractableCardSetPresenter CreateInteractableCardSetViewAlternate(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(interactableCardSetViewAlternatePrefab, parent, false);
        return new InteractableCardSetPresenter(view, cardSetModel, true);
    }
    
    public InteractableCardSetPresenter CreateInteractableCardSetViewInfo(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(interactableCardSetViewInfoPrefab, parent, false);
        return new InteractableCardSetPresenter(view, cardSetModel);
    }


    public CardSetPresenter CreateCardSetView(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(cardSetViewPrefab, parent, false);
        return new CardSetPresenter(view, cardSetModel);
    }
    
    public CardSetPresenter CreateMiniCardSetView(CardSetModel cardSetModel, Transform parent = null)
    {
        var view = Instantiate(miniCardSetViewPrefab, parent, false);
        return new CardSetPresenter(view, cardSetModel, true);
    }
}