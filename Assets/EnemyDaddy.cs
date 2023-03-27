using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyDaddy : MonoBehaviour
{
    public Transform Childe;
    public NavMeshAgent m_Daddy;
    public GameObject LoosePenal;
    private bool onetime;

    public Button RestartGameButton;
    public Button m_HomeButton;

    private void Start()
    {
        isfollowing = false;
        RestartGameButton.onClick.AddListener(RestartGame);
        //    m_HomeButton.onClick.AddListener(GameManager.ins.BackToMainMenu);
        m_Daddy = GetComponent<NavMeshAgent>();
        DOVirtual.DelayedCall(1f, () =>
        {
            //   m_Daddy.SetDestination(Childe.position);
            isfollowing = true;
        });
        onetime = false;
    }
    private bool isfollowing = false;

    private void FixedUpdate()
    {
        if (isfollowing)
        {
            m_Daddy.SetDestination(Childe.position);
        }
        if (Vector3.Distance(transform.position, Childe.position) < 1)
        {
            if (!onetime)
            {
                LoosePenal.SetActive(true);
                onetime = true;
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
}
