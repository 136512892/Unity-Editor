using UnityEngine;
using UnityEditor;

public class SceneGUIExample : EditorWindow
{
    [MenuItem("Example/Scene GUI Example")]
    public static void Open()
    {
        GetWindow<SceneGUIExample>().Show();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    private void OnSceneGUI(SceneView sceneView)
    {
        Handles.DrawLine(Vector3.zero, Vector3.up);
        //TOOD
    }
}