using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="PluggableAI/GeneralStats")]
public class GenericStats : ScriptableObject
{
    [Header("Genereal")]
    [Tooltip("NPC정찰 속도 clear state")]
    public float patrolSpeed = 2f;
    [Tooltip("NPC가 따라오는속도")]
    public float chaseSpeed = 5f;
    [Tooltip("npc 회피하는 속도 engage State")]
    public float evadeSpeed = 15f;
    [Tooltip("웨이포인트에서 대기하는 시간")]
    public float patrolWaitTime = 2f;
    [Header("Animation")]
    [Tooltip("장애물 레이어 마스크")]
    public LayerMask obstacleMask;
    [Tooltip("조준시 버벅임을 피하기 위한 최소 앵글")]
    public float angleDeadZone = 5f;
    [Tooltip("속도 댐핑 시간")]
    public float speedDampTime = 0.4f;
    [Tooltip("회전속도에 대한 댐핑시간")]
    public float angularSpeedDampTime = 0.2f;
    [Tooltip("각속도 안에서 각도회전에 따른 반응 시간")]
    public float angleResponseTime = 0.2f;
    [Header("Cover")]
    [Tooltip("캐릭터가 숨을 수 있는 장애물의 최소 높이값")]
    public float aboveCoverHeight = 1.5f;
    [Tooltip("숨을수 있는 장애물 레이어 마스크")]
    public LayerMask coverMask;
    [Tooltip("총을 쏠수 있는 마스크")]
    public LayerMask shotMask;
    [Tooltip("타겟(플레이어)")]
    public LayerMask targetMask;

}
