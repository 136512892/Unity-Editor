using UnityEngine;

public class GizmosExample : MonoBehaviour
{
    private Mesh mesh;
    private Texture texture;

    private void OnDrawGizmos()
    {
        DrawFrustumExample();
    }
    private void DrawFrustumExample()
    {
        Gizmos.DrawFrustum(Vector3.zero, 60f, 10f, 1f, 1.7f);
    }
    private void DrawTextureExample()
    {
        if (texture == null)
            texture = Resources.Load<Texture>("Axe");
        Gizmos.DrawGUITexture(new Rect(0, 0, 1, 1), texture);
    }
    private void DrawIconExample()
    {
        Gizmos.DrawIcon(Vector3.left, "Axe.png", true);
        Gizmos.DrawIcon(Vector3.right, "Axe.png", true, Color.cyan);
    }
    private void DrawMeshExample()
    {
        if (mesh == null)
            mesh = Resources.Load<Mesh>("Dagger_01");
        Gizmos.DrawMesh(mesh, -1, Vector3.left * .3f);
        Gizmos.DrawWireMesh(mesh, -1, Vector3.right * .3f);
    }
    private void DrawSphereExample()
    {
        Gizmos.DrawSphere(Vector3.left, 0.8f);
        Gizmos.DrawWireSphere(Vector3.right, 0.8f);
    }
    private void DrawCubeExample()
    {
        Gizmos.DrawCube(Vector3.left, Vector3.one * 0.8f);
        Gizmos.DrawWireCube(Vector3.right, Vector3.one * 0.8f);
    }
    private void DrawLineRayExample()
    {
        Gizmos.DrawLine(Vector3.zero, Vector3.up);
        Gizmos.DrawRay(Vector3.right, Vector3.up);
        Gizmos.DrawRay(new Ray(Vector3.right * 2f, Vector3.up));
    }
    private void Sample()
    {
        Gizmos.DrawLine(Vector3.zero, Vector3.up);
        Gizmos.DrawSphere(Vector3.right, .3f);
        Gizmos.DrawWireSphere(Vector3.right * 2f, .3f);
        Gizmos.DrawCube(Vector3.right * 3f, Vector3.one * .3f);
        Gizmos.DrawWireCube(Vector3.right * 4f, Vector3.one * .3f);
    }
}

