using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGameObjectOnButton : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private GameObject gameObjectToToggle;
    void Start()
    {
        toggleButton.onClick.AddListener(()=>gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
