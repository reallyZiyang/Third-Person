using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    PlayerAnimController animController;

    Vector3 moveDir;
    bool isRunning;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animController = GetComponentInChildren<PlayerAnimController>();
        animController.PlayerController = this;
    }

    private void Update()
    {
        InputCheck();
    }

    private void FixedUpdate()
    {
        FreeLook();

        Vector2 movement = SquareToCircle(moveDir);
        animController.SetFloat("SpeedZ", isRunning ? movement.sqrMagnitude * 2 : movement.sqrMagnitude);

    }

    /// <summary>
    /// 自由视角
    /// </summary>
    void FreeLook()
    {
        if (moveDir.sqrMagnitude == 0)
            return;
        Vector3 dir = moveDir.normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        Quaternion targetDir = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetDir, 12 * Time.deltaTime);
    }

    /// <summary>
    /// 输入检测
    /// </summary>
    void InputCheck()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        moveDir.x = h;
        moveDir.z = v;

        isRunning = Input.GetKey(KeyCode.LeftShift);

    }

    /// <summary>
    /// 椭圆映射
    /// </summary>
    /// <param name="oldVec"></param>
    /// <returns></returns>
    Vector2 SquareToCircle(Vector3 oldVec)
    {
        Vector2 newVec;
        newVec.x = oldVec.x * Mathf.Sqrt(1 - (oldVec.z * oldVec.z) / 2);
        newVec.y = oldVec.z * Mathf.Sqrt(1 - (oldVec.x * oldVec.x) / 2);
        return newVec;
    }

    /// <summary>
    /// 动画根节点速度作为移动速度
    /// </summary>
    /// <param name="velocity"></param>
    public void SetCharacterVelocity(Vector3 velocity)
    {
        characterController.SimpleMove(new Vector3(velocity.x, characterController.velocity.y, velocity.z));
    }
    

}
