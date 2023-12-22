using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class IdleCamera : MonoBehaviour
{
    public void SetEnabled(bool setTo)
    {
        gameObject?.SetActive(setTo) ;
    }

    void FixedUpdate()
    {


        transform.Rotate(Vector3.up, 5f * Time.fixedDeltaTime);

    }
}
