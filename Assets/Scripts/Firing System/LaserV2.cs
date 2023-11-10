using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;
using FishNet.Component.Prediction;

public class LaserV2 : NetworkBehaviour
{


    public float damage = 12f;

    private bool hasHit = false;

    [SerializeField] private ParticleSystem hitSparks;
    [SerializeField] private Transform raycastPoint;

    /// <summary>
    /// Initializes this projectile.
    /// </summary>
    /// <param name="direction">Direction to travel.</param>
    /// <param name="passedTime">How far in time this projectile is behind te prediction.</param>


    private void Awake()
    {



        Destroy(gameObject, 5f);
    }


    private void Update()
    {



        Debug.DrawLine(raycastPoint.transform.position, transform.position, Color.green, 5);
    }




    private void OnTriggerEnter(Collider collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            /* These projectiles are instantiated locally, as in,
             * they are not networked. Because of this there is a very
             * small chance the occasional projectile may not align with
             * 100% accuracy. But, the differences are generally
             * insignifcant and will not affect gameplay. */
                //transform.position-=GetComponent<Rigidbody>().velocity/2f;

            //If client show visual effects, play impact audio.
            if (InstanceFinder.IsClient)
            {
                //Debug.Log("Visuals and audio");
                //Show VFX.

                if (Physics.Linecast(raycastPoint.transform.position, transform.position, out RaycastHit hit))
                {

                    Debug.DrawLine(raycastPoint.transform.position, transform.position, Color.red, 5);



                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, -hit.normal);
                    Vector3 pos = hit.point;
                    Instantiate(hitSparks, pos, rot);



                    //Play Hit Sound Audio.

                }
            }
            //If server check to damage hit objects.
            if (InstanceFinder.IsServer)
            {
                //Debug.Log(collision.transform.gameObject.ToString());
                /* If a player ship was hit then remove 12 health.
                 * The health value can be synchronized however you like,
                 * such as a syncvar. */
                if(collision.gameObject.TryGetComponent<ShipPart>(out ShipPart ps))
                {

                    ps.hitPoints -= damage;
                    
                         ps.DestroyIfDead();
                    
                }
                
            }
            //if (collision.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))

            //Destroy projectile (probably pool it instead).
                Destroy(gameObject);

            //Despawn(gameObject);
        }
    }
}