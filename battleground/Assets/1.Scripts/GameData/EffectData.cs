using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.IO;
/// <summary>
/// 이펙트 클립 리스트와 이펙트 파일 이름과 경로를 가지고 있으며 파일을 읽고 쓰는 기능을 가지고 있다.
/// </summary>
public class EffectData : BaseData
{
    public EffectClip[] effectClips = new EffectClip[0];

    public string clipPath = "Effects/";
    private string xmlFilePath = "";
    private string xmlFileName = "effectData.xml";
    private string dataPath = "Data/effectData";
    //XML 구분자
    private const string EFFECT = "effect"; //저장키
    private const string CLIP = "clip";

    private EffectData() { }
    // 읽어오고 저장하고, 데이터를 삭제하고 , 특정클립을 얻어오고, 복사하는 기능

    public void LoadData()
    {
        xmlFilePath = Application.dataPath + dataDirectory;
        TextAsset asset = (TextAsset)ResourceManager.Load(dataPath);
        if(asset == null || asset.text == null)
        {
            AddData("New Effect");
            return;
        }

        using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
        {
            int currentID = 0;
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "length":
                            int length = int.Parse(reader.ReadString());
                            names = new string[length];
                            effectClips = new EffectClip[length];
                            break;

                        case "id":
                            currentID = int.Parse(reader.ReadString());
                            effectClips[currentID] = new EffectClip();
                            effectClips[currentID].realId = currentID;
                            break;

                        case "name":
                            names[currentID] = reader.ReadString();
                            break;

                        case "effectType":
                            effectClips[currentID].effectType = (EffectType)Enum.Parse(typeof(EffectType),reader.ReadString());
                            break;

                        case "effectName":
                            effectClips[currentID].effectName = reader.ReadString();
                            break;

                        case "effectPath":
                            effectClips[currentID].effctPath = reader.ReadString();
                            break;
                    }
                }
            }
        }
    }

    public void SaveData()
    {
        using (XmlTextWriter xml = new XmlTextWriter(xmlFilePath + xmlFileName, System.Text.Encoding.Unicode))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement(EFFECT);
            xml.WriteElementString("length", GetDataCount().ToString());
            for(int i=0; i<names.Length; i++)
            {
                EffectClip clip = effectClips[i];
                xml.WriteStartElement(CLIP);
                xml.WriteElementString("id",i.ToString());
                xml.WriteElementString("name", names[i]);
                xml.WriteElementString("effectType",clip.effectType.ToString());
                xml.WriteElementString("effectPath", clip.effctPath);
                xml.WriteElementString("effectName",clip.effectName);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.WriteEndDocument();
        }
    }

    public override int AddData(string newName)
    {
        if(names == null)
        {
            names = new string[] { newName };
            effectClips = new EffectClip[] { new EffectClip() };
        }
        else
        {
            names = ArrayHelper.Add(newName, names);
            effectClips = ArrayHelper.Add(new EffectClip(), effectClips);
        }
        return GetDataCount();
    }

    public override void RemoveData(int index)
    {
        names = ArrayHelper.Remove(index,names);
        if(names.Length == 0)
        {
            names = null;
        }
        effectClips = ArrayHelper.Remove(index, effectClips);
    }

    public void ClearData()
    {
        foreach(EffectClip clip in effectClips)
        {
            clip.ReleaseEffect();
        }
        effectClips = null;
        names = null;
    }

    public EffectClip GetCopy(int index)
    {
        if(index<0 || index >= effectClips.Length)
        {
            return null;
        }
        EffectClip original = effectClips[index];
        EffectClip clip = new EffectClip();
        clip.effectFullPath = original.effectFullPath;
        clip.effectName = original.effectName;
        clip.effectType = original.effectType;
        clip.effctPath = original.effctPath;
        clip.realId = effectClips.Length;
        return clip;
    }
    /// <summary>
    /// 원하는 인덱스를 프리로딩해서 찾아준다
    /// </summary>
    public EffectClip GetClip(int index)
    {
        if(index<0 || index>= effectClips.Length)
        {
            return null;
        }
        effectClips[index].PreLoad();
        return effectClips[index];
    }

    public override void Copy(int index)
    {
        names = ArrayHelper.Add(names[index], names);
        effectClips = ArrayHelper.Add(GetCopy(index),effectClips);
    }
}
