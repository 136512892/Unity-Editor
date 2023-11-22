using UnityEngine;
using System.Collections.Generic;

public class BezierCurvePath : MonoBehaviour
{
    /// <summary>
    /// 段数
    /// </summary>
    [Range(1, 100)] public int segments = 10;

    /// <summary>
    /// 是否循环
    /// </summary>
    public bool loop;

    /// <summary>
    /// 点集合
    /// </summary>
    public List<BezierCurvePoint> points = new List<BezierCurvePoint>(2)
    {
        new BezierCurvePoint() 
        {
            position = Vector3.back * 5f, 
            tangent = Vector3.back * 5f + Vector3.left * 3f
        },
        new BezierCurvePoint() 
        { 
            position = Vector3.forward * 5f, 
            tangent = Vector3.forward * 5f + Vector3.right * 3f
        }
    };

    /// <summary>
    /// 根据归一化位置值获取对应的贝塞尔曲线上的点
    /// </summary>
    /// <param name="t">归一化位置值 [0,1]</param>
    /// <returns></returns>
    public Vector3 EvaluatePosition(float t)
    {
        Vector3 retVal = Vector3.zero;
        if (points.Count > 0)
        {
            float max = points.Count - 1 < 1 ? 0 
                : (loop ? points.Count : points.Count - 1);
            float standardized = (loop && max > 0) ? ((t %= max) 
                + (t < 0 ? max : 0)) : Mathf.Clamp(t, 0, max);
            int rounded = Mathf.RoundToInt(standardized);
            int i1, i2;
            if (Mathf.Abs(standardized - rounded) < Mathf.Epsilon)
                i1 = i2 = (rounded == points.Count) ? 0 : rounded;
            else
            {
                i1 = Mathf.FloorToInt(standardized);
                if (i1 >= points.Count)
                {
                    standardized -= max;
                    i1 = 0;
                }
                i2 = Mathf.CeilToInt(standardized);
                i2 = i2 >= points.Count ? 0 : i2;
            }
            retVal = i1 == i2 ? points[i1].position 
                : BezierCurveUtility.Bezier3(points[i1].position,
                points[i1].position + points[i1].tangent, 
                points[i2].position - points[i2].tangent, 
                points[i2].position, standardized - i1);
        }
        return retVal;
    }

    /// <summary>
    /// 路径颜色(Gizmos)
    /// </summary>
    public Color pathColor = Color.green;

    private void OnDrawGizmos()
    {
        if (points.Count == 0) return;
        //缓存颜色
        Color cacheColor = Gizmos.color;
        //路径绘制颜色
        Gizmos.color = pathColor;
        //步长
        float step = 1f / segments;
        //缓存上个坐标点
        Vector3 lastPos = transform
            .TransformPoint(EvaluatePosition(0f));
        float end = (points.Count - 1 < 1 ? 0 
            : (loop ? points.Count : points.Count - 1)) + step * .5f;
        for (float t = step; t <= end; t += step)
        {
            //计算位置
            Vector3 p = transform.TransformPoint(EvaluatePosition(t));
            //绘制曲线
            Gizmos.DrawLine(lastPos, p);
            //记录
            lastPos = p;
        }
        //恢复颜色
        Gizmos.color = cacheColor;
    }
}