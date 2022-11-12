using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이펙트 프리팹과 경로와 타입등의 속성 데이터를 가지고 있게 되며
/// 프리팹 사전로딩 기능을 갖고 있고 - 풀링을 위한 기능이기도 합니다.
/// 이펙트 인스턴스 기능도 갖고 있으며 - 풀링과 연계해서 사용하기도 합니다.
/// </summary>
public class EffectClip
{
    //추후 속성은 같지만 다른 이펙트 클립이 있을 수 있어서 분별용
    public int realId = 0;

    public EffectType effectType = EffectType.NORMAL;
    public GameObject effectPrefab = null;
    public string effectName = "";
    public string effctPath = "";
    public string effectFullPath = "";
    public EffectClip() { }

    public void PreLoad()
    {
        effectFullPath = effctPath + effectName;
        if (effectFullPath != "" && effectPrefab == null)
        {
            effectPrefab = ResourceManager.Load(effectFullPath) as GameObject;
        }
    }
    public void ReleaseEffect()
    {
        if(effectPrefab != null)
        {
            effectPrefab = null;
        }
    }
    /// <summary>
    /// 원하는 위치에 내가 원하는 이펙트를 pos에 인스턴스 합니다.
    /// </summary>
    public GameObject Instantiate(Vector3 pos)
    {
        if(effectPrefab == null)
        {
            PreLoad();
        }
        if(effectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, pos,Quaternion.identity);
            return effect;
        }
        return null;
    }
}
