using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class RotateAnimation
{
    public bool toggle;
    public Vector3 startValue;
    public Vector3 endValue;
    public float duration = 1f;
    public float delay = 0f;
    public Ease ease = Ease.Linear;
    public RotateMode mode = RotateMode.Fast;
}