using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGame : MonoBehaviour
{
    public GameObject ConfetiEffect;
    private void OnTriggerEnter(Collider other)
    {
        /*if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            return;
        }  */

        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            var Confettieffect = Instantiate(ConfetiEffect, transform);
            Confettieffect.transform.parent = GameObject.Find("Main Camera").gameObject.transform;
            //Confettieffect.transform.parent = m_Camera.transform;//Latest Camera
            Confettieffect.transform.localPosition = new Vector3(0f, 0.2f, 2f);
            DOVirtual.DelayedCall(0.5f, () => Gamemanager.Instance.WinGame());
        }
    }
}
