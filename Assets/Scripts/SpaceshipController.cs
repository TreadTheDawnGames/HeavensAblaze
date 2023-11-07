/*using FishNet.Object;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//using static UnityEditor.Timeline.TimelinePlaybackControls;
using FishNet.Connection;
using FishNet.Object.Synchronizing;


public class SpaceshipController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    
    public float thrustMultiplier = 2;
    public float rotSpeed = 5f;
    public float spaceStablizeSpeed = 5f;
    public float stablizeBrake = 5f;
    public bool alwaysStableize = true;
    public bool alwaysBrake = false;

    public bool keyboardEnabled = false;
    public bool joystickEnabled = true;

    public float spaceBrakeSpeed = 5f;

    public float clampVelocityMagnitude = 5f;

    public float numFramesBetweenShots = 0f;
    public float framesBetweenShots = 700f;

    private Rigidbody rb;
    private PlayerShip playerShip;
    public List<Blaster> blasters = new List<Blaster>();
    private AimPoint aimPoint;

    public List<Transform> children = new List<Transform>();
    Transform shipBody;
    public ShipPart shipPart;




    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            gameObject.GetComponent<SpaceshipController>().enabled = false;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

    }



    private void Awake()
    {
        aimPoint = GetComponentInChildren<AimPoint>();

        Physics.IgnoreLayerCollision(9, 10, true);

        blasters = GetComponentsInChildren<Blaster>().ToList<Blaster>();

        playerShip = new PlayerShip();

        //aimPoint = GetComponentInChildren<AimPoint>();

        //playerShip.Keyboard.Fire.performed += blaster.Fire;
    }



    private void FixedUpdate()
    {
        if (IsOwner)
        {
            KeyboardControls(keyboardEnabled, gameObject);
            JoystickControls(joystickEnabled, gameObject, playerShip);

            if (playerShip.Keyboard.Brake.IsInProgress() || playerShip.Joystick.Brake.IsInProgress() || alwaysBrake)
            {
                BrakeAll(BrakeX(gameObject), BrakeY(gameObject), BrakeZ(gameObject), gameObject);
            }
            numFramesBetweenShots++;
            numFramesBetweenShots = Mathf.Clamp(numFramesBetweenShots, 0f, framesBetweenShots);
            if (playerShip.Joystick.Fire.IsInProgress() && numFramesBetweenShots >= framesBetweenShots)
            {
                FireBlasters(blasters, aimPoint.transform.gameObject, gameObject);
                numFramesBetweenShots = 0f;
            }
        }

    }
    


    private void JoystickControls(bool joystickEnabled, GameObject gameObject, PlayerShip playerShip)
    {
        //AimPoint aimPoint = GetComponentInChildren<AimPoint>();

        if (joystickEnabled)
        {
            playerShip.Joystick.Enable();

        }
        else
        {
            playerShip.Joystick.Disable();
        }

        if (playerShip.Joystick.enabled)
        {
            DoRotation(-playerShip.Joystick.Pitch.ReadValue<float>(), playerShip.Joystick.Yaw.ReadValue<float>(), playerShip.Joystick.Roll.ReadValue<float>(), gameObject);
            DoMovement(playerShip.Joystick.Lateral.ReadValue<float>(), playerShip.Joystick.Lift.ReadValue<float>(), -playerShip.Joystick.Thrust.ReadValue<float>(), gameObject);
            
        }
    }

    [ServerRpc]
    void FireBlasters(List<Blaster> blasters, GameObject aimPoint, GameObject gameObject)
    {

        foreach (Blaster blaster in blasters)
        {
            if (blaster != null)
            {
                if (blaster.transform.IsChildOf(gameObject.transform))
                {
                    ServerManager.Spawn(blaster.Fire(blaster.gameObject.transform, aimPoint.transform));
                }
            }
        }

    }

    private void KeyboardControls(bool keyboardEnabled, GameObject gameObject)
    {
        if (keyboardEnabled)
        {
            playerShip.Keyboard.Enable();

        }
        else
        {
            playerShip.Keyboard.Disable();
        }

        if (playerShip.Keyboard.enabled)
        {
            DoRotation(playerShip.Keyboard.Pitch.ReadValue<float>(), playerShip.Keyboard.Yaw.ReadValue<float>(), playerShip.Keyboard.Roll.ReadValue<float>(), gameObject);
            DoMovement(playerShip.Keyboard.Lateral.ReadValue<float>(), playerShip.Keyboard.Lift.ReadValue<float>(), playerShip.Keyboard.Thrust.ReadValue<float>(), gameObject);
            numFramesBetweenShots++;
            numFramesBetweenShots = Mathf.Clamp(numFramesBetweenShots, 0f, framesBetweenShots);
            if (playerShip.Keyboard.Fire.IsInProgress() && numFramesBetweenShots >= framesBetweenShots)
            {

                foreach (Blaster blaster in blasters)
                {
                    if (blaster.transform.IsChildOf(gameObject.transform))
                    {

                        blaster.Fire(blaster.transform, aimPoint.transform);
                        ServerManager.Spawn(blaster.Fire(blaster.gameObject.transform, aimPoint.gameObject.transform));

                    }
                }
                numFramesBetweenShots = 0;
            }
        }
    }





    [ServerRpc(RequireOwnership=true)]
    void DoMovement(float inputVectorLateral, float inputVectorLift, float inputVectorThrust, GameObject gameObject)
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(inputVectorLateral * moveSpeed, inputVectorLift * moveSpeed, inputVectorThrust * moveSpeed * thrustMultiplier), ForceMode.VelocityChange);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(gameObject.GetComponent<Rigidbody>().velocity, clampVelocityMagnitude);


    }
    
    [ServerRpc(RequireOwnership =true)]
    public void BrakeAll(float brakeX, float brakeY, float brakeZ, GameObject gameObject)
    {
        Vector3 locVel = new Vector3(brakeX, brakeY, brakeZ);
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.TransformDirection(locVel);
    }
    public float BrakeZ(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.z, blend);
        locVel.z = MovSpeed;
        return locVel.z;
    }
    public float BrakeY(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.y, blend);
        locVel.y = MovSpeed;
        return locVel.y;
    }
    public float BrakeX(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity);

        float blend = MathF.Pow(0.5f, spaceBrakeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.x, blend);
        locVel.x = MovSpeed;
        return locVel.x;
    }


    [ServerRpc(RequireOwnership = true)]
    void DoRotation(float inputVectorPitch, float inputVectorYaw, float inputVectorRoll, GameObject gameObject)
    {
        gameObject.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(inputVectorPitch * rotSpeed, inputVectorYaw * rotSpeed, -inputVectorRoll * rotSpeed), ForceMode.VelocityChange);

        //if (Mathf.Abs(inputVectorPitch) + Mathf.Abs(inputVectorYaw) + Mathf.Abs(inputVectorRoll) == 0f)
        {
            float pitch = StableizeX(gameObject);
            float yaw = StableizeY(gameObject);
            float roll = StableizeZ(gameObject);

            if (playerShip.Keyboard.RotBrake.IsInProgress() || playerShip.Joystick.RotBrake.IsInProgress() || alwaysStableize)
            {
                pitch *= stablizeBrake;
                yaw *= stablizeBrake;
                roll *= stablizeBrake;
            }

            StableizeAll(pitch, yaw, roll, gameObject);
        }
    }
    public void StableizeAll(float stablizeX, float stableizeY, float stableizeZ, GameObject gameObject)
    {
        Vector3 locVel = new Vector3(stablizeX, stableizeY, stableizeZ);
        gameObject.GetComponent<Rigidbody>().angularVelocity = gameObject.transform.TransformDirection(locVel);
    }
    public float StableizeZ(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);

        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.z, blend);
        locVel.z = MovSpeed;
        return locVel.z;
    }
    public float StableizeY(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);

        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.y, blend);
        locVel.y = MovSpeed;
        return locVel.y;
    }
    public float StableizeX(GameObject gameObject)
    {
        //Debug.Log("Z + " + context);
        var locVel = gameObject.transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().angularVelocity);

        float blend = MathF.Pow(0.5f, spaceStablizeSpeed);

        float MovSpeed = Mathf.Lerp(0, locVel.x, blend);
        locVel.x = MovSpeed;
        return locVel.x;
    }

}*/