using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DamageHologram : MonoBehaviour
{

    

    public Color GetDamageColor(ShipPart damagedPart)
    {

        if (damagedPart.hitPoints > (damagedPart.maxHitPoints / 3f) && damagedPart.hitPoints < 2f * (damagedPart.maxHitPoints / 3f))
        {
            return Color.yellow * 20;


        }
        else if (damagedPart.hitPoints < (damagedPart.maxHitPoints / 3) && damagedPart.hitPoints > 0f)
        {
            return Color.red * 40;

        }
        else if (damagedPart.hitPoints <= 0)
        {
            return Color.green * 0f;



        }
        else return Color.green;

        
    }


    public void ChangeCounterpartColor(GameObject counterpart, ShipPart damagedPart)
    {
        if (counterpart == null || counterpart.activeInHierarchy == false)
            return;


        if (damagedPart.hitPoints <= 0)
        {
            if (counterpart.transform.childCount > 0)
            {
                for (int i = 0; i < counterpart.transform.childCount; i++)
                {
                    ChangeCounterpartColor(counterpart.transform.GetChild(i).gameObject, damagedPart);
                }
            }
        }

        StartCoroutine(SwapHUDMaterial(counterpart, GetDamageColor(damagedPart), damagedPart));
        



    }
    public Material glitchedMaterial;
    public Material regularMaterial;


    private IEnumerator SwapHUDMaterial(GameObject hudPart, Color color, ShipPart damagedPart)
    {
        if (hudPart == null)
            yield break;

        hudPart.GetComponent<MeshRenderer>().material = glitchedMaterial;
        hudPart.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);

        yield return new WaitForSeconds(0.1f);

        bool damagedDestroyed = false;
        if (damagedPart == null) damagedDestroyed = true;

        
        if (hudPart != null && !damagedDestroyed)
        {
            hudPart.GetComponent<MeshRenderer>().material = regularMaterial;
            hudPart.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);


        }
    }
}