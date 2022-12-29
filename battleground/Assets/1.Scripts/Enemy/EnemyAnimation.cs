using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FC;
public class EnemyAnimation : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public float currentAimingAngleGap;
    [HideInInspector] public Transform gunMuzzle;
    [HideInInspector] public float angularSpeed;

    private StateController controller;
    private NavMeshAgent nav;
    private bool pendingAim; //조준을 기다리는 시간
    private Transform hips, spine; //bone transform
    private Vector3 initialRootRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialHipsRotation;
    private Vector3 lastRotation;
    private float timeCountAim, timeCountGuard;
    private readonly float turnSpeed = 25f; //strafing turn speed

    private void Awake()
    {
        //setup
        controller = GetComponent<StateController>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.updatePosition = false;

        hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        spine = anim.GetBoneTransform(HumanBodyBones.Spine);

        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;

        anim.SetTrigger(AnimatorKey.ChangeWeapon);
        anim.SetInteger(AnimatorKey.Weapon,(int)System.Enum.Parse(typeof(WeaponType),controller.classStats.WeaponType));

        foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
        {
            gunMuzzle = child.Find("muzzle");
            if(gunMuzzle != null)
            {
                break;
            }
        }
        foreach(Rigidbody member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    void SetUp(float speed, float angle, Vector3 strafeDirection)
    {
        angle *= Mathf.Deg2Rad;
        angularSpeed = angle / controller.generalStats.angleResponseTime;

        anim.SetFloat(AnimatorKey.Speed, speed, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(AnimatorKey.AngularSpeed, angularSpeed, controller.generalStats.angularSpeedDampTime, Time.deltaTime);

        anim.SetFloat(AnimatorKey.Horizontal, strafeDirection.x, controller.generalStats.speedDampTime, Time.deltaTime);
        anim.SetFloat(AnimatorKey.Vertical, strafeDirection.z, controller.generalStats.speedDampTime, Time.deltaTime);
    }

    void NavAnimSetup()
    {

    }
}
