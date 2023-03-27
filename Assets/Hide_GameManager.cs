using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hide_GameManager : MonoBehaviour
{

    public GameObject m_WinPenal;
    public GameObject m_loosePenal;
    public static Hide_GameManager instance;

    public List<GameObject> m_Levels = new List<GameObject>();
    public GameObject Particalsystem;
    public TextMeshProUGUI m_levelText;
    public Button m_home;
    private void Start()
    {
        m_Levels.ForEach(x => x.SetActive(false));
        m_Levels[0].gameObject.SetActive(true);
        CurrentLevel = 0;
        m_levelText.text = "Level:" + (CurrentLevel+1);
        //m_home.onClick.AddListener(GameManager.ins.BackToMainMenu);
    }
    private void Awake()
    {
        instance = this;
    }
    private int CurrentLevel;
    public void NextLevel()
    {
        m_WinPenal.SetActive(false);
        CurrentLevel++;

        if (CurrentLevel>1)
        {
            CurrentLevel = 0;
        }
        m_levelText.text = "Level:" + (CurrentLevel+1);
        m_Levels.ForEach(x => x.SetActive(false));
        m_Levels[CurrentLevel].gameObject.SetActive(true);
        Particalsystem.SetActive(false);

    }
    public void RetryLevel()
    {
        m_loosePenal.SetActive(false);
        m_Levels.ForEach(x => x.SetActive(false));
        m_Levels[CurrentLevel].gameObject.SetActive(true);
        m_levelText.text = "Level:"+CurrentLevel;
    }
    public void Win()
    {
        Particalsystem.SetActive(true);
        DOVirtual.DelayedCall(2f, () =>
        {
            m_WinPenal.SetActive(true);

        });
    }
}
