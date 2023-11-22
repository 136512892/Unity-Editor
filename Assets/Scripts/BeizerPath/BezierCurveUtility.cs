using UnityEngine;

public class BezierCurveUtility
{
    /// <summary>
    /// 一阶贝塞尔曲线
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">终点</param>
    /// <param name="t">[0,1]</param>
    /// <returns></returns>
    public static Vector3 Bezier1(Vector3 p0, Vector3 p1, float t)
    {
        return (1 - t) * p0 + t * p1;
    }

    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点</param>
    /// <param name="p2">终点</param>
    /// <param name="t">[0,1]</param>
    /// <returns></returns>
    public static Vector3 Bezier2(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 p0p1 = (1 - t) * p0 + t * p1;
        Vector3 p1p2 = (1 - t) * p1 + t * p2;
        return (1 - t) * p0p1 + t * p1p2;
    }

    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">终点</param>
    /// <param name="t">[0,1]</param>
    /// <returns></returns>
    public static Vector3 Bezier3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 p0p1 = (1 - t) * p0 + t * p1;
        Vector3 p1p2 = (1 - t) * p1 + t * p2;
        Vector3 p2p3 = (1 - t) * p2 + t * p3;
        Vector3 p0p1p2 = (1 - t) * p0p1 + t * p1p2;
        Vector3 p1p2p3 = (1 - t) * p1p2 + t * p2p3;
        return (1 - t) * p0p1p2 + t * p1p2p3;
    }
}