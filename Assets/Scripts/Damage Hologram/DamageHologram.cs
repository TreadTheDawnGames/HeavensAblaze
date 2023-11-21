using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DamageHologram : MonoBehaviour
{

    public Material glitchedMaterial;
    public Material regularMaterial;

/*    private int _damage;
    public int damage
    {
        get
        {
            return _damage;
        }
        set
        {
            _damage = value;
            doMyStuff(value);
        }
    }
*/
    

    public Color GetDamageColor(float partDamage)
    {

        if (partDamage > (50f / 3f) && partDamage < 2f * (50f / 3f))
        {
            return Color.yellow * 20;


        }
        else if (partDamage < (50f / 3) && partDamage > 0f)
        {
            return Color.red * 40;

        }
        else if (partDamage <= 0)
        {
            return Color.green * 0f;



        }
        else return Color.green;

        
    }


    public void ChangeColorAndMaterial(DamageHologram counterpart, float partDamage)
    {

        //GameObject counterpart= damagedPart.damageHudCounterpart;

        if (counterpart == null || counterpart.gameObject.activeInHierarchy == false)
            return;

        StartCoroutine(SwapHUDMaterial(counterpart,partDamage));

        /*if (partDamage <= 0)
        {

            Destroy(gameObject);
            
        }*/

        



    }

    private IEnumerator SwapHUDMaterial(DamageHologram counterpart, float damage)
    {
        print("Started coroutine for " + counterpart);

        Color color = GetDamageColor(damage);

        if (counterpart == null)
            yield break;

        counterpart.GetComponent<MeshRenderer>().material = glitchedMaterial;
        counterpart.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);

        print("material changed to hit");

        print("basePart damage (Before): " + counterpart.basePart.hitPoints);

        int count = 0;
       // while (count <= 150 && counterpart.basePart != null && damage == counterpart.basePart.hitPoints)
        {
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        print("basePart damage (After): " + count + " / " + counterpart.basePart.hitPoints);

        print("material changed to not-hit");


       /* bool damagedDestroyed = false;
        if (damagedPart == null) damagedDestroyed = true;*/
        if(counterpart.basePart == null || !counterpart.basePart.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
        }
        
        if (counterpart != null /*&& !damagedDestroyed*/)
        {
            counterpart.GetComponent<MeshRenderer>().material = regularMaterial;
            counterpart.GetComponent<MeshRenderer>().material.SetColor("_MainColor", color);

        }
        print("ended coroutine for " + counterpart);
    }

    public void SetToDeadMaterial(GameObject counterpart)
    {
        StopCoroutine("SwapHUDMaterial");

        counterpart.GetComponent<MeshRenderer>().material = glitchedMaterial;
        counterpart.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.green * 0f) ;

    }

    [SerializeField]
    public ShipPart basePart;

    /*public void UpdateHolo()
    {

       // print("hit " + gameObject.name);


        for (int i = 0; i < transform.childCount; i++)
        {
            if (basePart != null)
                transform.GetChild(i).GetComponent<DamageHologram>()?.UpdateCounterpart();
            else if(gameObject.GetComponent<MeshRenderer>()!=null)
            {
                gameObject.GetComponent<MeshRenderer>().material = glitchedMaterial;
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_MainColor", Color.green * 0f);

            }
            transform.GetChild(i).GetComponent<DamageHologram>()?.UpdateHolo();
        }
    }*/


    public virtual void UpdateCounterpart(float hitPoints)
    {
        if(basePart != null)
            ChangeColorAndMaterial(this, hitPoints);
        
    }
}