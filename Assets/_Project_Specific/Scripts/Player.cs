using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public static Player Instance;
    public bool IsInOtherObject;
    public Transform Fridge;
    public GameObject Visual;

    [Header("Movement Settings")]
    private float movementSpeed = 2.8f;
    private float rotationSpeed = 45;
    float horizontal;
    float vertical;
    bool isInput;
    Vector3 movementDir;
    // Input Controls
    Joystick m_joystick;
    internal bool canMove = true;
    [SerializeField] internal Animator m_Animator;
    public float Health = 50f;
    [SerializeField] Image Helthbar;
    //[SerializeField]public RectTransform m_rectbar;
    [SerializeField] private GameObject ParticleOnDestroy;
    [SerializeField] BoxCollider m_ThisBoxCol;
    public Vector3 Camerapivot;
    private bool isJoystickActive = false;
    [SerializeField] ParticleSystem m_StartParticle;
    [SerializeField] GameObject m_NovaParticlePrefab;

    private void Awake()
    {
        /* if (!UIManager.instance)
         {
             SceneManager.LoadScene(0);
         }*/
        Instance = this;
    }
    void Start()
    {
     //   m_rectbar.SetParent(null);
     //   m_rectbar.name = "PlayerCanvas";
        ParticleOnDestroy = Resources.Load("NewBloodblue") as GameObject;
    //    m_rectbar.gameObject.SetActive(true);//tempory comment
        Helthbar.fillAmount = 1f;
        m_joystick = Gamemanager.Instance._Joystick;
        //m_joystick.KillJoystick();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime);
            //m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(Vector3.back * Time.deltaTime);
            //m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(Vector3.up, -10);
        //    m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(Vector3.up, 10);
         //   m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
        }
    }
