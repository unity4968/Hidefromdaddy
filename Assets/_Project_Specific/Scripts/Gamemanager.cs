using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    public GameObject GameOver_Panel, Win_Panel, Main_camera, MenuDrag;
    public GameObject Level;
    [SerializeField] Text Level_no_txt;
    public bool IsUIOpen = false;
    [SerializeField] public GameObject ParticleEnhanceBlue;
    [SerializeField] public GameObject ObjectsCanvas;
    [SerializeField] public Joystick _Joystick;
    [SerializeField] public List<GameObject> Levels = new List<GameObject>();
    public bool Isstarted;


    public AudioSource m_win;
    public AudioSource m_loose;

    public Button m_Home;
    public int ForceLevel
    {
        get { return PlayerPrefs.GetInt("ForceLevel", -1); }
        set { PlayerPrefs.SetInt("ForceLevel", value); }
    }
    public int LoadForceLevel
    {
        get { return PlayerPrefs.GetInt("LoadForceLevel", -1); }
        set { PlayerPrefs.SetInt("LoadForceLevel", value); }
    }
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        m_Home.onClick.AddListener(() =>
        {
            AdsManager.inst.HomeScreen();
        });

        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level"));
            PlayerPrefs.Save();
        }
        LevelLoad(PlayerPrefs.GetInt("Level"));
        Level_no_txt.text = "Level " + (PlayerPrefs.GetInt("Level") + 1f);
    }
    public void Gameover()
    {
        if (!IsUIOpen)
        {
            AdsManager.inst.ShowInterstitial("");
            GameOver_Panel.SetActive(true);
            _Joystick.gameObject.SetActive(false);
            Debug.Log("Call Ones");
            IsUIOpen = true;
            m_loose.Play();
        }
    }
    public void WinGame()
    {
        AdsManager.inst.ShowInterstitial("");
        m_win.Play();
        IsUIOpen = true;
        Win_Panel.SetActive(true);
    }
    public void Onbtnclick(string buttonname)
    {

        switch (buttonname)
        {
            case "Reload":
                GameOver_Panel.SetActive(false);
                Destroy(Level);
                Level = null;
                LevelLoad(PlayerPrefs.GetInt("Level"));
                SimpleCamFollow.instance.SetcamerastartPos();
                ResetCamera();
                Isstarted = false;
                Level_no_txt.text = "Level " + (PlayerPrefs.GetInt("Level") + 1);
                break;
            case "Next":
                Win_Panel.SetActive(false);
                PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                //if (PlayerPrefs.GetInt("Level") < 13)
                //{
                //    PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                //}
                //else
                //{
                //    PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level")));
                //}
                PlayerPrefs.Save();
                Destroy(Level);
                Level = null;
                LevelLoad(PlayerPrefs.GetInt("Level"));
                SimpleCamFollow.instance.SetcamerastartPos();
                ResetCamera();
                Isstarted = false;
                Level_no_txt.text = "Level " + (PlayerPrefs.GetInt("Level") + 1);
                //Load next Level..
                break;
        }
    }
    public void OnClickGameStart()
    {
        Isstarted = true;
        Debug.Log(Isstarted);
        MenuDrag.SetActive(false);
    }
    public void LevelLoad(int Level_no)
    {
       AdsManager.inst.RequestAndLoadInterstitialAd();
        MenuDrag.SetActive(true);
        IsUIOpen = false;
        GameObject g;
        //GameObject g = (Resources.Load("Level_" + Level_no)) as GameObject;//Tempory 4
        if (Level_no <= 12)
        {
            g = Levels[Level_no] as GameObject;
        }
        else
        {
            if (ForceLevel == -1)
            {
                ForceLevel = Random.Range(0, 13);
                g = Levels[ForceLevel] as GameObject;
            }
            else
            {
                if (LoadForceLevel == Level_no)
                {
                    g = Levels[ForceLevel] as GameObject;
                }
                else
                {
                    ForceLevel = Random.Range(0, 13);
                    g = Levels[ForceLevel] as GameObject;
                }

            }
            LoadForceLevel = Level_no;
        }
        if (Level == null)
        {
            Level = Instantiate(g);
        }
    }
    [Button]
    void SetLevel(int i_LevelNo)
    {
        PlayerPrefs.SetInt("Level", i_LevelNo);
    }
    public void ResetCamera()
    {
        Main_camera.transform.localPosition = Level.transform.GetChild(0).transform.localPosition;
    }
}
