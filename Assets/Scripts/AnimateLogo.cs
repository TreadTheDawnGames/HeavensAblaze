using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using UnityEngine.Video;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AnimateLogo : MonoBehaviour
{
    [SerializeField]
    float fadeInWait = 3;
    [SerializeField]
    float waitTime = 3;
    [SerializeField]
    float fadeOutWait = 3;

    public delegate void AnimationCompletedHandler();
    public event AnimationCompletedHandler animationComplete = delegate { };

    [SerializeField]
    public NetworkUIV2 netHud;

    [SerializeField]
    VideoPlayer player;
    void Start()
    {

        player.targetCamera.cullingMask ^= 1 << LayerMask.NameToLayer("Default");
        
        RenderColor(Color.white, player.targetCamera) ;

        //player = GetComponent<VideoPlayer>();
        //player.targetCamera = FindObjectOfType<IdleCamera>().GetComponent<Camera>();

        player.targetCamera.backgroundColor = Color.white;

        if (netHud.devMode)
        {
            player.Stop();
            player.targetCameraAlpha = 0;
            player.targetCamera.cullingMask ^= 1 >> LayerMask.NameToLayer("Default");
            RenderSkybox(player.targetCamera);
            animationComplete.Invoke();
        }
        else
        {
            StartCoroutine(Animate());
        }
        


        //Destroy(gameObject);
    }
    IEnumerator Animate()
    {

        float elapsedTime = 0f;
        while (elapsedTime < fadeInWait)
        {
            
            player.targetCameraAlpha = Mathf.Lerp(0, 1, (elapsedTime / fadeInWait));
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);

        player.targetCamera.cullingMask ^= 1 >> LayerMask.NameToLayer("Default");
        RenderSkybox(player.targetCamera);

        elapsedTime = 0f;

        while (elapsedTime < fadeOutWait)
        {
            
            player.targetCameraAlpha = Mathf.Lerp(1, 0, (elapsedTime / fadeOutWait));
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
        print("completed logo");
        animationComplete.Invoke();

    }

    public void RenderSkybox(Camera targetCamera = null)
    {
        if (targetCamera == null)
        {
            //Get reference to main camera if no camera is passed
            targetCamera = Camera.main;
        }
        //set camera to render the skybox
        targetCamera.clearFlags = CameraClearFlags.Skybox;
    }

    public void RenderColor(Color color, Camera targetCamera = null)
    {
        if (targetCamera == null)
        {
            //Get reference to main camera if no camera is passed
            targetCamera = Camera.main;
        }

        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = color;
    }

}
