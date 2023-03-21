using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public class ObjectControls : MonoBehaviour
{
    Joystick m_joystick;
    float horizontal;
    float vertical;
    Vector3 movementDir;
    internal Player m_PlayerInside;
    [SerializeField] Image m_HpImage;
    private bool IsInDistroyCount;
    [SerializeField] private GameObject ParticleOnDestroy;
    [SerializeField] internal Collider m_AttachedCollider;
    [SerializeField] private RectTransform m_CanvasRec;
    [SerializeField] List<MeshRenderer> m_AllMeshRenderers = new List<MeshRenderer>();    
    private Vector3 HealthBarOffset;
    public Rigidbody m_Rigidbody;
    public bool IsBall, IsAmongGuy;
    bool isInput;
    Animator _AmongGuy;
    [Button]
    private void SetRefs()
    {
        m_AllMeshRenderers.Clear();
        m_AllMeshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }      
    private void OnEnable()
    {
        Instantiate(Gamemanager.Instance.ParticleEnhanceBlue, m_AttachedCollider.bounds.center, Quaternion.identity);
        var can = Instantiate(Gamemanager.Instance.ObjectsCanvas);
        m_CanvasRec = can.GetComponent<RectTransform>();
        m_HpImage = m_CanvasRec.GetChild(0).GetComponent<Image>();
    }

    void Start()
    {
        ParticleOnDestroy = Resources.Load("NewBloodblue") as GameObject;
        m_CanvasRec.localScale = new Vector3(0.5f, 1f, 1f);
        m_CanvasRec.gameObject.SetActive(true);
        m_joystick = FindObjectOfType<Joystick>();
        m_HpImage = m_HpImage.transform.GetChild(0).GetComponent<Image>();
        m_HpImage.fillAmount = 1f;
        HealthBarOffset = Vector3.up * (m_AttachedCollider.bounds.max.y + (IsBall ? 0.5f : 0.4f));

    }
    [Button]
    void SetCanPos()
    {
        HealthBarOffset = Vector3.up * m_AttachedCollider.bounds.max.y;
        Debug.Log(HealthBarOffset);
        HealthBarOffset.y += 1f;
        m_CanvasRec.position = HealthBarOffset + m_AttachedCollider.bounds.center;
    }

    void Update()
    {
        //Debug.Log("Is destory count is " + IsInDistroyCount);
        if (IsInDistroyCount) return;
        m_CanvasRec.position = HealthBarOffset + m_AttachedCollider.bounds.center;
            // m_CanvasRec.eulerAngles = localrot;       
        horizontal = m_joystick.Horizontal;
        vertical = m_joystick.Vertical;
        //New added
        if (!IsAmongGuy)
        {
            movementDir = new Vector3(horizontal, 0f, vertical);

            if (movementDir.magnitude > 0)
            {

                m_Rigidbody.AddForce(movementDir * 5, ForceMode.Impulse);
                //m_Rigidbody.AddTorque(movementDir*50, ForceMode.Acceleration);
            }
            m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, 3);
        }
        else
        {           
            movementDir = new Vector3(horizontal, 0f, vertical);
            isInput = movementDir != Vector3.zero;
            if(_AmongGuy==null)
            {
                _AmongGuy= GetComponent<Among_usPlayer>().m_Animator;
            }                  
            if (isInput)
            {
                _AmongGuy.SetBool("Run", true);
            }
            else
            {
                _AmongGuy.SetBool("Run", false);
                transform.parent.localPosition = new Vector3(transform.parent.localPosition.x + movementDir.x * 4.5f * Time.deltaTime, transform.parent.localPosition.y, transform.parent.localPosition.z + movementDir.z * 4.5f * Time.deltaTime);//old is -0.633f
                Rotate(movementDir);
            }
           
        }
        /*movementDir = new Vector3(horizontal, 0f, vertical);
        
        if (movementDir.magnitude > 0)
        {
            
            m_Rigidbody.AddForce(movementDir * 5, ForceMode.Impulse);
            //m_Rigidbody.AddTorque(movementDir*50, ForceMode.Acceleration);
        }
        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, 3);*/

        if (m_PlayerInside != null)
        {
            m_HpImage.fillAmount -= Time.deltaTime * 0.1f;
            m_PlayerInside.transform.position = m_AttachedCollider.bounds.center;
            if (m_HpImage.fillAmount <= 0.02f && !IsInDistroyCount)
            {
                OnDestroyThisThing();
            }
        }
    }
    void Rotate(Vector3 moveDir)
    {
        if (!GetInputValue())
            return;
        if (IsAmongGuy)
        {
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 45);

        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 45);
        }
    }
    public bool GetInputValue()
    {
        return isInput;
    }
    public void SetAllColor(Color i_Color)
    {
        if(IsAmongGuy)
        {
            GetComponent<SkinnedMeshRenderer>().sharedMaterial.color = i_Color;
        }
        else
        {
            m_AllMeshRenderers.ForEach(x => x.material.color = i_Color);
            if (m_AllMeshRenderers.Count == 1)
            {
                if (m_AllMeshRenderers[0].materials.Length > 1)
                {
                    for (int i = 0; i < m_AllMeshRenderers[0].materials.Length; i++)
                    {
                        m_AllMeshRenderers[0].materials[i].color = i_Color;
                    }
                }
            }
        }      
    }

    
    public void OnDestroyThisThing()
    {   //Die fridge        
        if (IsInDistroyCount || m_PlayerInside == null) return;
        IsInDistroyCount = true;
        Instantiate(Gamemanager.Instance.ParticleEnhanceBlue, m_AttachedCollider.bounds.center, Quaternion.identity);
        m_PlayerInside.OnMoveIntoOther(m_PlayerInside.transform, false, true);
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            /* Instantiate(ParticleOnDestroy, transform.position, Quaternion.identity);*/

            Destroy(m_CanvasRec.gameObject);
            Destroy(gameObject);
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsInDistroyCount) return;
        Debug.Log("Other triggered object is :: " + other.gameObject.name);
        if (other.gameObject.layer == 8 && m_PlayerInside != null)
        {
            Debug.Log("Player special is ");
            OnDestroyThisThing();
        }
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            if(m_PlayerInside !=null)
            {
                m_HpImage.fillAmount -= 0.3f;
                if (m_HpImage.fillAmount <= 0.02f && !IsInDistroyCount)
                {
                 OnDestroyThisThing();
                }
            }
        }
        if (other.CompareTag("Cylinder") && m_PlayerInside != null)
        {
            OnDestroyThisThing();
        }
        /*if (other.gameObject.layer == 8 && m_PlayerInside != null && !other.gameObject.GetComponent<Ref>().isref)
        {
            Debug.Log("Added new condition");
            m_HpImage.fillAmount -= 0.5f;
        }*/
        //New added
        //if (other.CompareTag("Boundry") && m_PlayerInside != null)
        //{
        //    //Destory ref object whenever reach boundry
        //    OnDestroyThisThing();
        //}
    }
    public void OnCollisionEnter(Collision collision)
    {   //Decrase a More helth bar         
        if (IsInDistroyCount) return;
        if (collision.gameObject.layer == 8 && m_PlayerInside != null && !collision.gameObject.GetComponent<Ref>().isref)
        {
            m_HpImage.fillAmount -= 0.1f;
        }
    }
}