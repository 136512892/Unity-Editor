using UnityEngine;
using UnityEditor;

public class BezierCurvePathExample : MonoBehaviour
{
    private float t;

    private void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime * .2f;
            t = Mathf.Clamp01(t);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Bezier3Example();
    }
    private void Bezier3Example()
    {
        Gizmos.color = Color.grey;
        Vector3 p0 = Vector3.left * 5f;
        Vector3 p1 = Vector3.left * 2f + Vector3.forward * 2f;
        Vector3 p2 = Vector3.right * 3f + Vector3.back * 4f;
        Vector3 p3 = Vector3.right * 5f;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Handles.Label(p0, "P0");
        Handles.Label(p1, "P1");
        Handles.Label(p2, "P2");
        Handles.Label(p3, "P3");
        Handles.SphereHandleCap(0, p0, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p1, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p2, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p3, Quaternion.identity,
            0.1f, EventType.Repaint);

        Gizmos.color = Color.green;
        for (int i = 0; i < 100; i++)
        {
            Vector3 curr = BezierCurveUtility.Bezier3(
                p0, p1, p2, p3, i / 100f);
            Vector3 next = BezierCurveUtility.Bezier3(
                p0, p1, p2, p3, (i + 1) / 100f);
            Gizmos.color = t > (i / 100f)
                ? Color.red : Color.green;
            Gizmos.DrawLine(curr, next);
        }
        Vector3 pt = BezierCurveUtility.Bezier3(p0, p1, p2, p3, t);
        Handles.Label(pt, string.Format("Pt (t = {0})", t));
        Handles.SphereHandleCap(0, pt, Quaternion.identity,
            0.1f, EventType.Repaint);
    }
    private void Bezier2Example()
    {
        Gizmos.color = Color.grey;
        Vector3 p0 = Vector3.left * 5f;
        Vector3 p1 = Vector3.left * 2f + Vector3.forward * 2f;
        Vector3 p2 = Vector3.right * 5f;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p2, p1);
        Handles.Label(p0, "P0");
        Handles.Label(p1, "P1");
        Handles.Label(p2, "P2");
        Handles.SphereHandleCap(0, p0, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p1, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p2, Quaternion.identity,
            0.1f, EventType.Repaint);

        Gizmos.color = Color.green;
        for (int i = 0; i < 100; i++)
        {
            Vector3 curr = BezierCurveUtility.Bezier2(
                p0, p1, p2, i / 100f);
            Vector3 next = BezierCurveUtility.Bezier2(
                p0, p1, p2, (i + 1) / 100f);
            Gizmos.color = t > (i / 100f)
                ? Color.red : Color.green;
            Gizmos.DrawLine(curr, next);
        }
        Vector3 pt = BezierCurveUtility.Bezier2(p0, p1, p2, t);
        Handles.Label(pt, string.Format("Pt (t = {0})", t));
        Handles.SphereHandleCap(0, pt, Quaternion.identity,
            0.1f, EventType.Repaint);
    }
    private void Bezier1Example()
    {
        Gizmos.color = Color.grey;
        Vector3 p0 = Vector3.left * 5f;
        Vector3 p1 = Vector3.right * 5f;
        Gizmos.DrawLine(p0, p1);
        Handles.Label(p0, "P0");
        Handles.Label(p1, "P1");
        Handles.SphereHandleCap(0, p0, Quaternion.identity,
            0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p1, Quaternion.identity,
            0.1f, EventType.Repaint);
        Vector3 pt = BezierCurveUtility.Bezier1(p0, p1, t);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(p0, pt);
        Handles.Label(pt, string.Format("Pt (t = {0})", t));
        Handles.SphereHandleCap(0, pt, Quaternion.identity,
            0.1f, EventType.Repaint);
    }
#endif
}