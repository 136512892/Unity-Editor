using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GizmosExample))]
public class GizmosExampleEditor : Editor
{
    [DrawGizmo(GizmoType.Pickable
        | GizmoType.Active | GizmoType.InSelectionHierarchy
        | GizmoType.NotInSelectionHierarchy, typeof(GizmosExample))]
    public static void DrawGizmos(GizmosExample example, GizmoType gizmoType)
    {
        //可以通过color调整颜色
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, Vector3.up);
        Gizmos.DrawSphere(Vector3.left, .3f);
        Gizmos.DrawWireSphere(Vector3.left * 2f, .3f);
        Gizmos.DrawCube(Vector3.left * 3f, Vector3.one * .3f);
        Gizmos.DrawWireCube(Vector3.left * 4f, Vector3.one * .3f);
    }
}