using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AimPoint : MonoBehaviour
{
    public bool active;
    PersonalizationManager pManager;

    private void Awake()
    {
        pManager = FindAnyObjectByType<PersonalizationManager>();
    }

    private void Update()
    {
        active = pManager.aimpointActive;
        transform.gameObject.GetComponent<MeshRenderer>().enabled = active;

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, pManager.distance);
    }

    
}
