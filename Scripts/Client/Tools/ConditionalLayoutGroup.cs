using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionalLayoutGroup : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;

    [SerializeField] private int childThreshold = 5;
    [SerializeField] private int lowerChildSpacing = -2;
    [SerializeField] private int upperChildSpacing = -3;
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > childThreshold)
        {
            layoutGroup.spacing = upperChildSpacing;
        }
        else
        {
            layoutGroup.spacing = lowerChildSpacing;
        }
    }
}
