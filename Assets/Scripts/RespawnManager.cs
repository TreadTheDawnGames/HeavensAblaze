using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Managing.Timing;

public class RespawnManager : MonoBehaviour
{
    [SerializeField]
    NetworkUIV2 netUI;
  

    [SerializeField]
    public Button button;
    
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
        netUI.SetNetUIVisability(show);
        button.gameObject.SetActive(show);
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
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
