using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using UnityEngine;

public class DeckFactory : MonoBehaviour
{
    [SerializeField] private InteractableDeckView interactableDeckViewPrefab;
    [SerializeField] private InteractableDeckView interactableDeckViewCrossPrefab;
    [SerializeField] private EditInteractableDeckView editInteractableDeckViewCrossPrefab;
    [SerializeField] private DeckView deckViewPrefab;

    public static DeckFactory Instance { get; private set; }

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

    public InteractableDeckPresenter CreateInteractableDeckView(DeckModel deckModel, Transform parent = null)
    {
        var view = Instantiate(interactableDeckViewPrefab, parent, false);
        return new InteractableDeckPresenter(view, deckModel);
    }
    
    public InteractableDeckPresenter CreateInteractableDeckViewCross(DeckModel deckModel, Transform parent = null)
    {
        var view = Instantiate(interactableDeckViewCrossPrefab, parent, false);
        return new InteractableDeckPresenter(view, deckModel);
    }
    
    public EditInteractableDeckPresenter CreateEditInteractableDeckViewCross(DeckModel deckModel, Transform parent = null)
    {
        var view = Instantiate(editInteractableDeckViewCrossPrefab, parent, false);
        return new EditInteractableDeckPresenter(view, deckModel);
    }

    public DeckPresenter CreateDeckView(DeckModel deckModel, Transform parent = null)
    {
        var view = Instantiate(deckViewPrefab, parent, false);
        return new DeckPresenter(view, deckModel);
    }
}
