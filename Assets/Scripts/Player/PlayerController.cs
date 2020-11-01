using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public Transform target;
    /// <summary>
    /// 角色控制器
    /// </summary>
    CharacterController characterController;

    /// <summary>
    /// 动画控制器
    /// </summary>
    PlayerAnimController animController;

    /// <summary>
    /// 角色状态
    /// </summary>
    CharacterState state;

    /// <summary>
    /// 按键
    /// </summary>
    [SerializeField] InputKeys keys;

    /// <summary>
    /// 模型
    /// </summary>
    Transform model;

    /// <summary>
    /// 锁定状态下的摄像机
    /// </summary>
    GameObject lockCamera;

    /// <summary>
    /// 移动欲望
    /// </summary>
    Vector3 moveDir;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        animController = GetComponentInChildren<PlayerAnimController>();
        animController.PlayerController = this;
        state = new CharacterState();
        animController.State = state;
        model = transform.Find("Model");
        lockCamera = GameObject.Find("Virtual Camera Group").transform.Find("LockLook").gameObject;
    }
    
    private void Update()
    {
        InputCheck();
        
    }

    private void FixedUpdate()
    {
        FreeLook();
        LockLook();

        Vector2 movement = SquareToCircle(moveDir);
        if(state.isLocking && state.isEquiped)
        {
            animController.SetFloat("SpeedZ", movement.y);
            animController.SetFloat("SpeedX", movement.x);
        }
        else
        {
            animController.SetFloat("SpeedZ", movement.sqrMagnitude);
        }

    }

    /// <summary>
    /// 自由视角
    /// </summary>
    void FreeLook()
    {
        if (state.isLocking)
            return;
        if (moveDir.sqrMagnitude < 0.1f)
            return;

        model.localRotation = Quaternion.Lerp(model.localRotation, Quaternion.Euler(0 ,0 ,0), 12 * Time.deltaTime);

        Vector3 dir = moveDir.normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        Quaternion targetDir = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetDir, 12 * Time.deltaTime);
    }

    /// <summary>
    /// 锁定视角
    /// </summary>
    void LockLook()
    {
        if (!state.isLocking)
            return;
        if (moveDir.sqrMagnitude < 0.1f)
            return;

        Vector3 cameraEuler = Camera.main.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0, cameraEuler.y, 0);

        //非战斗状态
        if(!state.isEquiped)
        {
            Vector3 dir = moveDir.normalized;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Quaternion targetDir = Quaternion.Euler(0, angle, 0);
            model.localRotation = Quaternion.Lerp(model.localRotation, targetDir, 12 * Time.deltaTime);
        }
        //战斗状态
        else
        {
            model.localRotation = Quaternion.Lerp(model.localRotation, Quaternion.Euler(0, 0, 0), 12 * Time.deltaTime);
        }

    }

    /// <summary>
    /// 输入检测
    /// </summary>
    void InputCheck()
    {
        float fwd = 0;
        float right = 0;
        fwd = Input.GetKey(keys.fwd) ? 1 : fwd;
        fwd = Input.GetKey(keys.bwd) ? -1 : fwd;
        right = Input.GetKey(keys.right) ? 1 : right;
        right = Input.GetKey(keys.left) ? -1 : right;

        moveDir.z = Mathf.Lerp(moveDir.z, fwd, 24 * Time.deltaTime);
        moveDir.x = Mathf.Lerp(moveDir.x, right, 24 * Time.deltaTime);

        //拔出武器
        if (Input.GetKeyDown(keys.equip))
        {
            state.isEquiped = !state.isEquiped;
            animController.SetTrigger("Equip");
        }

        //锁定视角
        if(Input.GetKeyDown(keys.Lock))
        {
            state.isLocking = !state.isLocking;
            lockCamera.SetActive(state.isLocking);
            animController.SetBool("LockState", state.isLocking);
        }

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
