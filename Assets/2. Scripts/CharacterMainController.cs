using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMainController : MonoBehaviour, IPunObservable
{
    /***********************************************************************
    *                               Definitions
    ***********************************************************************/
    #region .
    public enum CameraType { FpCamera, TpCamera };
    
    // 동기화 추가부분
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private CharacterController characterController;
    private PhotonView photonView;

    [Serializable]
    public class Components
    {
        public Camera tpCamera;
        public Camera fpCamera;

        [HideInInspector] public Transform tpRig;
        [HideInInspector] public Transform fpRoot;
        [HideInInspector] public Transform fpRig;

        [HideInInspector] public GameObject tpCamObject;
        [HideInInspector] public GameObject fpCamObject;

        [HideInInspector] public Rigidbody rBody;
        [HideInInspector] public Animator anim;
    }
    [Serializable]
    public class KeyOption
    {
        public KeyCode moveForward  = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft     = KeyCode.A;
        public KeyCode moveRight    = KeyCode.D;
        public KeyCode run  = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;
        public KeyCode switchCamera = KeyCode.Tab;
        public KeyCode showCursor = KeyCode.LeftAlt;
    }
    [Serializable]
    public class MovementOption
    {
        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = 6;
        [Tooltip("이동속도")]
        [Range(1f, 20f)]
        public float speed = 10f;
        [Tooltip("달리기 이동속도 증가 계수")]
        [Range(1f, 5f)]
        public float runningCoef = 3f;
        [Tooltip("점프 강도")]
        [Range(1f, 20f)]
        public float jumpForce = 8f;
        [Tooltip("가속도")]
        [Range(1f, 10f)]
        
        
    public float acceleration = 5f;
    }
    [Serializable]
    public class CameraOption
    {
        [Tooltip("게임 시작 시 카메라")]
        public CameraType initialCamera;
        [Range(1f, 10f), Tooltip("카메라 상하좌우 회전 속도")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("올려다보기 제한 각도")]
        public float lookUpDegree = -60f;
        [Range(0f, 75f), Tooltip("내려다보기 제한 각도")]
        public float lookDownDegree = 75f;
    }
    [Serializable]
    public class AnimatorOption
    {
        public string paramMoveX = "Move X";
        public string paramMoveZ = "Move Z";
        public string paramDistY = "Dist Y";
        public string paramGrounded = "Grounded";
        public string paramJump = "Jump";
        public float moveX;
        public float moveZ;
        public float distY;
        public bool grounded;
    }
    [Serializable]
    public class CharacterState
    {
        public bool isCurrentFp;
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isCursorActive;
    }

    #endregion
    /***********************************************************************
    *                               Fields, Properties
    ***********************************************************************/
    #region .
    public Components Com => _components;
    public KeyOption Key => _keyOption;
    public MovementOption MoveOption => _movementOption;
    public CameraOption   CamOption  => _cameraOption;
    public AnimatorOption AnimOption => _animatorOption;
    public CharacterState State => _state;

    [SerializeField] private Components _components = new Components();
    [Space]
    [SerializeField] private KeyOption _keyOption = new KeyOption();
    [Space]
    [SerializeField] private MovementOption _movementOption = new MovementOption();
    [Space]
    [SerializeField] private CameraOption   _cameraOption   = new CameraOption();
    [Space]
    [SerializeField] private AnimatorOption _animatorOption = new AnimatorOption();
    [Space]
    [SerializeField] private CharacterState _state = new CharacterState();

    private Vector3 _moveDir;
    private Vector3 _worldMove;
    private Vector2 _rotation;
private float _groundCheckRadius;
    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    
    private void Awake()
    {
        InitComponents();
        InitSettings();
    }

        private void Start()
    {
        characterController = GetComponent<CharacterController>();
        photonView = GetComponent<PhotonView>();
    }

    #endregion
    /***********************************************************************
    *                               Init Methods
    ***********************************************************************/
    #region .
    private void InitComponents()
    {
        LogNotInitializedComponentError(Com.tpCamera, "TP Camera");
        LogNotInitializedComponentError(Com.fpCamera, "FP Camera");
        TryGetComponent(out Com.rBody);
        Com.anim = GetComponentInChildren<Animator>();

        Com.tpCamObject = Com.tpCamera.gameObject;
        Com.tpRig = Com.tpCamera.transform.parent;
        Com.fpCamObject = Com.fpCamera.gameObject;
        Com.fpRig = Com.fpCamera.transform.parent;
        Com.fpRoot = Com.fpRig.parent;
    }

    private void InitSettings()
    {
        // Rigidbody
        if (Com.rBody)
        {
            // 회전은 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
            Com.rBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Camera
        var allCams = FindObjectsOfType<Camera>();
        foreach (var cam in allCams)
        {
            cam.gameObject.SetActive(false);
        }
        // 설정한 카메라 하나만 활성화
        State.isCurrentFp = (CamOption.initialCamera == CameraType.FpCamera);
        Com.fpCamObject.SetActive(State.isCurrentFp);
        Com.tpCamObject.SetActive(!State.isCurrentFp);
        TryGetComponent(out CapsuleCollider cCol);
    _groundCheckRadius = cCol ? cCol.radius : 0.1f;
    }

    #endregion
    /***********************************************************************
    *                               Checker Methods
    ***********************************************************************/
    #region .
    // Lerp를 위한 변수들
private float _moveX;
private float _moveZ;

private void UpdateAnimationParams()
{
    float x, z;

    if (State.isCurrentFp)
    {
        x = _moveDir.x;
        z = _moveDir.z;

        if (State.isRunning)
        {
            x *= 2f;
            z *= 2f;
        }
    }
    else
    {
        x = 0f;
        z = _moveDir.sqrMagnitude > 0f ? 1f : 0f;

        if (State.isRunning)
        {
            z *= 2f;
        }
    }

    // Lerp
    const float LerpSpeed = 0.05f;
    AnimOption.moveX = Mathf.Lerp(AnimOption.moveX, x, LerpSpeed);
    AnimOption.moveZ = Mathf.Lerp(AnimOption.moveZ, z, LerpSpeed);
    AnimOption.distY = _distFromGround;
    AnimOption.grounded = State.isGrounded;

    if (photonView.IsMine)
    {
        // 로컬 플레이어는 애니메이션을 업데이트합니다
        Com.anim.SetFloat(AnimOption.paramMoveX, AnimOption.moveX);
        Com.anim.SetFloat(AnimOption.paramMoveZ, AnimOption.moveZ);
        Com.anim.SetFloat(AnimOption.paramDistY, AnimOption.distY);
        Com.anim.SetBool(AnimOption.paramGrounded, AnimOption.grounded);
    }
    else
    {
        // 원격 플레이어는 애니메이션 매개변수를 받습니다
        photonView.RPC("SyncAnimationParams", RpcTarget.Others, AnimOption.moveX, AnimOption.moveZ, AnimOption.distY, AnimOption.grounded);
    }
}

[PunRPC]
private void SyncAnimationParams(float moveX, float moveZ, float distY, bool grounded)
{
    // 원격 플레이어는 자체 애니메이션 매개변수를 업데이트합니다
    Com.anim.SetFloat(AnimOption.paramMoveX, moveX);
    Com.anim.SetFloat(AnimOption.paramMoveZ, moveZ);
    Com.anim.SetFloat(AnimOption.paramDistY, distY);
    Com.anim.SetBool(AnimOption.paramGrounded, grounded);
}

    private void LogNotInitializedComponentError<T>(T component, string componentName) where T : Component
    {
        if(component == null)
            Debug.LogError($"{componentName} 컴포넌트를 인스펙터에 넣어주세요");
    }

    #endregion
    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .

    #endregion
    
    private void SetValuesByKeyInput()
{
    float h = 0f, v = 0f;

    if (Input.GetKey(Key.moveForward)) v += 1.0f;
    if (Input.GetKey(Key.moveBackward)) v -= 1.0f;
    if (Input.GetKey(Key.moveLeft)) h -= 1.0f;
    if (Input.GetKey(Key.moveRight)) h += 1.0f;

    Vector3 moveInput = new Vector3(h, 0f, v).normalized;
    _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.acceleration); // 가속, 감속
    _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

    State.isMoving = _moveDir.sqrMagnitude > 0.01f;
    State.isRunning = Input.GetKey(Key.run);
}

/// <summary> 1인칭 회전 </summary>
private void Rotate()
{
    if (State.isCurrentFp)
    {
        if (!State.isCursorActive)
            RotateFP();
    }
    else
    {
        if (!State.isCursorActive)
            RotateTP();
        RotateFPRoot();
    }

    if (photonView.IsMine)
    {
        // 로컬 플레이어의 회전 정보를 전송
        photonView.RPC("SyncRotation", RpcTarget.Others, Com.fpRig.localEulerAngles, Com.fpRoot.localEulerAngles);
    }
}

[PunRPC]
private void SyncRotation(Vector3 fpRigRotation, Vector3 fpRootRotation)
{
    // 원격 플레이어의 회전 정보를 수신하여 적용
    Com.fpRig.localEulerAngles = fpRigRotation;
    Com.fpRoot.localEulerAngles = fpRootRotation;
}

/// <summary> 1인칭 회전 </summary>
private void RotateFP()
{
    float deltaCoef = Time.deltaTime * 50f;

    // 상하 : FP Rig 회전
    float xRotPrev = Com.fpRig.localEulerAngles.x;
    float xRotNext = xRotPrev + _rotation.y
        * CamOption.rotationSpeed * deltaCoef;

    if (xRotNext > 180f)
        xRotNext -= 360f;

    // 좌우 : FP Root 회전
    float yRotPrev = Com.fpRoot.localEulerAngles.y;
    float yRotNext =
        yRotPrev + _rotation.x
        * CamOption.rotationSpeed * deltaCoef;

    // 상하 회전 가능 여부
    bool xRotatable =
        CamOption.lookUpDegree < xRotNext &&
        CamOption.lookDownDegree > xRotNext;

    // FP Rig 상하 회전 적용
    Com.fpRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

    // FP Root 좌우 회전 적용
    Com.fpRoot.localEulerAngles = Vector3.up * yRotNext;
}

/// <summary> 3인칭 회전 </summary>
private void RotateTP()
{
    float deltaCoef = Time.deltaTime * 50f;

    // 상하 : TP Rig 회전
    float xRotPrev = Com.tpRig.localEulerAngles.x;
    float xRotNext = xRotPrev + _rotation.y
        * CamOption.rotationSpeed * deltaCoef;

    if (xRotNext > 180f)
        xRotNext -= 360f;

    // 좌우 : TP Rig 회전
    float yRotPrev = Com.tpRig.localEulerAngles.y;
    float yRotNext =
        yRotPrev + _rotation.x
        * CamOption.rotationSpeed * deltaCoef;

    // 상하 회전 가능 여부
    bool xRotatable =
        CamOption.lookUpDegree < xRotNext &&
        CamOption.lookDownDegree > xRotNext;

    Vector3 nextRot = new Vector3
    (
        xRotatable ? xRotNext : xRotPrev,
        yRotNext,
        0f
    );

    // TP Rig 회전 적용
    Com.tpRig.localEulerAngles = nextRot;
}

/// <summary> 3인칭일 경우 FP Root 회전 </summary>
private void RotateFPRoot()
{
    if (State.isMoving == false) return;

    Vector3 dir = Com.tpRig.TransformDirection(_moveDir);
    float currentY = Com.fpRoot.localEulerAngles.y;
    float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

    if (nextY - currentY > 180f) nextY -= 360f;
    else if (currentY - nextY > 180f) nextY += 360f;

    Com.fpRoot.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);
}

