using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public static CoroutineHelper Instance { get; private set; }
    //private Queue<IEnumerator> coroutines = new Queue<IEnumerator>();
    //private bool currentCoroutinePlaying = false;

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

    // private void Update()
    // {
    //     if (currentCoroutinePlaying == false && coroutines.Count > 0)
    //     {
    //         StartCoroutine(coroutines.Dequeue());
    //         Debug.Log("coroutineplayed");
    //         currentCoroutinePlaying = true;
    //     }
    // }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public Coroutine StartCoroutineHelper(IEnumerator corutine)
    {
        return StartCoroutine(corutine);
    }

    // public void EnqueueCoroutine(IEnumerator coroutine)
    // {
    //     IEnumerator ConfirmCoroutine()
    //     {
    //         yield return coroutine;
    //         Debug.Log("coroutinestopped");
    //         currentCoroutinePlaying = false;
    //     }
    //     coroutines.Enqueue(ConfirmCoroutine());
    // }

}
