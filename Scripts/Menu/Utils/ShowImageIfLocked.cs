using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowImageIfLocked : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject image;


    private void OnEnable()
    {
        image.SetActive(!button.interactable);
    }
}