private void Move()
{
    // 이동하지 않는 경우, 미끄럼 방지
    if (State.isMoving == false)
    {
        Com.rBody.velocity = new Vector3(0f, Com.rBody.velocity.y, 0f);
        return;
    }

    // 실제 이동 벡터 계산
    // 1인칭
    if (State.isCurrentFp)
    {
        _worldMove = Com.fpRoot.TransformDirection(_moveDir);
    }
    // 3인칭
    else
    {
        _worldMove = Com.tpRig.TransformDirection(_moveDir);
    }

    _worldMove *= (MoveOption.speed) * (State.isRunning ? MoveOption.runningCoef : 1f);

    // Y축 속도는 유지하면서 XZ평면 이동
    Com.rBody.velocity = new Vector3(_worldMove.x, Com.rBody.velocity.y, _worldMove.z);
}
private void ShowCursorToggle()
{
    if (Input.GetKeyDown(Key.showCursor))
        State.isCursorActive = !State.isCursorActive;

    ShowCursor(State.isCursorActive);
}

private void ShowCursor(bool value)
{
    Cursor.visible = value;
    Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
}
private float _distFromGround;
private void CheckDistanceFromGround()
{
    Vector3 ro = transform.position + Vector3.up;
    Vector3 rd = Vector3.down;
    Ray ray = new Ray(ro, rd);
    

    const float rayDist = 500f;
    const float threshold = 0.01f;

    bool cast =
        Physics.SphereCast(ray, _groundCheckRadius, out var hit, rayDist, MoveOption.groundLayerMask);

    _distFromGround = cast ? (hit.distance - 1f + _groundCheckRadius) : float.MaxValue;
    State.isGrounded = _distFromGround <= _groundCheckRadius + threshold;
}

