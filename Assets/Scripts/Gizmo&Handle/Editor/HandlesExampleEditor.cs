using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HandlesExample))]
public class HandlesExampleEditor : Editor
{
    private HandlesExample example;

    private void OnEnable()
    {
        example = target as HandlesExample;
    }
    private void OnSceneGUI()
    {
        HandleCapExample();
    }
    private void HandleCapExample()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Transform transform = example.transform;
            float size = HandleUtility.GetHandleSize(transform.position);
            Handles.color = Handles.xAxisColor;
            Handles.CircleHandleCap(0, transform.position
                + new Vector3(3f, 0f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.right), size,
                EventType.Repaint);
            Handles.ArrowHandleCap(0, transform.position
                + new Vector3(3f, 0f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.right), size,
                EventType.Repaint);
            Handles.RectangleHandleCap(0, transform.position
                + new Vector3(6f, 0f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.right), size,
                EventType.Repaint);
            Handles.SphereHandleCap(0, transform.position
                + new Vector3(8f, 0f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.right), size,
                EventType.Repaint);
            Handles.color = Handles.yAxisColor;
            Handles.CircleHandleCap(0, transform.position
                + new Vector3(0f, 3f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.up), size,
                EventType.Repaint);
            Handles.ArrowHandleCap(0, transform.position
                + new Vector3(0f, 3f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.up), size,
                EventType.Repaint);
            Handles.RectangleHandleCap(0, transform.position
                + new Vector3(0f, 6f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.up), size,
                EventType.Repaint);
            Handles.SphereHandleCap(0, transform.position
                + new Vector3(0f, 8f, 0f), transform.rotation
                * Quaternion.LookRotation(Vector3.up), size,
                EventType.Repaint);
            Handles.color = Handles.zAxisColor;
            Handles.CircleHandleCap(0, transform.position
                + new Vector3(0f, 0f, 3f), transform.rotation
                * Quaternion.LookRotation(Vector3.forward), size,
                EventType.Repaint);
            Handles.ArrowHandleCap(0, transform.position
                + new Vector3(0f, 0f, 3f), transform.rotation
                * Quaternion.LookRotation(Vector3.forward), size,
                EventType.Repaint);
            Handles.RectangleHandleCap(0, transform.position
                + new Vector3(0f, 0f, 6f), transform.rotation
                * Quaternion.LookRotation(Vector3.forward), size,
                EventType.Repaint);
            Handles.SphereHandleCap(0, transform.position
                + new Vector3(0f, 0f, 8f), transform.rotation
                * Quaternion.LookRotation(Vector3.forward), size,
                EventType.Repaint);
        }
    }
    private void LabelExample()
    {
        Handles.Label(Vector3.left * .5f, "Hello World.");
        Handles.Label(Vector3.right * .5f, "Hellow World.",
            EditorStyles.whiteLabel);
    }
    private void RadiusHandleExample()
    {
        example.radius = Handles.RadiusHandle(
            Quaternion.identity, Vector3.left, example.radius);
        example.radius = Handles.RadiusHandle(
            Quaternion.identity, Vector3.right, example.radius, true);
    }
    private void ScaleHandleExample()
    {
        Handles.Label(Vector3.right, 
            string.Format("Scale: {0}", example.scale));
        example.scale = Handles.ScaleHandle(
            example.scale, Vector3.right, Quaternion.identity, 
            HandleUtility.GetHandleSize(Vector3.right));
    }
    private void RotationHandleExample()
    {
        Handles.Label(Vector3.right, 
            string.Format("Rot: {0}", example.rot));
        example.rot = Handles.RotationHandle(
            Quaternion.Euler(example.rot), Vector3.right).eulerAngles;
    }
    private void PositionHandleExample()
    {
        Handles.Label(example.pos, "拖动这个操控柄修改pos的值");
        example.pos = Handles.PositionHandle(example.pos, Quaternion.identity);
    }
    private void DrawDiscExample()
    {
        Handles.DrawWireDisc(Vector3.left, Vector3.up, 1f);
        Handles.DrawSolidDisc(Vector3.right, Vector3.up, 1f);
    }
    private void DrawArcExample()
    {
        Handles.DrawWireArc(Vector3.left, Vector3.up,
            Vector3.left, 180f, 1f);
        Handles.DrawSolidArc(Vector3.right, Vector3.up, 
            Vector3.left, 180f, 1f);
    }
    private void DrawWireCubeExample()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireCube(Vector3.zero, Vector3.one);
    }
    private void DrawPolyLineExample()
    {
        Vector3[] points = new Vector3[example.points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = example.points[i].position;
        }
        Handles.DrawPolyLine(points);
    }
    private void DrawLineExample()
    {
        Handles.color = Color.cyan;
        for (int i = 0; i < 10; i++)
        {
            Vector3 p1 = Vector3.right * (i * 0.5f);
            Vector3 p2 = p1 + Vector3.up;
            Handles.DrawLine(p1, p2, i);
        }
    }
}
