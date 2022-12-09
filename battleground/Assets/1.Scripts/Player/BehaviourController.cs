using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

/// <summary>
/// 현재 동작, 기본동작, 오버라이딩 동작, 잠긴 동작, 마우스 이동값
/// 땅에 서있는지 , GenericBehaviour를 상속받은 동작들을 업데이트 시켜준다.
/// </summary>
public class BehaviourController : MonoBehaviour
{
    #region ================field===================
    private List<GenericBehaviour> behaviours; //동작들
    private List<GenericBehaviour> overrideBehaviours; //우선시 되는 동작
    private int currentBehaviour; //현재동작 해시코드
    private int defaultBehaviour; //기동작 해시코드
    private int behaviourLocked;  //잠긴동작 해시코드
    //캐싱
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThirdPersonObitCamera camScript;
    private Transform myTransform;

    //
    private float h;
    private float v;
    public float turnSmoothing = 0.06f; //카메라를 향하도록 움직일때 회전속도.
    private bool changedFOV; //달리기 동작이 카메라 시야각이 변경되었을때 저장되었나.
    public float sprintFOV = 100f;
    private Vector3 lastDirection;
    private bool sprint;
    private int hFloat; //애니메이터용 가로축 값
    private int vFloat;
    private int groundedBool; //애니메이터 지상에 있는가
    private Vector3 colExtents; //땅과의 충돌체크를 위한 충돌체크 영역

    public float GetH { get => h; }
    public float GetV { get => v; }
    public ThirdPersonObitCamera GetCamScript { get => camScript; }
    public Rigidbody GetRigidbody { get => myRigidbody; }
    public Animator GetAnimator { get => myAnimator; }
    public int GetDefaultBehaviour { get => defaultBehaviour; }
    #endregion
    #region ================캐싱======================
    private void Awake()
    {

        behaviours = new List<GenericBehaviour>();
        overrideBehaviours = new List<GenericBehaviour>();
        myAnimator = GetComponent<Animator>();
        hFloat = Animator.StringToHash(FC.AnimatorKey.Horizontal);
        vFloat = Animator.StringToHash(FC.AnimatorKey.Vertical);
        camScript = playerCamera.GetComponent<ThirdPersonObitCamera>();
        myRigidbody = GetComponent<Rigidbody>();
        myTransform = transform;
        //ground?
        groundedBool = Animator.StringToHash(FC.AnimatorKey.Grounded);
        colExtents = GetComponent<Collider>().bounds.extents;

    }
    #endregion
    public bool IsMoving()
    {
        // 부동소수점 문제 때문에 h != 0 과 같은 식을 사용하지 않고 아래처럼 사용, Epsilon은 실수가 갖을 수 있는 가장 작은값
        return (Mathf.Abs(h) > Mathf.Epsilon) || (Mathf.Abs(v) > Mathf.Epsilon);
    }
    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon;
    }
    public bool CanSprint()
    {
        foreach (GenericBehaviour behaviour in behaviours)
        {
            if (!behaviour.AllowSprint)
            {
                return false;
            }
        }
        foreach (GenericBehaviour behaviour in overrideBehaviours)
        {
            if (!behaviour.AllowSprint)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint();
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(myTransform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
    }

    private void Update()
    {
        h = Input.GetAxis("Horiziontal");
        v = Input.GetAxis("Vertiacl");

        myAnimator.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        myAnimator.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        sprint = Input.GetButton(ButtonName.Sprint);

        if (IsSprinting())
        {
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
        }
        else if (changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
        }
        myAnimator.SetBool(groundedBool, IsGrounded());
    }

    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(myRigidbody.rotation, targetRotation, turnSmoothing);
            myRigidbody.MoveRotation(newRotation);

        }
    }

    void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;
        if (behaviourLocked > 0 || overrideBehaviours.Count == 0)
        {
            foreach(GenericBehaviour behaviour in behaviours)
            {
                if(behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    isAnyBehaviourActive = true;
                    behaviour.LocalFixedUpdate();
                }
            }
        }
        else
        {
            foreach(GenericBehaviour behaviour in overrideBehaviours)
            {
                behaviour.LocalFixedUpdate();
            }
        }
        if(!isAnyBehaviourActive && overrideBehaviours.Count == 0)
        {
            myRigidbody.useGravity = true;
            Repositioning();
        }
    }

    private void LateUpdate()
    {
        if (behaviourLocked > 0 || overrideBehaviours.Count == 0)
        {
            foreach(GenericBehaviour behaviour in behaviours)
            {
                if(behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode)
                {
                    behaviour.LocalLateUpdate();
                }
            }
        }
        else
        {
            foreach (GenericBehaviour behaviour in overrideBehaviours)
            {
                behaviour.LocalLateUpdate();
            }
        }
    }

    public void SubScribeBehaviour(GenericBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }
    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }
    public void RegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == defaultBehaviour)
        {
            currentBehaviour = behaviourCode;
        }
    }

    public void UnRegisterBehaviour(int behaviourCode)
    {
        if(currentBehaviour == behaviourCode)
        {
            currentBehaviour = defaultBehaviour;
        }
    }

    public bool OverrideWithBehaviour(GenericBehaviour behaviour)
    {
        if (!overrideBehaviours.Contains(behaviour))
        {
            if(overrideBehaviours.Count == 0)
            {
                foreach(GenericBehaviour behaviour1 in behaviours)
                {
                    if(behaviour1.isActiveAndEnabled && currentBehaviour == behaviour1.GetBehaviourCode)
                    {
                        behaviour1.OnOverride();
                        break;
                    }
                }
            }
            overrideBehaviours.Add(behaviour);
            return true;
        }
        return false;
    }

    public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
    {
        if (overrideBehaviours.Contains(behaviour))
        {
            overrideBehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    public bool IsOverriding(GenericBehaviour behaviour = null)
    {
        if(behaviour == null)
        {
            return overrideBehaviours.Count > 0;
        }
        return overrideBehaviours.Contains(behaviour);
    }

    public bool isCurrentBehaviour(int behaviour)
    {
        return this.currentBehaviour == behaviour;
    }

    public bool GetTempLockState(int behaviourCode = 0) 
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCode);
    }

    public void LockTempBehaviour(int behaviourCode)
    {
        if(behaviourLocked == 0)
        {
            behaviourLocked = behaviourCode;
        }
    }

    public void UnLockTempBehaviour(int behaviourCode)
    {
        if(behaviourLocked == behaviourCode)
        {
            behaviourLocked = 0;
        }
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }

    public void SetLastDirection(Vector3 dir)
    {
        lastDirection = dir;
    }
}

public abstract class GenericBehaviour : MonoBehaviour
{
    protected int speedFloat;
    protected BehaviourController behaviourController;
    protected int behaviourCode;
    protected bool canSprint;

    private void Awake()
    {
        this.behaviourController = GetComponent<BehaviourController>();
        // Set("문자열") 같은 경우는 성능상 좋지 않기때문에 미리 StringHash로 캐싱한다.
        speedFloat = Animator.StringToHash(FC.AnimatorKey.Speed);
        canSprint = true;
        //동작 타입을 해시코드로 가지고 있다가 추후에 구별용으로
        behaviourCode = this.GetType().GetHashCode();
    }

    public int GetBehaviourCode { get => behaviourCode; }
    public bool AllowSprint { get => canSprint; }

    public virtual void LocalLateUpdate()
    {

    }

    public virtual void LocalFixedUpdate()
    {

    }

    public virtual void OnOverride()
    {

    }
}

