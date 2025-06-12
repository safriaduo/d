using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkenBackground : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvasGroup;
    public static DarkenBackground Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Close();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Open()
    {
        canvasGroup.alpha = 0.75f;
        canvasGroup.blocksRaycasts = true;
    }


    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
