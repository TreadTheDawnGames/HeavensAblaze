using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public PredictionMotor targetShip;
    public new RectTransform transform;

    public Target(PredictionMotor targetShip)
    {
        this.targetShip = targetShip;
        this.transform = GetComponent<RectTransform>();
    }
}