#endif
    void FixedUpdate()
    {        
    //    m_rectbar.transform.position = new Vector3(m_ThisBoxCol.bounds.center.x,m_ThisBoxCol.bounds.max.y+0.5f,m_ThisBoxCol.bounds.center.z);
        if (!Gamemanager.Instance.Isstarted) return;
        m_joystick.gameObject.SetActive(!Gamemanager.Instance.IsUIOpen);
        if (!IsInOtherObject)
        {   
            Movement();
        }        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (Vector3.Distance(transform.position, other.transform.position) < 1.5f)
            {
                m_Animator.SetBool("HandAttack", true);

                IsInOtherObject = true;
                canMove = false;

                Gamemanager.Instance.Gameover();
                Debug.Log("Game Loose");
                transform.LookAt(other.transform);
                Health -= Time.deltaTime * 2;
                //Helthbar.fillAmount = Health;
            }
            if (Health <= 0)
            {
                var otheranimator = other.gameObject.GetComponent<Animator>();
                if (otheranimator)
                {
                    otheranimator.SetBool("GunAttack", false);
                    otheranimator.SetBool("HandAttack", false);
                }
                OnPlayerDie();
            }
        }
        /* if (other.gameObject.layer == 8 && other.gameObject.GetComponent<Ref>().isref)
         {
             var OtherObj = other.GetComponent<ObjectControls>();
             if (OtherObj.IsAmongGuy)
             {               
                 var Among_guy = other.GetComponent<Among_usPlayer>();
                 Among_guy.m_Animator.SetBool("Run", true);
             }
             if (OtherObj.m_PlayerInside == this) return;//Comment tempory
             OtherObj.enabled = true;
             OtherObj.m_PlayerInside = this;
             OtherObj.SetAllColor(Color.blue);            
             OtherObj.m_AttachedCollider.isTrigger = false;
             OtherObj.m_Rigidbody.useGravity = true;
             Transform ase = other.gameObject.transform;

             OnMoveIntoOther(ase, true, false);

         }
         if (other.CompareTag("Bullet") && Health > 1)
         {
             Destroy(other.gameObject);
             Health -= 10.0f;
             Helthbar.fillAmount = Health/50f;
             if (Health <= 0)
             {
                 *//*var otheranimator = other.gameObject.GetComponent<Animator>();
                 if (otheranimator)
                 {
                     otheranimator.SetBool("GunAttack", false);
                     otheranimator.SetBool("HandAttack", false);
                 }*//*
                 OnPlayerDie();
             }
         }*/
    }
    private void OnTriggerStay(Collider other)
    {/*
        if (other.gameObject.CompareTag("Enemy") && Health > 0)
        {
            if (Vector3.Distance(transform.position, other.transform.position) < 1.5f)
            {
                m_Animator.SetBool("HandAttack", true);
                Gamemanager.Instance.Gameover();
                Debug.Log("Game Loose"); 
                transform.LookAt(other.transform);
                Health -= Time.deltaTime * 2;
                //Helthbar.fillAmount = Health;
            }
            if (Health <= 0)
            {
                var otheranimator = other.gameObject.GetComponent<Animator>();
                if (otheranimator)
                {
                    otheranimator.SetBool("GunAttack", false);
                    otheranimator.SetBool("HandAttack", false);
                }
                OnPlayerDie();
            }
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        m_Animator.SetBool("HandAttack", false);
    }
    public void OnMoveIntoOther(Transform m_CamTargetTransform, bool IsPlayerInsideOther, bool PlayerSetActive)
    {
        transform.gameObject.SetActive(PlayerSetActive);
     //   m_rectbar.gameObject.SetActive(PlayerSetActive);
        if (PlayerSetActive)
        {
            GetComponent<Rigidbody>().isKinematic = true;

            m_Animator.SetBool("Run", false);
          //  m_Animator.SetBool("Jump", true);
            transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            DOVirtual.DelayedCall(0.25f, () =>
            {
                GetComponent<Rigidbody>().isKinematic = false;
                IsInOtherObject = IsPlayerInsideOther;
            });
        }
        else
        {
            IsInOtherObject = IsPlayerInsideOther;
        }
        SimpleCamFollow.instance.m_Target = m_CamTargetTransform.transform;
        SimpleCamFollow.instance.IsPlayerInSide = IsPlayerInsideOther;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Glass"))
        {
            if (m_StartParticle.isPlaying || Health<1) return;
            Health -= 10.0f;
            Helthbar.fillAmount = Health / 50f;
            m_StartParticle.Play();
            if (Health <= 0)
            {
                m_StartParticle.Stop();
                OnPlayerDie();
            }
            
        }
    }
    void Movement()
    {
        if (!canMove) return;

        if (m_joystick == null)
            m_joystick = Gamemanager.Instance._Joystick;
        if (m_joystick == null)
            return;
        // Pass input value
        horizontal = m_joystick.Horizontal;
        vertical = m_joystick.Vertical;
        movementDir = new Vector3(horizontal, 0f, vertical);
        isInput = movementDir != Vector3.zero;
        if (!isInput)
        {
         //   m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", false);
            return;
        }
        else
        {
            if (!isJoystickActive)
            {
                isJoystickActive = true;
              //  m_joystick.SetColor();
            }
           // m_Animator.SetBool("HandAttack", false);
         //   m_Animator.SetBool("Jump", false);
            m_Animator.SetBool("Run", true);
            transform.localPosition = new Vector3(transform.localPosition.x + movementDir.x * movementSpeed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z + movementDir.z * movementSpeed * Time.deltaTime);//old is -0.633f
            Rotate(movementDir);
        }   
    }
    [Button]
    void ShootOnGlass()
    {
        m_Animator.SetBool("MagicAttack", true);
        DOVirtual.DelayedCall(1.2f, () => 
        {
            if (Physics.Raycast(new Vector3(m_ThisBoxCol.bounds.center.x, m_ThisBoxCol.bounds.center.y, m_ThisBoxCol.bounds.center.z + 2f), transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                /*var novaeffect = Instantiate(m_NovaParticlePrefab, m_ThisBoxCol.bounds.center, Quaternion.identity);*/
                /* novaeffect.transform.DOMove(hit.point, 1f);*/

                GameObject projectile = Instantiate(m_NovaParticlePrefab, new Vector3(m_ThisBoxCol.bounds.center.x, m_ThisBoxCol.bounds.center.y, m_ThisBoxCol.bounds.center.z + 2f), Quaternion.identity) as GameObject; //Spawns the selected projectile
                projectile.transform.LookAt(hit.point); //Sets the projectiles rotation to look at the point clicked
                projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 1000f);
            }
        });
        DOVirtual.DelayedCall(2.3f, () => m_Animator.SetBool("MagicAttack", false));
       
    }
    public void OnPlayerDie()
    {
        Health = 0;
        Helthbar.fillAmount = 0f;
      //  m_rectbar.gameObject.SetActive(false);
        /*Instantiate(ParticleOnDestroy, transform.position, Quaternion.identity);*/
        m_Animator.SetTrigger("Die");
        Destroy(gameObject, 3);
        DOVirtual.DelayedCall(3, () =>
        {
            Gamemanager.Instance.Gameover();
        });
    }
    public void CancelMove()
    {
        canMove = false;
        /*j^oystick = null;*/
    }
    void Rotate(Vector3 moveDir)
    {
        if (!GetInputValue())
            return;        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotationSpeed);
    }
    public bool GetInputValue()
    {
        return isInput;
    }
}