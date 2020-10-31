using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    PlayerController playerController;
    public PlayerController PlayerController { set { playerController = value; } }
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }

    /// <summary>
    /// 动画执行时每一帧调用
    /// </summary>
    private void OnAnimatorMove()
    {
        playerController.SetCharacterVelocity(anim.velocity);
    }

}
