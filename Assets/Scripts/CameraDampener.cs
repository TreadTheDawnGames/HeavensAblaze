using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using System.Runtime.CompilerServices;

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
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }
    private void LateUpdate()
    {

        
       // transform.localPosition = startPosition + -Vector3.ClampMagnitude(transform.InverseTransformDirection(transform.root.GetComponent<Rigidbody>().angularVelocity), maxDisplacement) * positionMultiplier;
        //transform.localPosition = -Vector3.ClampMagnitude(transform.InverseTransformDirection(transform.root.GetComponent<Rigidbody>().velocity), maxDisplacement) * positionMultiplier;
        transform.localRotation = Quaternion.Euler(Vector3.ClampMagnitude(transform.InverseTransformDirection(transform.root.GetComponent<Rigidbody>().angularVelocity), maxDisplacement) * positionMultiplier);
        
    }


    public void Transition()
    {
        if (isActiveAndEnabled)
            StartCoroutine(Lerp());
    }

    float timeElapsed = 0;
    float lerpDuration = 3;

    public IEnumerator Lerp()
    {
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
