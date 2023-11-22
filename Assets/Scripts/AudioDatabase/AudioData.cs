using System;
using UnityEngine;

/// <summary>
/// 音频数据
/// </summary>
[Serializable]
public class AudioData
{
    /// <summary>
    /// 音频数据ID
    /// </summary>
    public int id;
    /// <summary>
    /// 音频剪辑资产
    /// </summary>
    public AudioClip clip;
}