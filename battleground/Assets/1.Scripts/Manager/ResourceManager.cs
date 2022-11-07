using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityObject = UnityEngine.Object;
/// <summary>
/// Resouces.Load를 래핑하는 클래스.
/// 나중에 에셋번들로 변경하기 위해서.
/// </summary>
public class ResourceManager
{
   public static UnityObject Load(string path)
    {
        return Resources.Load(path);
    }
    public static GameObject LoadAndInstantiate(string path)
    {
        UnityObject source = Load(path);
        if (source == null)
        {
            return null;
        }
        return GameObject.Instantiate(source) as GameObject;
    }
}
