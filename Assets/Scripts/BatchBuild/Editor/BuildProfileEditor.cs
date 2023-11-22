using System.IO;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildProfile))]
public sealed class BuildProfileEditor : Editor
{
    private readonly Dictionary<BuildTask, bool> foldoutMap 
        = new Dictionary<BuildTask, bool>();
    private Vector2 scroll = Vector2.zero;
    private BuildProfile profile;

    private void OnEnable()
    {
        profile = target as BuildProfile;
    }
    public override void OnInspectorGUI()
    {
        OnTopGUI();
        OnBodyGUI();

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(profile);
        }
    }

    private void OnTopGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("新建", EditorStyles.miniButtonLeft))
            {
                Undo.RecordObject(profile, "Create");
                var task = new BuildTask()
                {
                    ProductName = "Product Name",
                    BuildTarget = BuildTarget.StandaloneWindows64,
                    BuildPath = Directory.GetParent(
                        Application.dataPath).FullName
                };
                profile.BuildTasks.Add(task);
            }
            if (GUILayout.Button("展开", EditorStyles.miniButtonMid))
            {
                for (int i = 0; i < profile.BuildTasks.Count; i++)
                {
                    foldoutMap[profile.BuildTasks[i]] = true;
                }
            }
            if (GUILayout.Button("收缩", EditorStyles.miniButtonMid))
            {
                for (int i = 0; i < profile.BuildTasks.Count; i++)
                {
                    foldoutMap[profile.BuildTasks[i]] = false;
                }
            }
            GUI.color = Color.yellow;
            if (GUILayout.Button("清空", EditorStyles.miniButtonMid))
            {
                Undo.RecordObject(profile, "Clear");
                if (EditorUtility.DisplayDialog("提醒",
                    "是否确定清空列表?", "确定", "取消"))
                {
                    profile.BuildTasks.Clear();
                }
            }
            GUI.color = Color.cyan;
            if (GUILayout.Button("打包", EditorStyles.miniButtonRight))
            {
                if (EditorUtility.DisplayDialog("提醒",
                    "打包需要耗费一定时间,是否确定开始?", "确定", "取消"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("打包报告:\r\n");
                    for (int i = 0; i < profile.BuildTasks.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("Build",
                            "Building...", (float)(i + 1) / 
                                profile.BuildTasks.Count);
                        var task = profile.BuildTasks[i];
                        List<EditorBuildSettingsScene> buildScenes 
                            = new List<EditorBuildSettingsScene>();
                        for (int j = 0; j < task.SceneAssets.Count; j++)
                        {
                            var scenePath = AssetDatabase
                                .GetAssetPath(task.SceneAssets[j]);
                            if (!string.IsNullOrEmpty(scenePath))
                            {
                                buildScenes.Add(
                                    new EditorBuildSettingsScene(
                                        scenePath, true));
                            }
                        }
                        string locationPathName = string.Format("{0}/{1}", 
                            task.BuildPath, task.ProductName);
                        var report = BuildPipeline.BuildPlayer(
                            buildScenes.ToArray(), locationPathName, 
                            task.BuildTarget, BuildOptions.None);
                        sb.Append(string.Format("[{0}] 打包结果: {1}\r\n",
                            task.ProductName, report.summary.result));
                    }
                    EditorUtility.ClearProgressBar();
                    Debug.Log(sb.ToString());
                }
                return;
            }
            GUI.color = Color.white;
        }
        GUILayout.EndHorizontal();
    }
    private void OnBodyGUI()
    {
        scroll = GUILayout.BeginScrollView(scroll);
        {
            for (int i = 0; i < profile.BuildTasks.Count; i++)
            {
                var task = profile.BuildTasks[i];
                if (!foldoutMap.ContainsKey(task)) 
                    foldoutMap.Add(task, true);
                GUILayout.BeginHorizontal("Badge");
                GUILayout.Space(12);
                foldoutMap[task] = EditorGUILayout
                    .Foldout(foldoutMap[task], $"{task.ProductName}", true);
                GUILayout.Label(string.Empty);
                if (GUILayout.Button(EditorGUIUtility
                    .IconContent("TreeEditor.Trash"),
                    "IconButton", GUILayout.Width(20)))
                {
                    Undo.RecordObject(profile, "Delete Task");
                    foldoutMap.Remove(task);
                    profile.BuildTasks.Remove(task);
                    break;
                }
                GUILayout.EndHorizontal();
                if (foldoutMap[task])
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("打包场景：", GUILayout.Width(70));
                    if (GUILayout.Button(EditorGUIUtility
                        .IconContent("Toolbar Plus More"),
                        GUILayout.Width(28)))
                    {
                        task.SceneAssets.Add(null);
                    }
                    GUILayout.EndHorizontal();
                    if (task.SceneAssets.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(75);
                        GUILayout.BeginVertical("Badge");
                        for (int j = 0; j < task.SceneAssets.Count; j++)
                        {
                            var sceneAsset = task.SceneAssets[j];
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"{j + 1}.", GUILayout.Width(20));
                            task.SceneAssets[j] = EditorGUILayout
                                .ObjectField(sceneAsset, typeof(SceneAsset),
                                    false) as SceneAsset;
                            if (GUILayout.Button("↑", 
                                EditorStyles.miniButtonLeft,
                                GUILayout.Width(20)))
                            {
                                if (j > 0)
                                {
                                    Undo.RecordObject(profile,
                                        "Move Up Scene Assets");
                                    var temp = task.SceneAssets[j - 1];
                                    task.SceneAssets[j - 1] = sceneAsset;
                                    task.SceneAssets[j] = temp;
                                }
                            }
                            if (GUILayout.Button("↓",
                                EditorStyles.miniButtonMid,
                                GUILayout.Width(20)))
                            {
                                if (j < task.SceneAssets.Count - 1)
                                {
                                    Undo.RecordObject(profile, 
                                        "Move Down Scene Assets");
                                    var temp = task.SceneAssets[j + 1];
                                    task.SceneAssets[j + 1] = sceneAsset;
                                    task.SceneAssets[j] = temp;
                                }
                            }
                            if (GUILayout.Button(EditorGUIUtility
                                .IconContent("Toolbar Plus"), 
                                EditorStyles.miniButtonMid, 
                                GUILayout.Width(20)))
                            {
                                Undo.RecordObject(profile,
                                    "Add Scene Assets");
                                task.SceneAssets.Insert(j + 1, null);
                                break;
                            }
                            if (GUILayout.Button(EditorGUIUtility
                                .IconContent("Toolbar Minus"), 
                                EditorStyles.miniButtonRight, 
                                GUILayout.Width(20)))
                            {
                                Undo.RecordObject(profile, 
                                    "Delete Scene Assets");
                                task.SceneAssets.RemoveAt(j);
                                break;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("产品名称：", GUILayout.Width(70));
                    var newPN = GUILayout.TextField(task.ProductName);
                    if (task.ProductName != newPN)
                    {
                        Undo.RecordObject(profile, "Product Name");
                        task.ProductName = newPN;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("打包平台：", GUILayout.Width(70));
                    var newBT = (BuildTarget)EditorGUILayout
                        .EnumPopup(task.BuildTarget);
                    if (task.BuildTarget != newBT)
                    {
                        Undo.RecordObject(profile, "Build Target");
                        task.BuildTarget = newBT;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("打包路径：", GUILayout.Width(70));
                    GUILayout.TextField(task.BuildPath);
                    if (GUILayout.Button("Browse", GUILayout.Width(60)))
                    {
                        task.BuildPath = EditorUtility
                            .SaveFolderPanel("Build Path", 
                                task.BuildPath, "");
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
        }
        GUILayout.EndScrollView();
    }
}