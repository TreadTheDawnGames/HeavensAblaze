using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LambdaTheDev.NetworkAudioSync;
using FishNet.Object;
using Unity.VisualScripting;

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
    NetworkAudioSource netDeadThrust;

    [SerializeField]
    List<AudioSource> thrustSource = new List<AudioSource>();

    [SerializeField]
    AudioSource lateralSource;

    [SerializeField]
    AudioSource liftSource;

    [SerializeField]
    AudioSource deadThrust;

    [SerializeField]
    Transform lateralSoundTruck;

    [SerializeField]
    Transform liftSoundTruck;

    [SerializeField]
    PredictionMotor root;
    private void PlayLateral(float lateral)
    {

        lateralSoundTruck.localPosition = new Vector3(-lateral, 0);
        netLateralSource.Volume = Mathf.Abs(lateral);

    }
    private void PlayLift(float lift)
    {

        liftSoundTruck.localPosition = new Vector3(0,-lift);

        liftSource.volume = Mathf.Abs(lift);

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
    public void PlayClientSounds(float thrust, float lift, float lateral)
    {
        PlayThrust(thrust);
        PlayLift(lift);
        PlayLateral(lateral);
    }

    [ServerRpc]
    public void PlayServerSounds(float thrust, float lift, float lateral, bool brake)
    {
        if (root.inputType == PredictionMotor.InputType.Disabled) 
                return;
                

        thrust += 0.25f;

        //thrust = -thrust;
        if (thrust < 0f)
            thrust = Mathf.Abs(thrust) * 0.5f;

        thrust = Mathf.Clamp(thrust, 0.1f, 1f);
        if (brake) thrust *=0.75f;

        PlayServerThrust(thrust);
        PlayServerLift(lift);
        PlayServerLateral(lateral);
       // PlayClientSounds(thrust, lift, lateral);
    }

    private void PlayServerLateral(float lateral)
    {
        if (lateralSoundTruck != null)
        {


            lateralSoundTruck.localPosition = new Vector3(-lateral, 0);
            netLateralSource.Volume = Mathf.Abs(lateral);
        }

    }
    private void PlayServerLift(float lift)
    {
        if (liftSoundTruck != null)
        {

         
        liftSoundTruck.localPosition = new Vector3(0,-lift);

        netLiftSource.Volume = Mathf.Abs(lift);
        }

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
