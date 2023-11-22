using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音频数据库
/// </summary>
[CreateAssetMenu]
public class AudioDatabase : ScriptableObject
{
    /// <summary>
    /// 列表存储所有的音频数据组
    /// </summary>
    public List<AudioGroup> groups = new List<AudioGroup>(0);

    public AudioGroup this[int id]
    {
        get
        {
            return groups.Find(m => m.id == id);
        }
    }
}