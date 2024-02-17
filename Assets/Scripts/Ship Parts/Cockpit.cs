using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing;
using Unity.VisualScripting;


public class Cockpit : ShipPart
{
    [SerializeField]
    public PredictionMotor root;

    public Camera cam;

    [SerializeField]
    MainMenu menu;

    public bool destroyCockpit = false;
    //Need to activate camera only when my main body is destroyed

    [SerializeField]
    MainBodyDebrisMaker debrisMaker;

    bool cockpitDestroyed = false;

    public override void OnShipCreated(PredictionMotor ship)
    {
        print("Cockpit OnShipCreated");

        root = ship;
        menu = root.mainMenu;

        //        cam = GetCamInChildren(transform);
        //cam = GetComponentInChildren<Camera>();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (GetComponentInChildren<Camera>())
        {
            //if(GetComponentInChildren<Camera>()!=null)
            transform.GetComponentInChildren<Camera>().enabled = false;
        }
    }

    //[ServerRpc(RequireOwnership =false)]
    public override void DestroyIfDead(bool disregardHP = false)
    {
        //ChangeCounterpartColor(damageHudCounterpart, this);
        if (hitPoints <= 0 || disregardHP)
        {
            if (!hasRun)
                CockpitDestroyIfDeadObservers();
        }
    }

    [ObserversRpc]
    public void CockpitDestroyIfDeadObservers()
    {
        hasRun = true;
        //problem is CockpitDestroyIfDeadObservers method (this one) is being called multiple times. I need it to not be.

        print("destroy if dead for " + gameObject.name + "is started");

        Instantiate(destructionExplosion, transform.position, transform.rotation);

        //find camera and do the transition to look at the body
        print(IsOwner);
        if (IsOwner)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                print(i);
                if (transform.GetChild(i).TryGetComponent<CameraDampener>(out CameraDampener camDamp) && transform.root != this.transform)
                {
                    Debug.Log("Cockpit parent: " + transform.parent.name);
                    camDamp.transform.SetParent(transform.parent);
                    camDamp.cockpitDied = true;
                    camDamp.Transition();
                    i--;
                }

                print(transform.GetChild(i).name);
                Destroy(transform.GetChild(i).gameObject);
            }
            FindObjectOfType<RespawnManager>().SetShowRespawn(true);
            //spawns with cockpit
            transform.parent.GetComponent<NetworkBehaviour>().RemoveOwnership();


        }
        if (root != null)
        {
            print("Root = " + root.name);
            //disable player input
            root.inputType = PredictionMotor.InputType.Disabled;
            
        }
        else
        {
            print("Root is null");
            //if(IsOwner)
            if (GetComponentInChildren<CameraDampener>() != null)
            {

                FindObjectOfType<IdleCamera>(true)?.gameObject.SetActive(true);

            }
        }
        if (!cockpitDestroyed)
        {
            cockpitDestroyed = true;

            debrisMaker?.SpawnMainBodyDebris();
        }
        if(debrisMaker == null) 
        {
            Destroy(gameObject);
        }



    }


}
