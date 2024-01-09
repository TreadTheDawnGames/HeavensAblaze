using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class CameraDampener : NetworkBehaviour
{
    [SerializeField]
    Vector3 targetPosition;
    [SerializeField]
    Vector3 targetRotation;
    [SerializeField]
    float transitionSpeed = 1.0f;

    [SerializeField]
    PredictionMotor ship;

    public bool cockpitDied = false;

    public float maxDisplacement = 0.1f;
    public float positionMultiplier = 0.25f;

    [SerializeField]
    Camera cam;

    private void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if(ship==null) ship=transform.root.GetComponent<PredictionMotor>();
        StartCoroutine(WiggleCamera());
    }

    public float minFOV = 60;
    public float maxFOV = 61;
    public float thrustMultiplier = 0.5f;

    public IEnumerator WiggleCamera()
    {
        while (!cockpitDied)
        {
            transform.localRotation = Quaternion.Euler(Vector3.ClampMagnitude(transform.InverseTransformDirection(transform.root.GetComponent<Rigidbody>().angularVelocity), maxDisplacement) * positionMultiplier);
            if (ship.personalizationManager.useFOVEffects)
            {
                cam.fieldOfView += thrustMultiplier*(-0.5f+ship._thrust);
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
            }
            yield return null;
        }
    }

    public void Transition()
    {
        print("transition");
        if (isActiveAndEnabled)
            StartCoroutine(Lerp());
    }

    float timeElapsed = 0;
    float lerpDuration = 3;

    public IEnumerator Lerp()
    {
        print("camera lerp");
        StopCoroutine(WiggleCamera());
        Quaternion qTargetRot = Quaternion.Euler(targetRotation);
        
        //float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, transitionSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, qTargetRot, transitionSpeed * Time.deltaTime);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
    }



}
