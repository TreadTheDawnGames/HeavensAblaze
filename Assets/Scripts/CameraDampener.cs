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

    public bool cockpitDied = false;

    private void Update()
    {
        /*if (cockpitDied)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, transitionSpeed*Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, qTargetRot, transitionSpeed*Time.deltaTime);
            
        }*/
    }

    public void Transition()
    {
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
