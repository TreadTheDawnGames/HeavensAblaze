using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LambdaTheDev.NetworkAudioSync;
using FishNet.Object;

public class ShipSound : NetworkBehaviour
{
    [SerializeField]
    AudioClip pushHiss;
    [SerializeField]
    AudioClip thrustOngoing;
    [SerializeField]
    AudioClip thrustCooldown;
    [SerializeField]
    AudioClip brake;

    [SerializeField]
    AudioClip[] collisionThud;

    [SerializeField]
    List<NetworkAudioSource> netThrustSource = new List<NetworkAudioSource>();

    [SerializeField]
    NetworkAudioSource netLateralSource;

    [SerializeField]
    NetworkAudioSource netLiftSource;

    [SerializeField]
    NetworkAudioSource netBrakeSource;

    [SerializeField]
    NetworkAudioSource netDeadThrust;

    [SerializeField]
    List<AudioSource> thrustSource = new List<AudioSource>();

    [SerializeField]
    AudioSource lateralSource;

    [SerializeField]
    AudioSource liftSource;

    [SerializeField]
    AudioSource brakeSource;

    [SerializeField]
    AudioSource deadThrust;


    private void PlayLateral(float lateral)
    {


        lateralSource.pitch = lateral;

    }
    private void PlayLift(float lift)
    {


        liftSource.pitch = lift;

    }
    [ObserversRpc]
    public void PlayDeadThrust(float thrust)
    {
        deadThrust.volume = thrust;

    }
    private void PlayThrust(float thrust)
    {
        foreach (AudioSource audioSource in thrustSource)
        {
            if(audioSource != null&&audioSource.isActiveAndEnabled)
            {

            audioSource.pitch = thrust;
            }
        }
    }

    [ObserversRpc]
    public void PlayClientSounds(float thrust, float lift, float lateral, bool brake)
    {
        PlayThrust(thrust);
        PlayLift(lift);
        PlayLateral(lateral);
    }

    [ServerRpc]
    public void PlayServerSounds(float thrust, float lift, float lateral, bool brake)
    {
        //thrust = -thrust;
        if (thrust < 0f)
            thrust = Mathf.Abs(thrust) * 0.5f;

        thrust = Mathf.Clamp(thrust, 0.3f, 1f);
        if (brake) thrust *=0.75f;

        PlayServerThrust(thrust);
        PlayServerLift(lift);
        PlayServerLateral(lateral);
        PlayClientSounds(thrust, lift, lateral, brake);
    }

    private void PlayServerLateral(float lateral)
    {


        netLateralSource.Pitch = lateral;

    }
    private void PlayServerLift(float lift)
    {


        netLiftSource.Pitch = lift;

    }
    private void PlayServerThrust(float thrust)
    {
        if (thrust < 0f)
            thrust = Mathf.Abs(thrust) * 0.5f;

        foreach(NetworkAudioSource audioSource in netThrustSource)
        {
            if ( audioSource !=null&& audioSource.isActiveAndEnabled)
            {
                audioSource.Pitch = thrust;

            }
        }
    }

    [ServerRpc]
    public void PlayServerDeadThrust(float thrust)
    {
        if (Mathf.Abs(thrust) < 0.15f)
            thrust = 0;
        netDeadThrust.Volume = Mathf.Abs(thrust);
        PlayDeadThrust(Mathf.Abs(thrust));
    }



}
