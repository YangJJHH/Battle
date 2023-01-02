using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 숨을 만한 곳을 찾아주는 컴포넌트
/// 플레이어보다 멀리있는 건 제외
/// </summary>
public class CoverLookUp : MonoBehaviour
{
    private List<Vector3[]> allCoverSpots;
    private GameObject[] covers;
    private List<int> coverHashCodes; //cover Unity Id

    private Dictionary<float, Vector3> filteredSpots; //추가되면 필터링

    /// <summary>
    /// 현재 씬에 있는 오브젝트중 해당 레이어에 해당되는것들을 가져옴
    /// </summary>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    private GameObject[] GetObjectsInLayerMask(int layerMask)
    {
        List<GameObject> ret = new List<GameObject>();

        foreach(GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if(go.activeInHierarchy && layerMask == (layerMask | (1 << go.layer)))
            {
                ret.Add(go);
            }
        }
        return ret.ToArray();
    }

    private void ProcessPoint(List<Vector3> vector3s, Vector3 navtivePoint, float range)
    {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(navtivePoint, out hit, range, NavMesh.AllAreas))
        {
            vector3s.Add(hit.position);
        }
    }

    private Vector3[] GetSpots(GameObject go, LayerMask obstacleMask)
    {
        List<Vector3> bounds = new List<Vector3>();
        foreach(Collider col in go.GetComponents<Collider>())
        {
            float baseHeight = (col.bounds.center - col.bounds.extents).y;
            float range = 2 * col.bounds.extents.y;

            Vector3 deslocalForward = go.transform.forward * go.transform.localScale.z * 0.5f;
            Vector3 deslocalRight = go.transform.right * go.transform.localScale.x * 0.5f;

            if (go.GetComponent<MeshCollider>())
            {
                float maxBounds = go.GetComponent<MeshCollider>().bounds.extents.z + go.GetComponent<MeshCollider>().bounds.extents.x;
                Vector3 originForward = col.bounds.center + go.transform.forward * maxBounds;
                Vector3 originRight = col.bounds.center + go.transform.right * maxBounds;
                if(Physics.Raycast(originForward,col.bounds.center - originForward, out RaycastHit hit, maxBounds,obstacleMask))
                {
                    deslocalForward = hit.point - col.bounds.center;
                }
                if(Physics.Raycast(originRight,col.bounds.center - originRight, out hit, maxBounds, obstacleMask))
                {
                    deslocalRight = hit.point + col.bounds.center;
                }
            }

            else if(Vector3.Equals(go.transform.localScale, Vector3.one))
            {
                deslocalForward = go.transform.forward * col.bounds.extents.z;
                deslocalRight = go.transform.right * col.bounds.extents.x;
            }

            float edgeFactor = 0.75f;
            ProcessPoint(bounds, col.bounds.center + deslocalRight + deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward + deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward, range);
            ProcessPoint(bounds, col.bounds.center + deslocalForward - deslocalRight *edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight + deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center + deslocalRight , range);
            ProcessPoint(bounds, col.bounds.center + deslocalRight - deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward + deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward , range);
            ProcessPoint(bounds, col.bounds.center - deslocalForward - deslocalRight * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight - deslocalForward * edgeFactor, range);
            ProcessPoint(bounds, col.bounds.center - deslocalRight , range);


        }
        return bounds.ToArray();
    }
}
