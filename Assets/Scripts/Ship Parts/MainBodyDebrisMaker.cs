using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBodyDebrisMaker : ShipPart
{
    
    public void SpawnMainBodyDebris()
    {
        hasRun = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (IsServer)
            {
                InstantiateDebris(transform.GetChild(i).gameObject);
            }
        }

        gameObject.SetActive(false);
        //Destroy(gameObject);
        //Despawn(gameObject);
    }

    [ObserversRpc]
    private void InstantiateDebris(GameObject originalObject)
    {
        if (IsServer)
        {

            GameObject debrisChild = null;
            if (originalObject.TryGetComponent<ShipPart>(out ShipPart part))
            {
                if (part is not Cockpit)
                {
                    debrisChild = part.debris;
                }
            }

            if (debrisChild != null && debrisChild.GetComponent<ShipPart>() != null)
            {

                if (debrisChild.GetComponent<Rigidbody>() == null)
                {
                    debrisChild.AddComponent<Rigidbody>();
                }



                GameObject instance = Instantiate(debrisChild, originalObject.transform.position, originalObject.transform.rotation);
                GameObject spawn = instance;

                Vector3 rotationRandomizer = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                spawn.GetComponent<Rigidbody>().AddForce(originalObject.transform.root.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                spawn.GetComponent<Rigidbody>().AddTorque(originalObject.transform.root.GetComponent<Rigidbody>().angularVelocity + rotationRandomizer, ForceMode.VelocityChange);

                spawn.gameObject.GetComponent<ShipPart>().hitPoints = originalObject.GetComponent<ShipPart>().hitPoints;
                spawn.gameObject.GetComponent<Rigidbody>().useGravity = false;
                spawn.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                if (spawn.transform.childCount <= 0)
                {
                    for (int i = 0; i < originalObject.transform.childCount; i++)
                    {
                        //Debug.Log(GO.transform.GetChild(i).name+" found");
                        InstantiateChildDebris(originalObject.transform.GetChild(i).gameObject, spawn.transform);
                        // GO.transform.GetChild(i).SetParent(spawn.transform);
                    }


                }

                //need this to run regardless of server or client

                ServerManager.Spawn(spawn);
                SpawnChildren(spawn);
                if (IsOwner)
                {
                    FindObjectOfType<IdleCamera>(true)?.gameObject.SetActive(true);
                }


            }


        }
        

    }

    private void SpawnChildren(GameObject spawn)
    {
        if (IsServer)

            for (int i = 0; i < spawn.transform.childCount; i++)
            {


                GameObject child = spawn.transform.GetChild(i).gameObject;

                if (child.GetComponent<NetworkObject>() != null)
                {


                    ServerManager.Spawn(child);
                    SpawnChildren(child);


                }

            }
    }

    private void InstantiateChildDebris(GameObject GO, Transform parent)
    {
        if (IsServer)
        {



            GameObject debrisChild = null;
            if (GO.TryGetComponent<ShipPart>(out ShipPart part))
            {
                if(part is not Cockpit)
                {
                    debrisChild = part.debris;
                }
            }

            if (debrisChild == null)
            {
                return;
            }

            GameObject spawn = Instantiate(debrisChild, GO.transform.position, GO.transform.rotation, parent);

            if (spawn.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Destroy(rb);
            }

            spawn.gameObject.GetComponent<ShipPart>().hitPoints = GO.GetComponent<ShipPart>().hitPoints;

            //spawn.transform.SetParent(parent);

            for (int i = 0; i < GO.transform.childCount; i++)
            {
                //Debug.Log(GO.transform.GetChild(i).name+" found");
                InstantiateChildDebris(GO.transform.GetChild(i).gameObject, spawn.transform);
            }
            //Spawn(spawn);


        }

    }
}
