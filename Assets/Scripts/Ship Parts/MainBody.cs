using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainBody : ShipPart
{
    [SerializeField]
    private PredictionMotor root;

    [SerializeField]
    MainMenu menu;
    CameraDampener cam;

    private void Awake()
    {
        if (root == null)
        {
            root = transform.root.GetComponent<PredictionMotor>();
        }
    }
    public override void OnShipCreated(PredictionMotor ship)
    {
        FindObjectOfType<IdleCamera>(true)?.gameObject.SetActive(true);

        print("Body OnShipCreated");
        root = ship;

        menu = root.mainMenu;
        print("body menu = " + menu.name + "on awake");

        cam = GetComponentInChildren<CameraDampener>();
    }

   // [ServerRpc(RequireOwnership = false)]
    public override void DestroyIfDead()
    {
        //ChangeCounterpartColor(damageHudCounterpart, this);

        if (hitPoints <= 0f)
        {
            if (!hasRun)
                MainBodyDestroyIfDeadObservers();
        }
    }
    [ObserversRpc]
    void ChangeCamera()
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            if(cam.transform.parent.name != "Cockpit")
            {
                print("body menu = " + menu.name + "when destroyed");


                FindObjectOfType<IdleCamera>(true)?.gameObject.SetActive(true) ;

            }
        }
    }
    

    //[ObserversRpc(RunLocally =false)]
    public void MainBodyDestroyIfDeadObservers()
    {
        root.inputType = PredictionMotor.InputType.Disabled;

        root.gameObject.SetActive(false);
            FindObjectOfType<MainMenu>()?.SetShipPartDestroyed(this);
        ChangeCamera();
        base.DestroyIfDeadObservers();
    }

    void OnDestroy()
    {
        if (GetComponentInChildren<Camera>() != null)
        {
            root?.activeIdleCam?.SetEnabled(true);
        }
    }
    

   

}
