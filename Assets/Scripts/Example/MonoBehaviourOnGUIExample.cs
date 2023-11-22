using UnityEngine;

public class MonoBehaviourOnGUIExample : MonoBehaviour
{
    private Rect windowRect = new Rect(0f, 0f, 300f, 100f);

    private void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect,
            OnWindowGUI, "Example Window");
    }
    private void OnWindowGUI(int windowId)
    {
        GUI.DragWindow(new Rect(0f, 0f, 300f, 20f));
        GUILayout.Label("Hello World.");
    }

    private void TimeScaleExample()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("TimeScale",
            GUILayout.Width(100f));
        if (GUILayout.Button("0.1f",
            GUILayout.Width(100f), GUILayout.Height(50f)))
            Time.timeScale = 0.1f;
        if (GUILayout.Button("0.25f",
            GUILayout.Width(100f), GUILayout.Height(50f)))
            Time.timeScale = 0.25f;
        if (GUILayout.Button("0.5f",
            GUILayout.Width(100f), GUILayout.Height(50f)))
            Time.timeScale = 0.5f;
        if (GUILayout.Button("1",
            GUILayout.Width(100f), GUILayout.Height(50f)))
            Time.timeScale = 1f;
        GUILayout.EndHorizontal();
    }
}