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


     //[ServerRpc(RequireOwnership = false)]
    public override void DestroyIfDead()
    {
        if (hitPoints <= 0f)
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
}


