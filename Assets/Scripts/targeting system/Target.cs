using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    public PredictionMotor targetShip;
    public new RectTransform transform;

    public TMP_Text distanceDisplay;

    public Target(PredictionMotor targetShip)
    {
        this.targetShip = targetShip;
        this.transform = GetComponent<RectTransform>();
    }
}