private void Jump()
{
    if (Input.GetKeyDown(Key.jump))
    {
        Com.rBody.AddForce(Vector3.up * MoveOption.jumpForce, ForceMode.VelocityChange);
        Com.anim.SetTrigger(AnimOption.paramJump); // 로컬 플레이어의 애니메이션 트리거

        // 원격 플레이어에게도 점프 애니메이션을 동기화하기 위한 RPC 호출
        photonView.RPC("SyncJumpAnimation", RpcTarget.Others);
    }
}
[PunRPC]
private void SyncJumpAnimation()
{
    Com.anim.SetTrigger(AnimOption.paramJump); // 점프 애니메이션을 트리거합니다.
    Com.anim.SetBool(AnimOption.paramGrounded, true); // Grounded를 true로 설정
}

private void CameraViewToggle()
{
    if (photonView.IsMine && Input.GetKeyDown(Key.switchCamera))
    {
        State.isCurrentFp = !State.isCurrentFp;
        Com.fpCamObject.SetActive(State.isCurrentFp);
        Com.tpCamObject.SetActive(!State.isCurrentFp);

        if (State.isCurrentFp)
        {
            // 로컬 플레이어에만 적용
            Vector3 tpEulerAngle = Com.tpRig.localEulerAngles;
            Com.fpRig.localEulerAngles = Vector3.right * tpEulerAngle.x;
            Com.fpRoot.localEulerAngles = Vector3.up * tpEulerAngle.y;
        }
    }
}

private void Update()
{
    if (photonView.IsMine)
    {
        //ShowCursorToggle();
        CameraViewToggle();
        SetValuesByKeyInput();
        CheckDistanceFromGround();

        Rotate();
        Move();
        Jump();

        UpdateAnimationParams();
    }
    else
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, 500 * Time.deltaTime);
    }
}

public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        // 데이터를 다른 플레이어에게 전송
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation); // 회전 정보도 전송
    }
    else
    {
        // 네트워크에서 데이터 수신
        networkPosition = (Vector3)stream.ReceiveNext();
        networkRotation = (Quaternion)stream.ReceiveNext(); // 회전 정보도 수신
    }
}

}