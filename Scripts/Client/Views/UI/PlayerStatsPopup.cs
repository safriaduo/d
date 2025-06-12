using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsPopup : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text debrisText;
    [SerializeField] private TMP_Text tethrasText;
    [SerializeField] private TMP_Text packsText;

    private void Start()
    {
        Hide();
    }

    public void Initialize(int debrisNumber=0, int tethrasNumber=0, int packsNumber=0, bool hasUserAvatar=false, Sprite avatar=null)
    {
        Show();
        if(debrisText!=null)
            debrisText.text = debrisNumber.ToString();
        if(tethrasText!=null)
            tethrasText.text = tethrasNumber.ToString();
        if(packsText!=null)
            packsText.text = packsNumber.ToString();
        if(!hasUserAvatar || avatar==null)
            return;
        avatarImage.sprite = avatar;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    
}
