using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class MoveAnimation
{
    public bool toggle;
    public Vector3 startValue;
    public Vector3 endValue;
    public float duration = 1f;
    public float delay = 0f;
    public Ease ease = Ease.Linear;
}