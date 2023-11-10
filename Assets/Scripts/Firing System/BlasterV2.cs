using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using FishNet.Component.Prediction;
using UnityEngine.InputSystem;


public class BlasterV2 : NetworkBehaviour
{
    //private PlayerShip playerShip;
    private AimPoint aimPoint;

    public int numFramesBetweenShots = 0;
    public int framesBetweenShots = 10;
    private Transform originalRoot;

    public bool isUsingAimpoint = true;

    public AudioSource laserPew;

    public Color laserColor;

    [SerializeField]
    public PredictionMotor myShip;

    Vector3 originalPos;
    /*public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<BlasterV2>().enabled = false;
        }
        if(FindObjectOfType<AimPoint>().transform.root == transform.root)
        {
            aimPoint = transform.root.GetComponentInChildren<AimPoint>();

        }
        originalRoot = transform.root;

    }*/

    public void Setup()
    {
         
        if (!base.IsOwner)
        {
            gameObject.GetComponent<BlasterV2>().enabled = false;
        }
        if (FindObjectOfType<AimPoint>().transform.root == transform.root)
        {
            aimPoint = transform.root.GetComponentInChildren<AimPoint>();

        }
        
        originalRoot = transform.root;
    }

    private void Awake()
    {
        originalPos = transform.localPosition;
        //playerShip = new PlayerShip();
        //playerShip.Joystick.Enable();
    }

    /* private void FixedUpdate()
     {
         if (IsClient)
         {

             numFramesBetweenShots++;
             numFramesBetweenShots = (int)Mathf.Clamp(numFramesBetweenShots, 0f, framesBetweenShots);
             if (playerShip.Joystick.Fire.IsInProgress() && numFramesBetweenShots >= framesBetweenShots && transform.root == originalRoot)
             {
                 ClientFire(aimPoint, gameObject);
                 numFramesBetweenShots = 0;
             }
         }



     }*/

   

    public void DoClientFire()
    {
        if (IsClient)
        {
            if (transform.root == originalRoot)
            {
                //laserPew.Play();
                
                ClientFire(aimPoint);
            }
        }
    }

    /// <summary>
    /// Projectile to spawn.
    /// </summary>
    [Tooltip("Projectile to spawn.")]
    [SerializeField]
    private LaserV2 _projectile;
    /// <summary>
    /// Maximum amount of passed time a projectile may have.
    /// This ensures really laggy players won't be able to disrupt
    /// other players by having the projectile speed up beyond
    /// reason on their screens.
    /// </summary>
    private const float MAX_PASSED_TIME = 0.3f;

    /// <summary>
    /// Local client fires weapon.
    /// </summary>
    private void ClientFire(AimPoint aimPoint)
    {

        



        Vector3 instantiationPosition = transform.localPosition;
        Vector3 direction;
        if (isUsingAimpoint)
        {
            direction = aimPoint.transform.position - transform.position;
        }
        else
        {
            direction = transform.up - transform.position;
        }


        /* Spawn locally with 0f passed time.
         * Since this is the firing client
         * they do not need to accelerate/catch up
         * the projectile. */

        if (IsClient)
        {
            SpawnProjectile(instantiationPosition, direction, 0f);
        }
        
        //Ask server to also fire passing in current Tick.
        ServerFire(instantiationPosition, direction, base.TimeManager.Tick);
    }

    /// <summary>
    /// Spawns a projectile locally.
    /// </summary>
     
    

    private void SpawnProjectile(Vector3 instantiationPosition, Vector3 direction, float passedTime)
    {
        laserColor = myShip.syncedLaserColor;

        Quaternion rotationQ;
        if (isUsingAimpoint)
        {
            //find in direction of aimpoint
            Vector3 relativePos = aimPoint.transform.position - transform.position;
            //find rotation relative to aimpoint and rotate 90 deg
            rotationQ = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0)) * Quaternion.Euler(90, 0, 0); ;
        }
        else
        {
            rotationQ = transform.rotation ; 
        }



        transform.localPosition = new Vector3(originalPos.x, originalPos.y, originalPos.z);

        LaserV2 pp = Instantiate(_projectile, transform.position, rotationQ);
       
            pp.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", laserColor * 80);

        
        pp.GetComponent<NetworkObject>().SetParent(transform.GetComponent<NetworkObject>());
        //pp.Initialize(direction, passedTime);
        //pp.GetComponent<NetworkObject>().SetLocalOwnership(GetComponent<NetworkObject>().LocalConnection);

        

        Vector3 straight = new Vector3(0f, 70f, 0f);

        pp.GetComponent<Rigidbody>().AddRelativeForce(straight, ForceMode.VelocityChange);
        


    }

    /// <summary>
    /// Fires on the server.
    /// </summary>
    /// <param name="position">Position to spawn projectile.</param>
    /// <param name="direction">Direction to move projectile.</param>
    /// <param name="tick">Tick when projectile was spawned on client.</param>
    [ServerRpc(RequireOwnership =false)]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick)
    {
        /* You may want to validate position and direction here.
         * How this is done depends largely upon your game so it
         * won't be covered in this guide. */

        //Get passed time. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        /* Cap passed time at half of constant value for the server.
         * In our example max passed time is 300ms, so server value
         * would be max 150ms. This means if it took a client longer
         * than 150ms to send the rpc to the server, the time would
         * be capped to 150ms. This might sound restrictive, but that would
         * mean the client would have roughly a 300ms ping; we do not want
         * to punish other players because a laggy client is firing. */
        passedTime = Mathf.Min(MAX_PASSED_TIME / 2f, passedTime);

        //Spawn on the server.
        SpawnProjectile(position, direction, passedTime);
        //Tell other clients to spawn the projectile.
        ObserversFire(position, direction, tick);
    }

    /// <summary>
    /// Fires on all clients but owner.
    /// </summary>
    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversFire(Vector3 position, Vector3 direction, uint tick)
    {
        //Like on server get the time passed and cap it. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        //Spawn the projectile locally.
        SpawnProjectile(position, direction, passedTime);
    }


    /*public override void DestroyIfDead()
    {
        BlastersDestroyIfDead();
    }

    [ObserversRpc]
    public void BlastersDestroyIfDead()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
            children.Add(transform.GetChild(i));

        foreach (Transform child in children)
        {
            Destroy(child);
        }
        Destroy(gameObject);
    }*/



}
