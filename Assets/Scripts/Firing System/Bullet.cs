using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet;
using FishNet.Managing.Timing;
using System;

[ObsoleteAttribute]
public class Bullet : NetworkBehaviour
{
    private Rigidbody rb;

    public float weaponDamage = 12;
    public float shootSpeed = 100f;
    public float deleteTime = 5f;

    public Transform upperEnd;

    public ParticleSystem hitSparks;

    // Start is called before the first frame update
    private void Awake()
    {

        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(0,shootSpeed,0, ForceMode.VelocityChange);

        //GetComponent<NetworkObject>().enabled = true;
        //GetComponent<NetworkObject>().Spawn(gameObject);

        Destroy(gameObject, deleteTime);

    }


    ///TODO
    ///Spawn seperate instances of laser for client and server
    ///Server does actual damage while client only shows visuals.
    ///onTriggerEnter instead of checking for hit every frame
    ///on trigger enter ignore raycast lasers
    ///replicate on server and observers (see point 1)
    ///Ask Dad what a struct is and how to use one
    ///
    ///ACTUALLY JUST MAKE A BULLET AND THE PROJECTILESHOOTERTEST SHOULD/COULD WORK

    
    
    private void FixedUpdate()
    {
        Debug.DrawLine(gameObject.transform.position, upperEnd.position, Color.green,2);
        // CalculateHit(gameObject);
        
        
    }


    //[ServerRpc(RequireOwnership = false)]


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (IsClient)
        {
            if (Physics.Linecast(gameObject.transform.position, upperEnd.position, out RaycastHit hit))
            {
                //Debug.Log(hit.transform.name);

                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Vector3 pos = hit.point;

                Debug.Log("pos = " + pos + "rot = " + rot);

                Instantiate(hitSparks, pos, rot);
                //ServerManager.Spawn(ps);                           
            }
        }

            if (IsServer)
            {

                ShipPart target = other.transform.GetComponent<ShipPart>();
                if (target != null)
                {
                    target.hitPoints -= weaponDamage;
                    target.DestroyIfDead();
                }

            }
        Destroy(gameObject);
    }

    




    public void CalculateHit(GameObject laser)
    {

        if (IsClient)
        {


            if (Physics.Raycast(gameObject.transform.position, transform.up * 2f, out RaycastHit hit))
            {
                Collider other = hit.collider;

                if (hit.transform.GetComponent<ParticleSystem>() == null)
                {

                    if (other.transform.tag != "projectile")
                    {

                        Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        Vector3 pos = hit.point;

                        GameObject ps = Instantiate(hitSparks, pos, rot).gameObject;
                        ServerManager.Spawn(ps);
                    }
                    
                }

                if (other.transform.root.gameObject.tag == "ship body" & other.transform.root != laser.transform.root)
                {

                    ShipPart target = other.transform.GetComponent<ShipPart>();

                    if (target != null)
                    {
                        target.hitPoints-=weaponDamage;
                        target.DestroyIfDead();

                    }

                }
            }
        }
        Destroy(laser);
    }
    

    public ShipPart GetTopmostParentWithShipPart(Transform transform)
    {
        if(transform.TryGetComponent<ShipPart>(out ShipPart SP))
        {
            return SP;
        }
        else
        {
            transform.TryGetComponent<Transform>(out Transform myTransform);

            if (transform.parent != null)
            {
                return GetTopmostParentWithShipPart(myTransform.parent);

            }
            else 
            { 
                Debug.Log("Bullet.GetTopmostParent... Returned Null"); 
                return null; 
            }
        }
    }






}
