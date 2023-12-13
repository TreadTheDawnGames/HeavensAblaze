using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class AimPoint : MonoBehaviour
{

    private void Awake()
    {

        UpdatePosition(PlayerPrefs.GetInt("distance", 75));

        UpdateViewability(PlayerPrefs.GetInt("showAimpoint", 0) == 1 ? true : false);
    }

   

    public void UpdatePosition(int distance)
    {
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, distance);
    }

    public void UpdateViewability(bool active)
    {
        transform.gameObject.GetComponent<MeshRenderer>().enabled = active;

    }

}
