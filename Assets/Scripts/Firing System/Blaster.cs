using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//using static UnityEditor.Timeline.TimelinePlaybackControls;
using FishNet.Connection;
using FishNet.Object;


public class Blaster : NetworkBehaviour
{
    public GameObject laser;
    public AimPoint aimPoint;
    Quaternion rotation;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<Blaster>().enabled = false;
        }
    }

    private void Awake()
    {
        aimPoint = transform.root.GetComponentInChildren<AimPoint>();

    
    }

    public GameObject Fire(Transform blaster, Transform aimPoint)
    {
        //find in direction of aimpoint
        Vector3 relativePos = aimPoint.transform.position - blaster.transform.position;
        //find rotation relative to aimpoint
        Quaternion rotationQ = Quaternion.LookRotation(relativePos,new Vector3(0,1,0));
        //rotate relative rotation 90 degrees
        rotation = rotationQ*Quaternion.Euler(90,0,0);

        

        return Instantiate(laser, blaster.transform.position, rotation);
    }








}
