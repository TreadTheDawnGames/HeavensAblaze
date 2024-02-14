using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingHud : MonoBehaviour
{
    [SerializeField]
    RectTransform target;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Camera shipCam;

    private void Start()
    {
        
    }

    private void Update()
    {
        foreach(PredictionMotor ship in FindObjectsOfType<PredictionMotor>())
        {
            target.localPosition = shipCam.WorldToScreenPoint(ship.transform.position);
        }
    }

}
