using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 
/// state -> actions update -> transition check
/// state에 필요한 기능들. 애니메이션 콜백들.
/// 시야 체크 , 찾아논 엄폐물 장소중 가장 가까운 위치를 찾는 기능
/// </summary>
public class StateController : MonoBehaviour
{
    public GenericStats genericStats;
    public ClassStats statData;
    public string classID;
    public ClassStats.Param classStats 
    {
        get
        {
            foreach(ClassStats.Sheet sheet in statData.sheets)
            {
                foreach (ClassStats.Param param in sheet.list)
                {
                    if (param.ID.Equals(classID))
                    {
                        return param;
                    }
                }
            }
            return null;
        }
    }

    public State currentState;
    public State remainState;

    public Transform aimTarget;
    public List<Transform> patrolWayPoints;
    public int bullets;
    [Range(0,50)]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    [Range(0, 25)]
    public float perceptionRadius;

    [HideInInspector]public float nearRadius;
    [HideInInspector]public NavMeshAgent nav;
    [HideInInspector]public int wayPointIndex;
    [HideInInspector]public int maximumBurst = 7;

    [HideInInspector]public float blindEngageTime = 30f; //대상이 시야에서 멀어지고 일정시간동안 대상을 찾는시간
    [HideInInspector]public bool targetInsight;
    [HideInInspector]public bool focusSight;
    [HideInInspector]public bool hadClearshot; // 과거에
    [HideInInspector]public bool haveClearShot; //현재
    [HideInInspector]public int coverHash = -1; //각각 다른 장애물에 숨기위한 고유 해쉬값
    
    [HideInInspector]public EnemyVariables variables;
    [HideInInspector]public Vector3 personalTarget = Vector3.zero;

    private int magBullets;
    private bool aiActive;
    private static Dictionary<int, Vector3> coverSpot; // ai들이 공유할 장애물 점유상황
    private bool straging;
    private bool aiming;
    private bool checkedOnLoop, blockedSight;

    [HideInInspector] public EnemyAnimation enemyAnimation;
    [HideInInspector] public CoverLookUp coverLookUp;

    public Vector3 CoverSpot 
    {
        get { return coverSpot[GetHashCode()]; } //ai마다 고유한 해쉬코드
        set { coverSpot[GetHashCode()] = value; }
    }

    public void TransitionToState(State nextState, Decision decision)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }
}
