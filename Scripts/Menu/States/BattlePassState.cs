using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class BattlePassState : AState
    {
        [SerializeField] private BattlePassView battlePassView;
        [SerializeField] private Button buyPremiumButton;
        [SerializeField] private ConfirmPopup goToStoreConfirmPopup;
        [SerializeField] private PopupOneButton popupOneButton;


        public override void Enter(AState from)
        {
            gameObject.SetActive(true);

            var battlePass = BattlePassAPI.BattlePass;
            
            battlePassView.gameObject.SetActive(true);
            battlePassView.Initialize(battlePass);
            foreach (var levelView in battlePassView.GetLevelViews())
            {
                levelView.SetPremiumButton(OnBuyPremiumButtonPressed);
            }
        }

        private void Start()
        {
            buyPremiumButton.onClick.AddListener(OnBuyPremiumButtonPressed);
        }

        private void OnBuyPremiumButtonPressed()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            PurchaseInAppBattlePass(Constants.BattlePassAndroid);
#elif UNITY_IOS
            PurchaseInAppBattlePass(Constants.BattlePassiOS);
#elif UNITY_WEBGL
                goToStoreConfirmPopup.SetConfirmPopup(()=>Application.OpenURL(Constants.WEB_STORE_URL),true, Constants.WEB_STORE_BODY, Constants.WEB_STOORE_TITLE);
                goToStoreConfirmPopup.Open();
#endif
        }

        private void PurchaseInAppBattlePass(string battlepassId)
        {
            LoadingPopup.Instance.Open();
            InAppPurchasingAPI.Instance.PurchaseItem(battlepassId, ()=>
            {
                LoadingPopup.Instance.Close();
                popupOneButton.SetUpPopupButton("You bought the Premium Battle Pass!", "Cool!", popupOneButton.Close, "Premium Battle Pass");
                popupOneButton.Open();
            }, ()=>
            {
                LoadingPopup.Instance.Close();
                ErrorPopup.Instance.Open();
                ErrorPopup.Instance.SetErrorText("Something went wrong, please try again later.\nPurchase failed");
            });
        }

        public override void Exit(AState to)
        {
            gameObject.SetActive(false);
        }

        public override string GetName() => Constants.BattlePassState;
    }
}