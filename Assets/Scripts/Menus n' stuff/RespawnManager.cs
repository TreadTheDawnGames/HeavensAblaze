using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
public class RespawnManager : NetworkBehaviour
{
    [SerializeField]
    NetworkUIV2 netUI;
  

    [SerializeField]
    Button button;
    
    public bool cockpitDead = false;
    private void Awake()
    {
        if (netUI == null)
        {
            netUI = FindObjectOfType<NetworkUIV2>();
        }

    }
    public void SetShowRespawn(bool show)
    {
        button.gameObject.SetActive(show);
        netUI.SetNetUIVisability(show);
    }
    public void StartRespawn()
    {
        StartCoroutine(Respawn());
    }
    public IEnumerator Respawn()
    {
        netUI.JoinRelay();
        yield return new WaitUntil(() => netUI.clientStarted == false);
        netUI.JoinRelay();
        button.gameObject.SetActive(false);
        
    }

}
