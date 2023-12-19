using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing;


public class Cockpit : ShipPart
{
    [SerializeField]
    public PredictionMotor root;

    public Camera cam;

    //Need to activate camera only when my main body is destroyed

    private void Awake()
    {
        root = transform.root.GetComponent<PredictionMotor>();
//        cam = GetCamInChildren(transform);
        //cam = GetComponentInChildren<Camera>();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (GetComponentInChildren<Camera>())
        {
            //if(GetComponentInChildren<Camera>()!=null)
                transform.GetComponentInChildren<Camera>().gameObject.SetActive(false); ;
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public override void DestroyIfDead()
    {
        //ChangeCounterpartColor(damageHudCounterpart, this);

        if (!hasRun)
        CockpitDestroyIfDeadObservers(); 
    }

    [ObserversRpc]
    public void CockpitDestroyIfDeadObservers()
    {
        if (hitPoints <= 0)
        {
            Instantiate(destructionExplosion, transform.position, transform.rotation);

            //find camera and do the transition to look at the body
            for (int i = 0; i < transform.childCount; i++)
            {
                if (IsOwner)
                {

                    if (transform.GetChild(i).TryGetComponent<CameraDampener>(out CameraDampener camDamp) && transform.root != this.transform)
                    {
                        camDamp.transform.SetParent(transform.parent);
                        camDamp.cockpitDied = true;
                        camDamp.Transition();
                        i--;
                    }
                    /*else
                    {
                        FindObjectOfType<IdleCamera>(true).SetEnabled(true);
                    }*/
                }
                Destroy(transform.GetChild(i).gameObject);

            }
            if (root != null)
            {
                //disable player input
                root.inputType = PredictionMotor.InputType.Disabled;

            }
            else
            {
                FindObjectOfType<IdleCamera>(true).SetEnabled(true);
            }

            Destroy(gameObject);
        }

    }
    Camera GetCamInChildren(Transform parent)
    {

        if (parent.TryGetComponent<Camera>(out Camera cam))
            return cam;
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (GetCamInChildren(parent.GetChild(i)) != null)
                {
                    Debug.Log("found " + parent.GetChild(i));
                    return GetCamInChildren(parent.GetChild(i));
                }
            }
            return null;
            //return cam;
        }
    }

}
