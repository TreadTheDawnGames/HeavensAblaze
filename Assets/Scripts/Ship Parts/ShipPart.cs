
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using FishNet.Component.Transforming;
using FishNet.Object.Prediction;
using FishNet.Observing;



public class ShipPart : NetworkBehaviour
{
    [SyncVar]
    public float hitPoints = 50f;

    //[SyncVar] public Transform parent;
    //[SyncVar] public NetworkObject netParent;

    public bool hasRun = false;

    public List<Transform> transforms = new List<Transform>();
    public ParticleSystem destructionExplosion;

    public bool destroyObject = false;

    [SerializeField]
    public List<AudioSource> explosion = new List<AudioSource>();


    public NetworkConnection owner;
    public bool showOwner = false;

    public int childCount;

    private Transform originalRoot;

    public int partId;

    [SerializeField]
    NetworkBehaviour _parent;

#if UNITY_EDITOR
    private void FixedUpdate()
    {
        owner = GetComponent<NetworkObject>().Owner;

        if (showOwner)
            Debug.Log(owner.ToString());

        childCount = transform.childCount;


        if (destroyObject)
        {
            if (IsServer)
            {

                hitPoints = 0f;
                DestroyIfDead();
            }
        }
    }
#endif

    private void Awake()
    {
    }


    //[ServerRpc]
    public virtual void DestroyIfDead()
    {
        if (hitPoints <= 0f)
        {
            if (!hasRun)
                DestroyIfDeadObservers();
        }


    }


    public GameObject debris;


    //[ObserversRpc]
    public void DestroyIfDeadObservers()
    {
        hasRun = true;
        Instantiate(destructionExplosion, transform.position, transform.rotation);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (IsServer)
            {
                InstantiateDebris(transform.GetChild(i).gameObject);
            }
        }

        Destroy(gameObject);
        //Despawn(gameObject);
    }
    private void InstantiateDebris(GameObject originalObject)
    {
        if (IsServer)
        {

            GameObject debrisChild = null;
            if (originalObject.TryGetComponent<ShipPart>(out ShipPart part))
            {

                debrisChild = part.debris;
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
                TransferCamera(spawn);
            }
            

        }


    }

    [ObserversRpc(ExcludeServer =false,RunLocally =true)]
    public void TransferCamera(GameObject spawn)
    {
        if (spawn.TryGetComponent<Cockpit>(out Cockpit cockpit) && transform.root.GetComponentInChildren<CameraDampener>())
        {

            transform.root.GetComponentInChildren<CameraDampener>().transform.SetParent(cockpit.transform);


        }
    }

    public void SpawnChildren(GameObject spawn)
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

    public void InstantiateChildDebris(GameObject GO, Transform parent)
    {
        if (IsServer)
        {


            Debug.Log("***************");

            GameObject debrisChild = null;
            if (GO.TryGetComponent<ShipPart>(out ShipPart part))
            {
                debrisChild = part.debris;
            }

            if (debrisChild == null)
            {
                return;
            }

            Debug.Log("++++++++++ Instantiating: " + debrisChild.name + " / " + parent.name);
            GameObject spawn = Instantiate(debrisChild, GO.transform.position, GO.transform.rotation, parent);


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
