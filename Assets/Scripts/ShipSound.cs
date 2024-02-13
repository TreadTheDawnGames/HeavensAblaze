using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LambdaTheDev.NetworkAudioSync;
using FishNet.Object;
using Unity.VisualScripting;
using GameKit.Utilities.Types;

public class ShipSound : NetworkBehaviour
{
  
    [SerializeField]
    List<NetworkAudioSource> netThrustSource = new List<NetworkAudioSource>();

    [SerializeField]
    NetworkAudioSource netLateralSource;

    [SerializeField]
    NetworkAudioSource netLiftSource;


    [SerializeField]
    NetworkAudioSource netDeadThrust;

    [SerializeField]
    NetworkAudioSource netRoll, netPitch, netYaw;

    [SerializeField]
    Transform lateralSoundTruck;

    [SerializeField]
    Transform liftSoundTruck;

    [SerializeField]
    PredictionMotor root;






    [ServerRpc]
    public void PlayServerSounds(float thrust, float lift, float lateral, float roll, float pitch, float yaw, bool brake, bool deadThrust)
    {
        if (root.inputType == PredictionMotor.InputType.Disabled) 
                return;

        float brakeVal;
        if (brake)
        {
            brakeVal = 0.75f;
            
        }
        else
        {
            brakeVal = 1f;
        }

        if (deadThrust)
        {
            if (thrust != 0f)
            {
                thrust = 1f;
                
            }
            PlayServerDeadThrust(thrust * brakeVal);
        }
        else
        {

            thrust += 0.25f;

            if (thrust < 0f)
                thrust = Mathf.Abs(thrust) * 0.5f;

            thrust = Mathf.Clamp(thrust, 0.1f, 1f);


            PlayServerThrust(thrust * brakeVal);
        }
        PlayServerLift(lift * brakeVal);
        PlayServerLateral(lateral * brakeVal);
        PlayServerRoll(roll * brakeVal);
        PlayServerPitch(pitch * brakeVal);
        PlayServerYaw(yaw * brakeVal);

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

    private void PlayServerRoll(float roll)
    {
        netRoll.Volume = Mathf.Abs(roll);
    }
    private void PlayServerPitch(float pitch)
    {
        netPitch.Volume = Mathf.Abs(pitch);
    }
    private void PlayServerYaw(float yaw)
    {
        netYaw.Volume = Mathf.Abs(yaw);
    }

    
    public void PlayServerDeadThrust(float thrust)
    {
        
        netDeadThrust.Volume = Mathf.Abs(thrust);
    }



}
