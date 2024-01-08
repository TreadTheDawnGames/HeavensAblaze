using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class IdleCamera : MonoBehaviour
{
    NetworkUIV2 netUI;

    public void SetEnabled(bool setTo)
    {
        gameObject?.SetActive(setTo) ;
    }

    private void Awake()
    {
        netUI = FindObjectOfType<NetworkUIV2>();
    }

    private void OnEnable()
    {
        if(netUI.mainMenu!=null)
            netUI.SetNetUIVisability(true);
    }

    void FixedUpdate()
    {

        transform.Rotate(Vector3.up, 5f * Time.fixedDeltaTime);

    }
}
