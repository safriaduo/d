using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using Dawnshard.Network;
using Dawnshard.Views;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class RewardsState : AState
    {
        [SerializeField] private GameObject packParent;
        [SerializeField] private DeckPackView deckPack;
        [SerializeField] private Button goBackToCollectionButton;
        [SerializeField] private GameObject goBackToCollectionGameObject;
        [SerializeField] private Transform setParent;
        [SerializeField] private CanvasGroup unpackBackground;
        [SerializeField] private Transform viewsParent;
        [SerializeField] private PackItemView packItemPrefab;
        private DeckPackView deckPackInstance;

        public override void Enter(AState from)
        {
            
            gameObject.SetActive(true);

            var optionMenu = new Dictionary<string, Action>();

            optionMenu["Packs"] = ()=>
            {
                DeleteAllViews();
                GetPack(Constants.StandardPack);
                GetPack(Constants.WorldPack);
                GetPack(Constants.ShinyPack);
                GetPack(Constants.BlazingPack);
            };
            
            goBackToCollectionButton.onClick.AddListener(() =>
            {
                StartCoroutine(CleanPack());
            });
            
            MenuManager.Instance.SetOptionsBackground(optionBackground);
            MenuManager.Instance.SetOptions(optionMenu, "Packs");
            LoadUserData();
            
            
        }
        

        private void GetPack(string packName)
        {
            if (GameController.Instance.Wallet.TryGetValue(packName, out var value) && value > 0)
            {
                var standardPack = Instantiate(packItemPrefab, viewsParent);
                standardPack.Initialize(new RewardModel(packName, value), 
                    (packName,world)=>
                    {
                        packParent.SetActive(true);
                        deckPackInstance = Instantiate(deckPack, packParent.transform);
                        deckPackInstance.gameObject.SetActive(true);
                        deckPackInstance.CreateNewSet(packName, world);
                    });
            }
        }
        
        private void DeleteAllViews()
        {
            foreach (Transform child in viewsParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private IEnumerator CleanPack()
        {
            MenuManager.Instance.GoToState(Constants.HomeState, true);
            yield return new WaitForSeconds(0.5f);
            while (unpackBackground.alpha > 0)
            {
                unpackBackground.alpha -= 0.2f;
                yield return new WaitForEndOfFrame();
            }
            unpackBackground.blocksRaycasts = false;
            foreach (Transform child in setParent)
            {
                Destroy(child.gameObject);
            }
            goBackToCollectionGameObject.SetActive(false);
            setParent.gameObject.SetActive(false); 
            unpackBackground.gameObject.SetActive(false);
        }

        private async void LoadUserData()
        {
            try
            {
                await GameController.Instance.RefreshAssets();
                var optionMenu = new Dictionary<string, Action>();

                optionMenu["Packs"] = ()=>
                {
                    DeleteAllViews();
                    GetPack(Constants.StandardPack);
                    GetPack(Constants.WorldPack);
                    GetPack(Constants.ShinyPack);
                    GetPack(Constants.BlazingPack);
                };
            
                goBackToCollectionButton.onClick.AddListener(() =>
                {
                    StartCoroutine(CleanPack());
                });
            
                MenuManager.Instance.SetOptionsBackground(optionBackground);
                MenuManager.Instance.SetOptions(optionMenu, "Packs");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }        
        }

        public override void Exit(AState to)
        {
            gameObject.SetActive(false);
            if(deckPackInstance!=null)
                Destroy(deckPackInstance.gameObject);
            packParent.SetActive(false);
        }

        public override string GetName() => Constants.RewardsState;
    }
}
