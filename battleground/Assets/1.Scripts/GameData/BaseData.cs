using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  data의 기본 클래스 입니다.
///  공통적인 데이터를 가지고 있게 되고, 이름만 현재 갖고있다
///  데이터의 갯수와 이름의 목록 리스트를 얻을 수 있다.
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/9.ResourcesData/Resources/Data/";
    public string[] names = null;

    public BaseData() { }

    public int GetDataCount()
    {
        int retValue = 0;
        if (this.names != null)
        {
            retValue = this.name.Length;
        }
        return retValue;
    }

    /// <summary>
    /// 툴에 출력하기 위한 이름 목록을 만들어주는 함수.
    /// fillterWord는 필터링할 단어.
    /// </summary>
    /// <returns></returns>
    public string[] GetNameList(bool showID, string fillterWord = "")
    {
        string[] retList = new string[0];
        if (this.names == null) return retList;

        retList = new string[this.names.Length];

        for (int i = 0; i < this.names.Length; i++)
        {
            if (fillterWord != "")
            {
                if (names[i].ToLower().Contains(fillterWord.ToLower()) == false)
                {
                    continue;
                }
            }
            if (showID)
            {
                retList[i] = i.ToString() + " : " + this.names[i];
            }
            else
            {
                retList[i] = this.names[i];
            }
        }

        return retList;
    }

    public virtual int AddData(string newName)
    {
        return GetDataCount();
    }
    public virtual void RemoveData(int index)
    {

    }
    public virtual void Copy(int index)
    {

    }
}
