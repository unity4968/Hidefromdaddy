using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float speed = 10.0f;
    public float sensitivity = 2.0f;
    public float gravity;
    public float jump = 7f;

    private CharacterController player;
    private float groundDistance;

    public VariableJoystick moveJoystick;
    public VariableJoystick lookJoystick;

    public Button m_HomeScreen;
    public Transform RaycastPostion;
    void Start()
    {
        m_HomeScreen.onClick.AddListener(AdsManager.inst.HomeScreen);
        player = GetComponent<CharacterController>();
        groundDistance = player.bounds.extents.y;
    }
    void Update()
    {

        // get the movement
        //float moveFB = Input.GetAxis("Vertical") * speed;
        //float moveLR = Input.GetAxis("Horizontal") * speed;

        float moveFB = moveJoystick.Vertical * speed;
        float moveLR = lookJoystick.Horizontal * speed;

        // handle jumping
        if (isGrounded())
        {
            if (Input.GetButtonDown("Jump"))
            {
                gravity += jump;
            }
        }

        Vector3 movement = new Vector3(0, gravity, moveFB);

        movement = transform.rotation * movement;
        player.Move(movement * Time.deltaTime);
        transform.Rotate(0, moveLR, 0);

    }

    void FixedUpdate()
    {
        if (!isGrounded())
        {
            gravity += (Physics.gravity.y * 2) * (Time.deltaTime * 2);
            Debug.Log("Player is falling");
        }
        else
        {
            gravity = 0f;
            Debug.Log("Player is grounded");
        }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(RaycastPostion.position + Vector3.up * .5f, RaycastPostion.TransformDirection(Vector3.forward), out hit, 1, 1))
        {
            Debug.DrawRay(RaycastPostion.position + Vector3.up * .5f, RaycastPostion.TransformDirection(Vector3.forward) * 1, Color.green);
            Debug.Log("Did Hit::::---" + hit.collider.gameObject.name);
            if (hit.collider.gameObject.name == "DoorAnim")
            {
                hit.collider.GetComponent<DoorOpen>().OpenDoor();
            }
            if (hit.collider.gameObject.name == "CupboardDoor_L")
            {
                hit.collider.GetComponent<Animation>().Play("CupboardDoorOpen_L");
                hit.collider.GetComponent<Animation>().Play("CupboardDoorOpen_R");
                Debug.Log("DrowerOn");
            }
        }
        /*  else
          {
              Debug.DrawRay(RaycastPostion.position + Vector3.up * .5f, RaycastPostion.TransformDirection(Vector3.forward) * 1, Color.red);
              Debug.Log("Did not Hit");
          }*/
    }
    bool isGrounded()
    {
        return Physics.Raycast(transform.localPosition, -Vector3.up, groundDistance + 0.1f);
    }
}