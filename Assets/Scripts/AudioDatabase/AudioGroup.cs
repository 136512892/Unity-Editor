using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音频数据组
/// </summary>
public class AudioGroup : ScriptableObject
{
    /// <summary>
    /// 音频数据组ID
    /// </summary>
    public int id;
    /// <summary>
    /// 列表存储组中所有的音频数据
    /// </summary>
    public List<AudioData> datas = new List<AudioData>(0);

    public AudioClip this[int id]
    {
        get
        {
            int index = datas.FindIndex(m => m.id == id);
            return index != -1 ? datas[index].clip : null;
        }
    }
}