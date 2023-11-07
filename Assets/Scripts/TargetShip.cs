using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//using static UnityEditor.Timeline.TimelinePlaybackControls;


public class TargetShip : MonoBehaviour
{


    public List<Collider> colliders = new List<Collider>();
    Transform shipBody;
    public ShipPart shipPart;



    private void Awake()
    {
        shipBody = transform.root;
        colliders = GetComponentsInChildren<Collider>().ToList();

        for (int i = 0; i < transform.root.childCount; i++)
        {
            shipBody.GetChild(i).AddComponent<ShipPart>();
            shipBody.GetChild(i).tag = "ship body";
            // shipPart.transforms.Add(shipBody.GetChild(i));
        }

        foreach (Collider collider in colliders)
        {

            //collider.AddComponent<ShipPart>();
            shipPart = collider.GetComponent<ShipPart>();

           
        }



    }
}



    