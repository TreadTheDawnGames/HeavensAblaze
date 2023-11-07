using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Transforming;
using FishNet.Object.Prediction;
public class ShipDebrisModel : NetworkBehaviour
{
    public ShipPart pairedPart;

    public List<Transform> shipParts = new List<Transform>();


    private void Awake()
    {
        /*for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.AddComponent<ShipDebrisModel>();
        }*/


        GetChildRecursive(gameObject);
        int i = 1;
        foreach (Transform part in shipParts)
        {
            if (part.TryGetComponent<ShipPart>(out ShipPart part2))
            {
                part2.partId = i++;
            }
            
        }
        

    }
    private void GetChildRecursive(GameObject obj)
    {
        if (obj == null)
            return;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
                continue;
            //child.gameobject contains the current child you can do whatever you want like add it to an array
            if (child.TryGetComponent<ShipPart>(out ShipPart part))
                shipParts.Add(part.transform);
            GetChildRecursive(child.gameObject);
        }
    }

    ShipPart GetDebrisShipPart(List<Transform> parts, int targetID)
    {
        if(parts.Count < 1)
            Debug.LogWarning("Debris ship part list is empty");

        foreach (Transform partTransform in parts)
        {
            if (partTransform.TryGetComponent<ShipPart>(out ShipPart part))
            {
                if (part.partId == targetID)
                    return part;
            }
                
        }

        return null;
    }

    public bool hasDestroyed = false;
    [ServerRpc(RequireOwnership =false, RunLocally =true)]
    public void SpawnUndestroyedObjects(List<ShipPart> undestroyedChildren)
    {
        ///make manual spawning debris for each part individually.
        ///i.e. wing Instantiates wingDebris in onDestroy.


        //ObserversSpawnUndestroyedObjects(undestroyedChildren);

        /*if (!hasDestroyed)
        {

            //hasDestroyed = true;

            foreach (ShipPart child in undestroyedChildren)
            {
                int i;
                if (child != null)
                    i = child.partId;
                else i = -1;

                Debug.Log(i + child?.name);

                //if(IsServer)
                   // child.RemoveOwnership();

                if (i >= 0)
                {
                    //part.GetComponent<NetworkObject>().UnsetParent();

                    NetworkObject spawnMe = GetDebrisShipPart(shipParts, i)?.GetComponent<NetworkObject>();

                    if (spawnMe != null)
                    {

                        NetworkObject debris = Instantiate(spawnMe, child.transform.position, child.transform.rotation);

                        if (debris.TryGetComponent<ConnectorWing>(out ConnectorWing wing))
                        {
                            if (!child.GetComponent<ConnectorWing>().hasChild)
                            {
                                for (int j = 0; j < wing.childCount; j++)
                                {
                                    Destroy(wing.transform.GetChild(j).gameObject);
                                    //wing.transform.DetachChildren();
                                }
                            }
                        }

                        debris.gameObject.AddComponent<Rigidbody>();
                        debris.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        debris.gameObject.GetComponent<ShipPart>().hitPoints = child.hitPoints;

                        Vector3 rotationRandomizer = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));



                        //part.RemoveParentFromChildren(child);
                        //child.GetComponent<NetworkObject>().UnsetParent();
                        NetworkObject spawn = debris;
                        
                            spawn.GetComponent<Rigidbody>().AddForce(child.transform.root.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                            spawn.GetComponent<Rigidbody>().AddTorque(child.transform.root.GetComponent<Rigidbody>().angularVelocity + rotationRandomizer, ForceMode.VelocityChange);
                        
                        Spawn(spawn);
                        Debug.Log("Spawned " + spawn);
                    }
                }
            }
        }*/

        //-------------------------------------------------------------------

        /*foreach(Transform child in undestroyedChildren)
        {
            if (child == null || child.GetComponent<ShipPart>() == null)
            {
                Debug.Log("child was null");
                continue;
            }
           int i = child.GetComponent<ShipPart>().ObjectId;
        
            //ShipPart child = undestroyedChildren[i];

            Transform partTransform = shipParts[i];

            if (partTransform != null && partTransform.GetComponent<ShipPart>() != null)
            {
                ShipPart part = partTransform.GetComponent<ShipPart>();
                //part.GetComponent<NetworkObject>().UnsetParent();

                GameObject debris = Instantiate(partTransform.gameObject, child.transform.position, child.transform.rotation);

                debris.gameObject.AddComponent<Rigidbody>();
                debris.gameObject.GetComponent<Rigidbody>().useGravity = false;

                Vector3 rotationRandomizer = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));


                if (transform.root.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    debris.GetComponent<Rigidbody>().AddForce(rb.velocity, ForceMode.VelocityChange);
                    debris.GetComponent<Rigidbody>().AddTorque(rb.angularVelocity + rotationRandomizer, ForceMode.VelocityChange);
                }

                part.RemoveParentFromChildren(partTransform);
                partTransform.GetComponent<NetworkObject>().UnsetParent();

                Spawn(debris);
                Debug.Log("Spawned " + debris);

            }
            else
            {
                Debug.Log("part was null");
            }
        }*/

    }

    [ObserversRpc(RunLocally =true,ExcludeServer=true)]
    public void ObserversSpawnUndestroyedObjects(List<ShipPart> undestroyedChildren)
    {
        if (!hasDestroyed)
        {

            hasDestroyed = true;

            foreach (ShipPart child in undestroyedChildren)
            {
                int i;
                if (child != null)
                    i = child.partId;
                else i = -1;

                Debug.Log(i + child?.name);

                //if(IsServer)
                // child.RemoveOwnership();

                if (i >= 0)
                {
                    //part.GetComponent<NetworkObject>().UnsetParent();

                    Transform spawnMe = GetDebrisShipPart(shipParts, i)?.GetComponent<Transform>();

                    if (spawnMe != null)
                    {

                        Transform debris = Instantiate(spawnMe, child.transform.position, child.transform.rotation);

                        if (debris.TryGetComponent<ConnectorWing>(out ConnectorWing wing))
                        {
                            if (!child.GetComponent<ConnectorWing>().hasChild)
                            {
                                for (int j = 0; j < wing.childCount; j++)
                                {
                                    Destroy(wing.transform.GetChild(j).gameObject);
                                    //wing.transform.DetachChildren();
                                }
                            }
                        }

                        debris.gameObject.AddComponent<Rigidbody>();
                        debris.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        debris.gameObject.GetComponent<ShipPart>().hitPoints = child.hitPoints;

                        Vector3 rotationRandomizer = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));



                        //part.RemoveParentFromChildren(child);
                        //child.GetComponent<NetworkObject>().UnsetParent();
                        //Transform spawn = debris;


                        debris.AddComponent<NetworkObject>();
                        //if(IsServer)
                     //       Instantiate(spawn);
                        Spawn(debris.gameObject);
                        
                        debris.GetComponent<Rigidbody>().AddForce(child.transform.root.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                        debris.GetComponent<Rigidbody>().AddTorque(child.transform.root.GetComponent<Rigidbody>().angularVelocity + rotationRandomizer, ForceMode.VelocityChange);
                        
                        Debug.Log("Spawned " + debris);
                    }
                }
            }
        }
    }

    }
