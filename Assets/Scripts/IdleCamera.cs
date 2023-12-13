using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class IdleCamera : MonoBehaviour
{
    /*[SerializeField]
    AudioListener listener;

    private void OnDisable()
    {

        listener.enabled = false;


    }

    private void OnEnable()
    {

        listener.enabled = true;

    }*/

    /*IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Destroy(transform.GetChild(0).gameObject);
    }*/

    void FixedUpdate()
    {


        transform.Rotate(Vector3.up, 5f * Time.fixedDeltaTime);

    }
}
