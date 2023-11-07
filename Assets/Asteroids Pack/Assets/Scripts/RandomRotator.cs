using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    [SerializeField]
    private float tumble;

    void Start()
    {
        //GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
        Vector3 rotationRandomizer = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        GetComponent<Rigidbody>().AddTorque(rotationRandomizer, ForceMode.VelocityChange) ;
    }
}