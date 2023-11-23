using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
using FishNet.Component.Prediction;
using UnityEngine.InputSystem;
using System.ComponentModel;
using FishNet.Transporting;

public class BlasterV3 : NetworkBehaviour
{
    AimPoint aimPoint;
    public PredictionMotor myShip;
    public bool isUsingAimpoint;

    //[Range(0.0f, 1.0f)]
    public float velocityDivider = 0.15f;

    Color laserColor;

    public Vector3 originalPos;

    [SerializeField]
    AudioSource audioSource;

    public void Setup()
    {
        if(audioSource == null) audioSource = GetComponent<AudioSource>();

        isUsingAimpoint = PlayerPrefs.GetInt("useAimpoint", 1) == 1 ? true : false;

        if (!base.IsOwner)
        {
            gameObject.GetComponent<BlasterV3>().enabled = false;
        }
        if (FindObjectOfType<AimPoint>().transform.root == transform.root)
        {
            aimPoint = transform.root.GetComponentInChildren<AimPoint>();

        }

        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Projectile to spawn.
    /// </summary>
    [Tooltip("Projectile to spawn.")]
    [SerializeField]
    private LaserV3 _projectile;
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
    public void ClientFire(Rigidbody rb)
    {
;       //y = m * x + b;
;
        float extraSpaceZ = Mathf.Clamp(velocityDivider * transform.InverseTransformDirection(rb.velocity).z, 0f, 10f);

        transform.localPosition = new Vector3(originalPos.x, originalPos.y, originalPos.z + extraSpaceZ);


        //Vector3 predictedPoint = transform.position + rb.velocity * Time.fixedDeltaTime;

//        Debug.Log(transform.position + " / " + predictedPoint);

        //need to make lasers not destroy firing ship lasers
        //maybe try moving spawn point using animation curve

        Vector3 position = transform.position;
        Vector3 direction = isUsingAimpoint ? (aimPoint.transform.position - transform.position).normalized : transform.forward;

        /* Spawn locally with 0f passed time.
         * Since this is the firing client
         * they do not need to accelerate/catch up
         * the projectile. */
        if (IsClientOnly)
        {
            SpawnProjectile(position, direction, 0f, LocalConnection);

        }
        //Ask server to also fire passing in current Tick.
        ServerFire(position, direction, base.TimeManager.Tick, LocalConnection);
    }

    /// <summary>
    /// Spawns a projectile locally.
    /// </summary>
    
    private void SpawnProjectile(Vector3 position, Vector3 direction, float passedTime, NetworkConnection connection)
    {

        audioSource.pitch = Random.Range(0.75f, 1f);
        audioSource.Play();

        laserColor = myShip.syncedLaserColor;

        Debug.Log(passedTime);

        

        position += direction.normalized * 30f * passedTime ;

        Quaternion rotationQ;
        if (isUsingAimpoint)
        {
            //find in direction of aimpoint
            Vector3 relativePos = aimPoint.transform.position - transform.position;
            //find rotation relative to aimpoint and rotate 90 deg
            rotationQ = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0)) ;
        }
        else
        {
            rotationQ = transform.rotation;
        }

        LaserV3 pp = Instantiate(_projectile, position, rotationQ);

        Debug.DrawLine(position, pp.transform.TransformPoint(pp.transform.forward), Color.magenta, 5f, false) ;

        pp.GetComponent<NetworkObject>().SetLocalOwnership(LocalConnection);

        pp.Initialize(direction, passedTime, laserColor, connection);
        

    }


    /// <summary>
    /// Fires on the server.
    /// </summary>
    /// <param name="position">Position to spawn projectile.</param>
    /// <param name="direction">Direction to move projectile.</param>
    /// <param name="tick">Tick when projectile was spawned on client.</param>
    [ServerRpc]
    private void ServerFire(Vector3 position, Vector3 direction, uint tick, NetworkConnection connection)
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
        SpawnProjectile(position, direction, passedTime, connection);
        //Tell other clients to spawn the projectile.
        ObserversFire(position, direction, tick, connection);
    }


    /// <summary>
    /// Fires on all clients but owner.
    /// </summary>
    [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
    private void ObserversFire(Vector3 position, Vector3 direction, uint tick, NetworkConnection connection)
    {
        //Like on server get the time passed and cap it. Note the false for allow negative values.
        float passedTime = (float)base.TimeManager.TimePassed(tick, false);
        passedTime = Mathf.Min(MAX_PASSED_TIME, passedTime);

        //Spawn the projectile locally.
        SpawnProjectile(position, direction, passedTime, connection);
    }

    



}
