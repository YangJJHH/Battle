using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonObitCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f,0.5f,-2.0f);

    public float smooth = 10f; //카메라 반응속도
    public float horizontalAimingSpeed = 6.0f; //수평 회전 속도
    public float verticalAimingSpeed = 6.0f; //수직 회전 속도
    public float maxVerticalAngle = 30.0f; //최대 수직 회전각
    public float minVerticalAngle = -60.0f;
    public float recoilAngleBounce = 5.0f; //사격반동값
    private float angleH = 0.0f; //마우스 이동에 따른 수평이동 수치
    private float angleV = 0.0f;
    private Transform cameraTransform;
    private Camera myCamera;
    private Vector3 relCameraPos; //플레이어로부터 카메라까지의 벡터
    private float relCameraPosMag; //플레이어로부터 카메라까지의 거리
    private Vector3 smoothPivotOffset; //카메라 피봇용 보간 벡터
    private Vector3 smoothCamOffset; //카메라 위치용 보간 벡터
    private Vector3 targetPivotOffset; //카메라 위치용 보간 벡터
    private Vector3 targetCamOffset;
    private float defaultFOV; //기본 시야각
    private float targetFOV;
    private float targetMaxVerticalAngle; //카메라 수직 최대 각도.
    private float recoilAngle = 0f;

    public float GetH { get => angleH; }

    void Awake()
    {
        cameraTransform = transform;
        myCamera = cameraTransform.GetComponent<Camera>();
        cameraTransform.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cameraTransform.rotation = Quaternion.identity;

        //카메라와 플레이어간의 상대벡터, 충돌체크 시 사용
        relCameraPos = cameraTransform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f;

        //기본세팅
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
        angleH = player.eulerAngles.y;

        ResetFOV();
        ResetTargetOffset();
        ResetMaxVerticalAngle();
    }

    public void ResetTargetOffset()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }

    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }

    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticalAngle = maxVerticalAngle;
    }

    public void BounceVertical(float degree)
    {
        recoilAngle = degree;
    }

    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }

    public void SetFOV(float customFOV)
    {
        targetFOV = customFOV;
    }
}
