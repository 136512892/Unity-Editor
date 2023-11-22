using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurvePath))]
public class BezierCurvePathEditor : Editor
{
    private BezierCurvePath path;
    private const float sphereHandleCapSize = .2f;

    private void OnEnable()
    {
        path = target as BezierCurvePath;
    }

    private void OnSceneGUI()
    {
        //路径点集合为空
        if (path.points == null || path.points.Count == 0) return;
        //当前选中工具非移动工具
        if (Tools.current != Tool.Move) return;
        //颜色缓存
        Color cacheColor = Handles.color;
        Handles.color = Color.yellow;
        //遍历路径点集合
        for (int i = 0; i < path.points.Count; i++)
        {
            Vector3 position = DrawPositionHandle(i);
            Vector3 cp = DrawTangentHandle(i);
            //绘制切线
            Handles.DrawDottedLine(position, cp, 1f);
        }
        //恢复颜色
        Handles.color = cacheColor;
    }

    //路径点操作柄绘制
    private Vector3 DrawPositionHandle(int index)
    {
        BezierCurvePoint point = path.points[index];
        //局部转全局坐标
        Vector3 position = path.transform.TransformPoint(point.position);
        //操作柄的旋转类型
        Quaternion rotation = Tools.pivotRotation == PivotRotation.Local
            ? path.transform.rotation : Quaternion.identity;
        //操作柄的大小
        float size = HandleUtility
            .GetHandleSize(position) * sphereHandleCapSize;
        //在该路径点绘制一个球形
        Handles.color = Color.white;
        Handles.SphereHandleCap(0, position, 
            rotation, size, EventType.Repaint);
        Handles.Label(position, string.Format("Point{0}", index));
        //检测变更
        EditorGUI.BeginChangeCheck();
        //坐标操作柄
        position = Handles.PositionHandle(position, rotation);
        //变更检测结束 如果发生变更 更新路径点
        if (EditorGUI.EndChangeCheck())
        {
            //记录操作
            Undo.RecordObject(path, "Position Changed");
            //全局转局部坐标
            point.position = path.transform.InverseTransformPoint(position);
            //更新路径点
            path.points[index] = point;
        }
        return position;
    }

    //控制点操作柄绘制
    private Vector3 DrawTangentHandle(int index)
    {
        BezierCurvePoint point = path.points[index];
        //局部转全局坐标
        Vector3 cp = path.transform
            .TransformPoint(point.position + point.tangent);
        //操作柄的旋转类型
        Quaternion rotation = Tools.pivotRotation == PivotRotation.Local
            ? path.transform.rotation : Quaternion.identity;
        //操作柄的大小
        float size = HandleUtility
            .GetHandleSize(cp) * sphereHandleCapSize;
        //在该控制点绘制一个球形
        Handles.color = Color.yellow;
        Handles.SphereHandleCap(0, cp, rotation, size, EventType.Repaint);
        //检测变更
        EditorGUI.BeginChangeCheck();
        //坐标操作柄
        cp = Handles.PositionHandle(cp, rotation);
        //变更检测结束 如果发生变更 更新路径点
        if (EditorGUI.EndChangeCheck())
        {
            //记录操作
            Undo.RecordObject(path, "Control Point Changed");
            //全局转局部坐标
            point.tangent = path.transform
                .InverseTransformPoint(cp) - point.position;
            //更新路径点
            path.points[index] = point;
        }
        return cp;
    }
}