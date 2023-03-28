using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{

    public Animation mAnimation;
    private void Start()
    {
        mAnimation = GetComponent<Animation>();
        mAnimation.playAutomatically = false;
        Animation anim = mAnimation;
        /*foreach (AnimationState state in anim)
        {
            Debug.Log(state.name);
        }*/
    }
    public void OpenDoor()
    {
        mAnimation.Play("Door_OpenB");
    }
}
