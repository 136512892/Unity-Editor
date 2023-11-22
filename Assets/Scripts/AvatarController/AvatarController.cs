using UnityEngine;

/// <summary>
/// Avatar角色控制器
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class AvatarController : MonoBehaviour
{
    #region >> Variables
    //主相机
    [Tooltip("人物控制相机"), SerializeField] private Transform mainCamera;
    //动画组件
    private Animator animator;
    //角色控制器
    private CharacterController cc;

    #region >> Bone
    [Header("Bone")]
#if UNITY_EDITOR
    [SerializeField] private bool boneGizmos = true;
    [SerializeField] private Color boneGizmosColor = Color.cyan;
    [Range(.01f, .5f), SerializeField] private float boneGizmosScale = .05f;
#endif
    //头部
    private Transform head;
    #endregion

    #region >> Movement
    [Header("Movement")]
    [Tooltip("是否启用跑功能"), SerializeField] private bool enableSprint = true;
    [Tooltip("走速度，注意与Animator BlendTree中的Threshold阈值保持一致")]
    [SerializeField] private float walkSpeed = 2f;
    [Tooltip("跑速度，注意与Animator BlendTree中的Threshold阈值保持一致")]
    [SerializeField] private float sprintSpeed = 5.35f;
    [Tooltip("加速快捷键")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    //移动速度
    private float speed;
    //Animator BlendTree中的速度
    private float animBlendSpeed;
    //目标旋转值
    private float targetRotation;
    //旋转速度
    private float rotationSpeed;
    [Tooltip("到达目标旋转值的近似时间")]
    [Range(0.0f, 0.3f), SerializeField]
    private float rotationSmoothTime = .12f;
    [Tooltip("移动速度的插值速度")]
    [SerializeField] private float movementLerpSpeed = 10f;
    #endregion

    #region >> Jump & Gravity & Ground
    [Header("Jump & Gravity & Ground")]
    [Tooltip("是否启用跳跃功能"), SerializeField] private bool enableJump = true;
    [Tooltip("跳跃按键")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [Tooltip("跳跃的高度")]
    [SerializeField] private float jumpHeight = 1.3f;
    [Tooltip("重力大小")]
    [SerializeField] private float gravity = -15f;
    [Tooltip("跳跃冷却时长（落地后需经过该时长才可再次跳跃）")]
    [SerializeField] public float jumpCD = .7f;
    [Tooltip("经过该时长后进入下落状态")]
    [SerializeField] public float fallDelay = .2f;
    //跳跃计时
    private float jumpTimeCounter = .5f;
    //下落计时
    private float fallTimeCounter = .15f;

    //是否处于地面
    private bool isGrounded = true;
    [Tooltip("用于粗糙地面")]
    [SerializeField] private float groundedOffset = -0.14f;
    [Tooltip("地面检测的半径大小，需要与角色控制器的半径匹配")]
    [SerializeField] private float groundCheckRadius = 0.28f;
    [Tooltip("地面检测的层级")]
    [SerializeField] private LayerMask groundLayers = 1;

    //垂直方向上的速度
    private float verticalVelocity;
    private readonly float maxVelocity = 53.0f;
    //斜坡上的速度
    private Vector3 slopeVelocity;
    //缓存斜坡上的速度
    private Vector3 lastSlopeVelocity;
    //当前速度
    private Vector3 currentVelocity;
    //地面法线向量
    private Vector3 groundNormal;
    #endregion

    #region >> Head Tracker
    [Header("Head Tracker")]
    [Tooltip("是否启用Head Tracker"), SerializeField] private bool enableHeadTracker = true;
    [Tooltip("忽略HeadTracker的动画状态标签"), SerializeField] private string ignoreHeadTrackTag = "IgnoreHeadTrack";
    [Tooltip("水平方向上的角度限制"), SerializeField] private Vector2 horizontalAngleLimit = new Vector2(-70f, 70f);
    [Tooltip("垂直方向上的角度限制"), SerializeField] private Vector2 verticalAngleLimit = new Vector2(-60f, 60f);
    [Tooltip("超出限制范围时自动回正"), SerializeField] private bool autoTurnback = true;
    [Tooltip("插值速度"), Range(2f, 10f), SerializeField] private float headTrackerLerpSpeed = 5f;
    private float headHeight; //头部的高度
    private float headAngleX;
    private float headAngleY;
    #endregion
    #endregion

    #region >> Mono
    private void Awake()
    {
        //如果相机未赋值 根据Tag标签获取主相机 如果主相机为空 通过Find查找场景中的相机
        if (mainCamera == null)
            mainCamera = (Camera.main != null ? Camera.main : FindObjectOfType<Camera>()).transform;
        //获取角色控制器组件
        cc = GetComponent<CharacterController>();
        //获取动画组件
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        //通过GetBoneTransform获取身体各部位Transform
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        //获取头部的高度
        headHeight = Vector3.Distance(transform.position, head.position);
    }
    private void FixedUpdate()
    {
        GroundedCheck();
    }
    private void Update()
    {
        JumpAndGravity();
        Movement();
    }
    private void LateUpdate()
    {
        HeadTracker();
    }
    #endregion

    #region >> NonPublic Methods
    //移动
    private void Movement()
    {
        //获取输入值
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //没有任何输入时速度为0 加速键按下时速度为跑的速度 否则为走的速度
        float targetSpeed = input == Vector2.zero ? 0f : (enableSprint && Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed);
        //当前水平方向上的速度
        float currentHorizontalSpeed = new Vector3(cc.velocity.x - lastSlopeVelocity.x, 0f, cc.velocity.z - lastSlopeVelocity.z).magnitude;
        //加速或降速 差值0.1
        if (currentHorizontalSpeed < targetSpeed - .1f || currentHorizontalSpeed > targetSpeed + .1f)
        {
            //插值运算
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * input.magnitude, Time.deltaTime * movementLerpSpeed);
            //保留三位小数
            speed = Mathf.Round(speed * 1000f) * .001f;
        }
        else speed = targetSpeed;

        //Animator BlendTree中的速度 
        animBlendSpeed = Mathf.Lerp(animBlendSpeed, targetSpeed, Time.deltaTime * movementLerpSpeed);
        animBlendSpeed = animBlendSpeed >= .1f ? animBlendSpeed : 0f;

        //移动的方向
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        //输入不为空时 处理方向
        if (input != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationSpeed, rotationSmoothTime), 0f);
        }

        moveDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
        //移动
        //cc.Move(moveDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        Vector3 slopeVelocityToUse = Vector3.zero;
        if (slopeVelocity.magnitude > .001f && speed > .001f)
        {
            Vector3 verticalVel = Vector3.Project(slopeVelocity, transform.up);
            Vector3 slopeVelPlanar = slopeVelocity - verticalVel;
            Vector3 subtract = Vector3.Project(moveDirection.normalized * speed, slopeVelPlanar.normalized);
            Vector3 nextSlopeVelPlanar = slopeVelPlanar - subtract;
            float dot = Vector3.Dot(nextSlopeVelPlanar.normalized, slopeVelPlanar.normalized);
            if (dot > 0) slopeVelocityToUse = dot > 0f ? (nextSlopeVelPlanar + verticalVel) : Vector3.zero;
        }
        else slopeVelocityToUse = slopeVelocity;
        cc.Move(speed * Time.deltaTime * moveDirection.normalized + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime + slopeVelocityToUse * Time.deltaTime);
        lastSlopeVelocity = slopeVelocityToUse;

        //移动动画
        animator.SetFloat(AnimatorParameters.Speed, animBlendSpeed);
    }
    //地面检测
    private void GroundedCheck()
    {
        Vector3 characterVelocity = cc.velocity;
        characterVelocity.y = 0;
        if (characterVelocity.magnitude > 0.01f)
            currentVelocity = characterVelocity.normalized;

        groundNormal = Vector3.up;

        if (verticalVelocity > 0)
        {
            isGrounded = false;
            slopeVelocity = Vector3.Lerp(slopeVelocity, Vector3.zero, Time.deltaTime * 20.0f);
            animator.SetBool(AnimatorParameters.Land, isGrounded);
            return;
        }

        groundCheckRadius = cc.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);

        Vector3 targetSlopeVelocity = Vector3.zero;

        float positiveGroundedOffset = Mathf.Abs(groundedOffset);
        Vector3 origin = transform.position + transform.up * (groundCheckRadius + positiveGroundedOffset);
        float twoTimesPositiveGroundedOffset = positiveGroundedOffset * 2;
        if (Physics.SphereCast(origin, groundCheckRadius, -transform.up, out RaycastHit hit, twoTimesPositiveGroundedOffset, groundLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 scaledCenterOffsetVec = cc.center;
            scaledCenterOffsetVec.x *= transform.localScale.x;
            scaledCenterOffsetVec.y *= transform.localScale.y;
            scaledCenterOffsetVec.z *= transform.localScale.z;
            float scaledHeight = cc.height * transform.localScale.y;
            Vector3 p1 = transform.position + scaledCenterOffsetVec + transform.up * (scaledHeight * 0.5f - groundCheckRadius);
            Vector3 p2 = transform.position + scaledCenterOffsetVec - transform.up * (scaledHeight * 0.5f - groundCheckRadius);

            float penetrationCastRadius = groundCheckRadius * 0.99f;
            float additionalRayDist = groundCheckRadius - penetrationCastRadius;

            Vector3 rayDirDistVec = transform.position - hit.point;
            Vector3 rayDirVec = rayDirDistVec.normalized;
            Vector3 projRayDistVec = Vector3.ProjectOnPlane(rayDirDistVec, transform.up);
            float lengthX = Mathf.Max(projRayDistVec.magnitude, 0.01f);
            float lengthX2 = groundCheckRadius - lengthX;
            Vector3 hypothenuse2 = (lengthX2 / lengthX) * rayDirDistVec + rayDirVec * (additionalRayDist + twoTimesPositiveGroundedOffset);

            if (Physics.CapsuleCast(p1, p2, penetrationCastRadius, rayDirVec, hypothenuse2.magnitude, groundLayers, QueryTriggerInteraction.Ignore))
                isGrounded = true;
            else
            {
                groundNormal = hit.normal;
                float angle = Vector3.Angle(hit.normal, transform.up);

                if (angle > cc.slopeLimit)
                {
                    Vector3 raycastOrigin = transform.position + transform.up * (positiveGroundedOffset + cc.stepOffset) + currentVelocity * groundCheckRadius;
                    if (Physics.Raycast(raycastOrigin, -transform.up, out RaycastHit hit2, twoTimesPositiveGroundedOffset + cc.stepOffset * 2.0f, groundLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (Vector3.Angle(hit2.normal, transform.up) > cc.slopeLimit)
                        {
                            isGrounded = false;
                            Vector3 rightVec = Vector3.Cross(hit.normal, transform.up).normalized;
                            targetSlopeVelocity = Vector3.Cross(hit.normal, rightVec).normalized;
                        }
                        else isGrounded = true;
                    }
                    else
                    {
                        isGrounded = false;
                        Vector3 rightVec = Vector3.Cross(hit.normal, transform.up).normalized;
                        targetSlopeVelocity = Vector3.Cross(hit.normal, rightVec).normalized;
                    }
                }
                else isGrounded = true;

            }
        }
        else isGrounded = false;

        if (targetSlopeVelocity.magnitude > 0.001f)
            slopeVelocity += Mathf.Abs(gravity) * Time.deltaTime * targetSlopeVelocity;
        else
            slopeVelocity = Vector3.Lerp(slopeVelocity, Vector3.zero, Time.deltaTime * 20.0f);
        animator.SetBool(AnimatorParameters.Land, isGrounded);
    }
    //跳跃和重力
    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            //重置下落计时
            fallTimeCounter = fallDelay;

            //停止跳跃和下落动画
            if (animator)
            {
                animator.SetBool(AnimatorParameters.Jump, false);
                animator.SetBool(AnimatorParameters.Fall, false);
            }

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = Mathf.Lerp(gravity, -2.0f, Vector3.Dot(Vector3.up, groundNormal));
            }

            //跳跃
            if (enableJump && Input.GetKeyDown(jumpKey) && jumpTimeCounter <= 0.0f)
            {
                //重置跳跃计时
                jumpTimeCounter = jumpCD;
                //到达指定高度所需的速度 = (高度 * -2 * 重力)的平方根
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //跳跃动画
                animator.SetBool(AnimatorParameters.Jump, true);
            }

            if (jumpTimeCounter >= 0.0f)
                jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            //下落动画
            if (fallTimeCounter >= 0.0f)
                fallTimeCounter -= Time.deltaTime;
            else animator.SetBool(AnimatorParameters.Fall, true);
        }

        //重力
        if (verticalVelocity < maxVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (verticalVelocity > 0)
        {
            Vector3 scaledCenterOffsetVec = cc.center;
            scaledCenterOffsetVec.x *= transform.localScale.x;
            scaledCenterOffsetVec.y *= transform.localScale.y;
            scaledCenterOffsetVec.z *= transform.localScale.z;
            float scaledHeight = cc.height * transform.localScale.y;
            float scaledRadius = cc.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);
            Vector3 origin = transform.position + scaledCenterOffsetVec + transform.up * (scaledHeight * 0.5f - scaledRadius);

            float positiveGroundedOffset = Mathf.Abs(groundedOffset);
            float penetrationCastRadius = scaledRadius * 0.99f;
            float rayDist = scaledRadius - penetrationCastRadius + positiveGroundedOffset;

            if (Physics.SphereCast(origin, penetrationCastRadius, transform.up, out _, rayDist, groundLayers, QueryTriggerInteraction.Ignore))
            {
                jumpTimeCounter = 0.0f;
                verticalVelocity = Mathf.Lerp(verticalVelocity, 0.0f, Time.deltaTime * 10.0f);
            }
        }
    }
    //头部朝向
    private void HeadTracker()
    {
        if (!enableHeadTracker) return;
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //看向的位置
        Vector3 headLookAtPos;
        //该动画忽略HeadTracker(使其不起作用)
        if (animatorStateInfo.IsTag(ignoreHeadTrackTag))
            headLookAtPos = transform.position + transform.up * headHeight + transform.forward;
        else
        {
            Vector3 position = mainCamera.transform.position + mainCamera.transform.forward * 100f;
            if (!autoTurnback) headLookAtPos = position;
            else
            {
                Vector3 direction = position - (transform.position + transform.up * headHeight);
                Quaternion rotation = Quaternion.LookRotation(direction, transform.up);
                Vector3 angle = rotation.eulerAngles - transform.eulerAngles;
                float x = NormalizeAngle(angle.x);
                float y = NormalizeAngle(angle.y);
                bool isInRange = x >= verticalAngleLimit.x && x <= verticalAngleLimit.y
                    && y >= horizontalAngleLimit.x && y <= horizontalAngleLimit.y;
                headLookAtPos = isInRange ? position : (transform.position + transform.up * headHeight + transform.forward);
            }
        }
        Vector3 headPosition = transform.position + transform.up * headHeight;
        Quaternion lookRotation = Quaternion.LookRotation(headLookAtPos - headPosition);
        Vector3 eulerAngles = lookRotation.eulerAngles - transform.rotation.eulerAngles;
        float normalizedX = NormalizeAngle(eulerAngles.x);
        float normalizedY = NormalizeAngle(eulerAngles.y);
        headAngleX = Mathf.Clamp(Mathf.Lerp(headAngleX, normalizedX, Time.deltaTime * headTrackerLerpSpeed), verticalAngleLimit.x, verticalAngleLimit.y);
        headAngleY = Mathf.Clamp(Mathf.Lerp(headAngleY, normalizedY, Time.deltaTime * headTrackerLerpSpeed), horizontalAngleLimit.x, horizontalAngleLimit.y);
        Quaternion rotY = Quaternion.AngleAxis(headAngleY, head.InverseTransformDirection(transform.up));
        head.rotation *= rotY;
        Quaternion rotX = Quaternion.AngleAxis(headAngleX, head.InverseTransformDirection(transform.TransformDirection(Vector3.right)));
        head.rotation *= rotX;

        float NormalizeAngle(float angle)
        {
            if (angle > 180) angle -= 360f;
            else if (angle < -180) angle += 360f;
            return angle;
        }
    }
    #endregion
}

public class AnimatorParameters
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Land = Animator.StringToHash("Land");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Fall = Animator.StringToHash("Fall");
    public static readonly int Crouch = Animator.StringToHash("Crouch");
    public static readonly int LeftFootIKWeight = Animator.StringToHash("LeftFootIKWeight");
    public static readonly int RightFootIKWeight = Animator.StringToHash("RightFootIKWeight");
}