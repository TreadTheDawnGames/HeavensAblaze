using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParticleSysPlayDespawn : MonoBehaviour
{

    public List<AudioClip> audios = new List<AudioClip>();

    // Start is called before the first frame update
    private void Awake()
    {

        if(TryGetComponent<AudioSource>(out AudioSource audio))
        {

            audio.clip = audios[Random.Range(0, audios.Count)];
            audio.Play();
        }

        

        Destroy(transform.gameObject, GetComponent<ParticleSystem>().main.duration);
    }
}
