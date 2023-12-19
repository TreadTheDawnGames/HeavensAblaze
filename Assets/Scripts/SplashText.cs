using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;

[Serializable]
public class SplashText
{
    private static SplashText instance;

    private SplashText() { }

    [SerializeField] string[] texts = new string[] { };

    public static SplashText GetInstance()
    {
        if(instance == null)
        {
            instance = new SplashText();
        }
        return instance;
    }

    public string Get()
    {

        string path = Application.streamingAssetsPath + "/SplashText/" + "Splashes" + ".txt";

        try
        {
        texts = File.ReadAllLines(path);

        }
        catch
        {
            return "Did you delete the splash file? *squints*";
        }

        Debug.Log(texts.Length);

        if (texts.Length == 0)
        {
            Debug.LogError("No splashes!");
            return "Wake Up!";
        }
        int randomNum = UnityEngine.Random.Range(0, texts.Length - 1);
        Debug.Log(randomNum);
        return texts[UnityEngine.Random.Range(0, randomNum)];
    }
}
