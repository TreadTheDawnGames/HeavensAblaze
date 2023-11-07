using System;
using Unity.Services.Core;
using UnityEngine;



public class ServicesInitializer : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
