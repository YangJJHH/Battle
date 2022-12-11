﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 마우스 오른쪽 버튼으로 조준. 다른 동작을 대체해서 동작하게 됩니다.
/// 마우스 휠버튼으로 좌우 카메라 변경
/// 벽의 모서리에서 조준할때 상체를 살짝 기울여주는 기능.
/// </summary>
public class AimBehaviour : GenericBehaviour
{
    public Texture2D crossHair;
    public float aimTurnSmoothing = 0.15f;
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f);
    public Vector3 aimcamOffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimBool; //애니메이터 파라미터 조준.
    private bool aim; //조준중?
    private int cornerBool; //애니메이터 관련 코너.
    private bool peekCorner; //플레이거가 코너 모서리에 있는지 여부
    private Vector3 initialRootRotation; //루트 본 으로부터 로컬 회전값;
    private Vector3 initialHipRotation;
    private Vector3 initialSpineRotation;
    private Transform myTransform;

    private void Start()
    {
        //set up
        aimBool = Animator.StringToHash(FC.AnimatorKey.Aim);
        cornerBool = Animator.StringToHash(FC.AnimatorKey.Corner);
        myTransform = transform;

        //value
        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (hips.parent == myTransform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;
    }

    //카메라에 따라 플레이어를 올바른 방향으로 회전.
    void Rotating()
    {
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviourController.GetCamScript.GetH, 0.0f);
        float minSpeed = Quaternion.Angle(myTransform.rotation, targetRotation) * aimTurnSmoothing;

        if (peekCorner)
        {
            //조준 중일때 플레이어 상체만 살짝 기울여 주기 위함
            myTransform.rotation = Quaternion.LookRotation(-behaviourController.GetLastDirection());
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);
            Transform spine = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
            spine.rotation = targetRotation;
        }
        else
        {
            behaviourController.SetLastDirection(forward);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, minSpeed * Time.deltaTime);
        }
    }

    //조준중일때 관리하는 함수
    void AimManageMent()
    {
        Rotating();
    }
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        //조준이 불가능한 상태일 때에 대한 예외처리
        if (behaviourController.GetTempLockState(this.behaviourCode) || behaviourController.IsOverriding(this))
        {
            yield return false;
        }
        else
        {
            aim = true;
            int signal = 1;
            if (peekCorner)
            {
                signal = (int)Mathf.Sign(behaviourController.GetH);
            }
            aimcamOffset.x = Mathf.Abs(aimcamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(speedFloat, 0.0f);
            behaviourController.OverrideWithBehaviour(this);
        }
    }

    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCamScript.ResetTargetOffset();
        behaviourController.GetCamScript.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.1f);
        behaviourController.RevokeOverridingBehaviour(this);
    }

    public override void LocalFixedUpdate()
    {
        if (aim)
        {
            behaviourController.GetCamScript.SetTargetOffset(aimPivotOffset, aimcamOffset);
        }
    }

    public override void LocalLateUpdate()
    {
        AimManageMent();
    }

    private void Update()
    {
        peekCorner = behaviourController.GetAnimator.GetBool(cornerBool);
        if (Input.GetAxisRaw(ButtonName.Aim) != 0 && !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if (aim && Input.GetAxisRaw(ButtonName.Aim) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }
        canSprint = !aim;
        if (aim && Input.GetButtonDown(ButtonName.Shoulder) && !peekCorner)
        {
            aimcamOffset.x = aimcamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }
        behaviourController.GetAnimator.SetBool(aimBool,aim);
    }

    private void OnGUI()
    {
        if(crossHair != null)
        {
            float length = behaviourController.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);
            if(length < 0.05f) 
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crossHair.width * 0.5f),Screen.height * 0.5f - (crossHair.height * 0.5f),crossHair.width,crossHair.height),crossHair);
            }
        }
    }

}
