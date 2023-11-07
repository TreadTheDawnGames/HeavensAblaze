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
        cam = GetCamInChildren(transform);
    }

    public override void OnStopClient()
    {
        if (cam != null)
        {
            base.OnStopClient();
            if(GetComponentInChildren<Camera>()!=null)
                transform.GetComponentInChildren<Camera>().gameObject.SetActive(false); ;
        }
    }
    /*public override void OnStartClient()
    {
            base.OnStopClient();  
        if (cam != null)
        {
            if (root!=null && root.GetComponent<NetworkObject>().IsOwner)
            {

                cam.gameObject.SetActive(true);
            }
        }
    }*/

    [ServerRpc(RequireOwnership =false)]
    public override void DestroyIfDead()
    {
        if (!hasRun)
        CockpitDestroyIfDeadObservers(); 
    }

    [ObserversRpc]
    public void CockpitDestroyIfDeadObservers()
    {
        if (hitPoints <= 0)
        {
            Instantiate(destructionExplosion, transform.position, transform.rotation);

            for (int i = 0; i < transform.childCount; i++)
            {

                if (transform.GetChild(i).TryGetComponent<CameraDampener>(out CameraDampener camDamp))
                {
                    camDamp.transform.SetParent(transform.parent);
                    camDamp.Transition();
                    i--;
                }
                Destroy(transform.GetChild(i).gameObject);

            }
            if(root!=null)
            {

                root.inputType = PredictionMotor.InputType.Disabled;

            /*if(root.mainCam.activeInHierarchy)
                root.mainCam.GetComponent<CameraDampener>().Transition();*/
            

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
