using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : MonoBehaviour
{
    private MMFeedbacks mmFeedbaks;

    private void Awake()
    {
        mmFeedbaks = GetComponent<MMFeedbacks>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    mmFeedbaks.PlayFeedbacks();
        //}
    }
}
