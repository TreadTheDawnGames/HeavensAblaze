using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ConnectorWingDamageHologram : DamageHologram
{

    [SerializeField]
    GameObject alternateHoloSupport;

    public override void UpdateCounterpart(float hitPoints)
    {

            foreach(Transform child in transform)
               {
                   child.SetParent(alternateHoloSupport.transform);
               }
               RecoverLostHudItems(alternateHoloSupport);
        Destroy(gameObject);
    }


    void RecoverLostHudItems(GameObject alternateSupportWithChildren)
    {
        if (alternateSupportWithChildren == null)
            return;

        


        alternateSupportWithChildren.GetComponent<MeshRenderer>().material = regularMaterial;
         alternateSupportWithChildren.GetComponent<MeshRenderer>().material.SetColor("_MainColor", GetDamageColor(alternateHoloSupport.GetComponent<DamageHologram>().basePart.hitPoints));
        for (int i = 0; i < alternateSupportWithChildren.transform.childCount; i++)
        {
            RecoverLostHudItems(alternateSupportWithChildren.transform.GetChild(i).gameObject);
        }
    }
}
