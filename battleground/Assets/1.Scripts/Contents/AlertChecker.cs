using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertChecker : MonoBehaviour
{
    [Range(0,50)]public float alertRadius;
    public int extraWaves = 1;
    public LayerMask alertMask = TagAndLayer.LayerMasking.Enemy;
    private Vector3 current;
    private bool alert;

    private void Start()
    {
        //특정함수를 특정주기로 반복해주는 함수 (코루틴과 다름)
        InvokeRepeating("PingAlert", 1, 1);
    }

    //특정 위치 주변으로 alert을 보낸다
    private void AlertNearBy(Vector3 origin, Vector3 target, int wave= 0)
    {
       if(wave > this.extraWaves)
        {
            return;
        }

        Collider[] targetsInViewRadius = Physics.OverlapSphere(origin, alertRadius, alertMask);

        foreach(Collider obj in targetsInViewRadius)
        {
            obj.SendMessage("AlertCallback",target,SendMessageOptions.DontRequireReceiver);
            AlertNearBy(obj.transform.position, target, wave + 1);
        }
    }

    public void RootAlertNearBy(Vector3 origin)
    {
        current = origin;
        alert = true;
    }

    void PingAlert()
    {
        if (alert)
        {
            alert = false;
            AlertNearBy(current,current);
        }
    }
}
