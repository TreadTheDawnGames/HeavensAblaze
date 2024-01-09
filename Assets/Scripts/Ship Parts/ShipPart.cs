
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Linq;
using FishNet.Component.Transforming;
using FishNet.Object.Prediction;
using FishNet.Observing;
using static PredictionMotor;



public class ShipPart : NetworkBehaviour
{
    [SyncVar(Channel = Channel.Unreliable, OnChange = nameof(on_health))]
    public float hitPoints = 50f;
    public float maxHitPoints = 50f;
    private void on_health(float prev, float next, bool asServer)
    {
        
            if (damageHudCounterpart != null)
            {
                damageHudCounterpart?.GetComponent<DamageHologram>()?.UpdateCounterpart(next);
            }
            if(asServer)
                DestroyIfDead();
        


    }
    
    //[SyncVar] public Transform parent;
    //[SyncVar] public NetworkObject netParent;

    public bool hasRun = false;

    public ParticleSystem destructionExplosion;

    public bool destroyObject = false;

    [SerializeField]
    public List<AudioSource> explosion = new List<AudioSource>();


    public NetworkConnection owner;
    public bool showOwner = false;

    public int childCount;

    private Transform originalRoot;

    [SerializeField]
    NetworkBehaviour _parent;

    [SerializeField]
    public ParticleSystem collisionImpact;

    public PredictionMotor ship;

#if UNITY_EDITOR
    private void FixedUpdate()
    {
        //ship = transform.root.GetComponent<PredictionMotor>();

        owner = GetComponent<NetworkObject>().Owner;

        if (showOwner)
            Debug.Log(owner.ToString());

        childCount = transform.childCount;


        if (destroyObject)
        {
            if (IsServer)
            {

                hitPoints = 0f;
                //DestroyIfDead();
            }
        }
    }
#endif
    
    private void OnDestroy()
    {

        if (TryGetComponent<AudioSource>(out AudioSource audio)) 
        {
            if (audio.isPlaying)
            {
                audio.Stop();
            }
        }

        if (damageHudCounterpart != null)
        {
            transform.root.GetComponentInChildren<DamageHologram>()?.UpdateCounterpart(hitPoints) ;
        }
    }

    public virtual void OnShipCreated(PredictionMotor ship) { }

   // [ServerRpc(RequireOwnership=false)]
    public virtual void DestroyIfDead()
    {
        //ChangeCounterpartColor(damageHudCounterpart, this);
        

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
                debrisChild = part.debris;
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



    private void OnCollisionEnter(Collision collision)
    {
            ShipPart childPart = collision.GetContact(0).thisCollider.GetComponent<ShipPart>();
           // Rigidbody otherPart = collision.GetContact(0).otherCollider.transform.root.GetComponentInChildren<Rigidbody>();

        if (IsClient && childPart != null)
        {
            Instantiate(childPart.collisionImpact, collision.GetContact(0).point, Quaternion.Euler(collision.GetContact(0).normal));
        }

        if (IsServer)
        {


            if (GetComponent<Rigidbody>().velocity.magnitude > 2)
            {
                    float damage = GetComponent<Rigidbody>().velocity.magnitude / 1.6f;
                /*if(otherPart!=null) 
                { 
                    damage *= Mathf.Clamp(otherPart.velocity.magnitude, 1f, 50f); 
                }*/
                childPart.hitPoints -= damage;

            }
            childPart.DestroyIfDead();
        }
        
    }

    [SerializeField]
    public GameObject damageHudCounterpart;

    

    

    

}
