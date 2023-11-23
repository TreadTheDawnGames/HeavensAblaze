using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipSound : MonoBehaviour
{
    [SerializeField]
    AudioClip pushHiss;
    [SerializeField]
    AudioClip thrustOngoing;
    [SerializeField]
    AudioClip thrustCooldown;

    [SerializeField]
    AudioClip[] collisionThud;

    [SerializeField]
    AudioSource thrustSource;

    [SerializeField]
    AudioSource lateralSource;

    [SerializeField]
    AudioSource liftSource;

    [SerializeField]
    AudioSource collisionSource;

    
    public void PlayLateral(float lateral)
    {
        
        
            lateralSource.pitch = lateral;
        
    }public void PlayLift(float lift)
    {
        
        
            liftSource.pitch = lift;
        
    }

    public void PlayThrust(float volume)
    {
        if (volume < 0f)
            volume = Mathf.Abs(volume) * 0.5f;

        thrustSource.pitch = volume;
    }
    public void PlayCollision(Vector3 position, float volume)
    {
        collisionSource.clip = collisionThud[Random.Range(0, collisionThud.Count()-1)];
        collisionSource.transform.position = position;
        collisionSource.volume = volume;
        collisionSource.Play();



    }

}
