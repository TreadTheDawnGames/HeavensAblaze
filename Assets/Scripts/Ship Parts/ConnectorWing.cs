using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;


public class ConnectorWing : ShipPart
{

    public Transform alternateSupport;
    public bool hasChild = true;
    [SerializeField]
    GameObject alternateSupportHudCounterpart;

     //[ServerRpc(RequireOwnership = false)]
    public override void DestroyIfDead(bool disregardHP = false)
    {
       // ChangeCounterpartColor(damageHudCounterpart, this);
        if (hitPoints <= 0f || disregardHP)
        {
            if(!hasRun)
                ConnectorWingDestroyIfDeadObservers();
        }
    }

    //[ObserversRpc]
    public void ConnectorWingDestroyIfDeadObservers()
    {
        if (hitPoints <= 0)
        {
            Instantiate(destructionExplosion, transform.position, transform.rotation);
            if (explosion.Count != 0)
            {
                explosion[Random.Range(0, explosion.Count)].Play();
            }

            if (alternateSupport != null)
            {
                /*foreach(Transform child in damageHudCounterpart.transform)
                {
                    child.SetParent(alternateSupportHudCounterpart.transform);
                }
                RecoverLostHudItems(alternateSupportHudCounterpart);*/

                for (int i = 0; i < transform.childCount; i++)
                {
                    // if (transform.GetChild(i).name.Contains("Collider")) { break; }
                    transform.GetChild(i).SetParent(alternateSupport);
                    //     target.GetComponent<NetworkObject>().RemoveOwnership();
                }
                if (GetComponent<NetworkObject>().ParentNetworkObject != null)
                {
                    GetComponent<NetworkObject>().UnsetParent();

                }
                Destroy(gameObject);
            }
            else
            {
                base.DestroyIfDeadObservers();
            }

            //target.transform.DetachChildren();
            //transform.parent = parent;


            // DestroyIfDeadObservers(target);
            Destroy(gameObject);


        }

    }

    void RecoverLostHudItems(GameObject alternateSupportWithChildren)
    {
        if (alternateSupportWithChildren == null)
            return;

       // alternateSupportWithChildren.GetComponent<MeshRenderer>().material = regularMaterial;
       // alternateSupportWithChildren.GetComponent<MeshRenderer>().material.SetColor("_MainColor", GetDamageColor(alternateSupport.GetComponent<ShipPart>()));
        for (int i = 0; i < alternateSupportWithChildren.transform.childCount; i++)
        {
            RecoverLostHudItems(alternateSupportWithChildren.transform.GetChild(i).gameObject);
        }
    }
}


