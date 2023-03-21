using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    [SerializeField] GameObject WithoutFragmentObj;
    [SerializeField] GameObject FragmentObj;
    public int ThrowCountForBreakGlass;   
    private bool IsBreaked=false;
    private bool IsBroken;
   [ShowInInspector,ReadOnly] private int CurrentThrows=0;
    [Button]
    void Setrefs()
    {
        WithoutFragmentObj = transform.GetChild(0).gameObject;
        FragmentObj = transform.GetChild(1).gameObject;
        FragmentObj.SetActive(false);
        var boxcol=   GetComponent<BoxCollider>();
        if (boxcol == null) gameObject.AddComponent<BoxCollider>();        
        boxcol.size = WithoutFragmentObj.transform.localScale;
        /*boxcol.isTrigger = true;*/
    }
    private void OnCollisionEnter(Collision other)
    {       
        if (IsBreaked) return;
        if (other.gameObject.CompareTag("Missile"))
        {
            Debug.Log(other.gameObject.name);
            CurrentThrows++;
            if(ThrowCountForBreakGlass==CurrentThrows)
            {
                BreakGlass();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {        
        if (IsBreaked) return;
        if (other.gameObject.CompareTag("Missile"))
        {
            Debug.Log(other.gameObject.name);
            CurrentThrows++;
            if (ThrowCountForBreakGlass == CurrentThrows)
            {
                BreakGlass();
            }
        }
    }
    [Button]
    void BreakGlass()
    {
        if (IsBreaked) return;
        IsBreaked = true;
        GetComponent<BoxCollider>().isTrigger = IsBreaked;
        WithoutFragmentObj.SetActive(false);
        FragmentObj.SetActive(true);
        Destroy(gameObject,3f);
    }

}

