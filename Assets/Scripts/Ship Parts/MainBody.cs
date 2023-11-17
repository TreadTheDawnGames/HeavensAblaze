using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBody : ShipPart
{
    [SerializeField]
    private PredictionMotor root;

    CameraDampener cam;

    private void Awake()
    {
        root = transform.root.GetComponent<PredictionMotor>();
        cam = GetCamInChildren(root.transform);
    }


    //[ServerRpc(RequireOwnership = false)]
    public override void DestroyIfDead()
    {
        //ChangeCounterpartColor(damageHudCounterpart, this);

        if (hitPoints <= 0f)
        {
            if (!hasRun)
                MainBodyDestroyIfDeadObservers();
        }
    }

    public void MainBodyDestroyIfDeadObservers()
    {
        root.inputType = PredictionMotor.InputType.Disabled;

        base.DestroyIfDeadObservers();
        root.gameObject.SetActive(false);
    }


    CameraDampener GetCamInChildren(Transform parent)
    {

        if (parent.TryGetComponent<CameraDampener>(out CameraDampener cam))
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
    Cockpit GetCockpitInChildren(Transform parent)
    {

        if (parent.TryGetComponent<Cockpit>(out Cockpit cam))
            return cam;
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (GetCockpitInChildren(parent.GetChild(i)) != null)
                {
                    Debug.Log("found " + parent.GetChild(i));
                    
                    return GetCockpitInChildren(parent.GetChild(i));
                }
            }
            return null;
            //return cam;
        }
    }

}
