using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressToInvoke : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] UnityEvent OnClick;

    // Update is called once per frame
    private void OnMouseDown()
    {
        OnClick.Invoke();
    }
}
