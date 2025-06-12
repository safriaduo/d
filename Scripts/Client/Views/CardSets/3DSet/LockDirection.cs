using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDirection : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    }
}
