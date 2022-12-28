using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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


}
