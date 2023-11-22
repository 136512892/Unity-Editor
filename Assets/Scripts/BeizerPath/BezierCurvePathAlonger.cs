using UnityEngine;

public class BezierCurvePathAlonger : MonoBehaviour
{
    [SerializeField] private BezierCurvePath path;
    [SerializeField] private float speed = .1f;

    private float normalized = 0f;
    private Vector3 lastPosition;

    private void Update()
    {
        float t = normalized + speed * Time.deltaTime;
        float max = path.points.Count - 1 < 1 ? 0 : (path.loop
            ? path.points.Count : path.points.Count - 1);
        normalized = (path.loop && max > 0) ? ((t %= max)
            + (t < 0 ? max : 0)) : Mathf.Clamp(t, 0, max);
        transform.position = path.EvaluatePosition(normalized);
        Vector3 forward = transform.position - lastPosition;
        transform.forward = forward != Vector3.zero
            ? forward : transform.forward;
        lastPosition = transform.position;
    }
}