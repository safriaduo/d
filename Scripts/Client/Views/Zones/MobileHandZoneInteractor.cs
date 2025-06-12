using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Views;
using MoreMountains.Tools;
using UnityEngine;

public class MobileHandZoneInteractor : MonoBehaviour
{
    [SerializeField] private HandZoneView handZoneView;
    [SerializeField] private GameObject handZoneMobile;
    [SerializeField] private LayerMask DontHideLayers;
    private bool isMobileHandShown = false;

    private void Start()
    {
        #if !(UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS)
        handZoneMobile.SetActive(false);
        #endif
    }

    private void Update()
    {
        //handZoneView.SpawnMobileCardDummies();
        if (Input.GetMouseButtonDown(0) && isMobileHandShown)
        {
            if (!RaycastHitOnGivenLayer(DontHideLayers))
            {
                handZoneView.HideMobileHand();
                isMobileHandShown = false;
            }
        }
        if (isMobileHandShown)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Card");
        }
    }
    
    public bool RaycastHitOnGivenLayer(LayerMask layer)
    {
        if (UIManager.CurrentStateUI != UIManager.StateUI.None)
            return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out _, Mathf.Infinity, layer);
    }

    private void OnMouseDown()
    {
        if (!isMobileHandShown)
        {
            handZoneView.ShowMobileHand();
            isMobileHandShown = true;
        }
    }
}
