using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomExplanationView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    public void setTitle(string title)
    {
        titleText.text = title;
    }

    public void setBody(string body)
    {
        bodyText.text = body;
    }
}
