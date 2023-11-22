using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioDatabase))]
public class AudioDatabaseEditor : Editor
{
    private AudioDatabase database;
    private Dictionary<AudioData, AudioSource> players;
    private GUIStyle progressStyle;

    private void OnEnable()
    {
        database = target as AudioDatabase;
        players = new Dictionary<AudioData, AudioSource>();
        EditorApplication.update += Update;
    }

    private void OnDestroy()
    {
        EditorApplication.update -= Update;
        foreach (var player in players)
            DestroyImmediate(player.Value.gameObject);
        players.Clear();
    }

    private void Update()
    {
        Repaint();
        foreach (var player in players)
        {
            if (!player.Value.isPlaying)
            {
                DestroyImmediate(player.Value.gameObject);
                players.Remove(player.Key);
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (progressStyle == null)
        {
            progressStyle = new GUIStyle(GUI.skin.label) 
            { 
                alignment = TextAnchor.LowerRight, 
                fontSize = 8, 
                fontStyle = FontStyle.Italic
            };
        }
        if (GUILayout.Button("Create New Audio Group"))
        {
            var audioGroup = Activator.CreateInstance<AudioGroup>();
            audioGroup.name = "New Audio Group";
            audioGroup.id = database.groups.Count == 0 
                ? 100 : (database.groups.Count + 100);
            AssetDatabase.AddObjectToAsset(audioGroup, database);
            database.groups.Add(audioGroup);
            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(audioGroup));
        }
        EditorGUILayout.Space();
        for (int i = 0; i < database.groups.Count; i++)
        {
            AudioGroup group = database.groups[i];
            OnAudioGroupGUI(group);
            EditorGUILayout.Space();
        }
    }

    private void OnAudioGroupGUI(AudioGroup group)
    {
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        //Name
        string newGroupName = EditorGUILayout.TextField(group.name);
        if (newGroupName != group.name)
        {
            Undo.RecordObject(database, "Audio Group Name");
            group.name = newGroupName;
        }
        //ID
        var newId = EditorGUILayout.IntField(group.id);
        if (newId != group.id)
        {
            Undo.RecordObject(database, "Audio Group ID");
            group.id = newId;
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssetIfDirty(database);
        }
        //删除按钮
        if (GUILayout.Button(EditorGUIUtility.IconContent(
            "Toolbar Minus"), GUILayout.Width(20f)))
        {
            if (EditorUtility.DisplayDialog(
                "提醒", "是否确认删除该音频数据组", "确认", "取消"))
            {
                Undo.RecordObject(database, "Delete Audio Group");
                database.groups.Remove(group);
                AssetDatabase.RemoveObjectFromAsset(group);
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //Audio Datas
        for (int j = 0; j < group.datas.Count; j++)
        {
            AudioData data = group.datas[j];
            OnAudioDataGUI(group, data);
        }

        EditorGUILayout.Space();
        //以下代码块中绘制了一个矩形区域
        //将AudioClip资产拖到该区域则添加一项音频数据
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var dropRect = new Rect(lastRect.x + 2f,
                lastRect.y - 2f, 120f, 20f);
            bool containsMouse = dropRect
                .Contains(Event.current.mousePosition);
            if (containsMouse)
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        bool containsAudioClip = DragAndDrop
                            .objectReferences.OfType<AudioClip>().Any();
                        DragAndDrop.visualMode = containsAudioClip 
                            ? DragAndDropVisualMode.Copy 
                            : DragAndDropVisualMode.Rejected;
                        Event.current.Use();
                        Repaint();
                        break;
                    case EventType.DragPerform:
                        IEnumerable<AudioClip> audioClips = DragAndDrop
                            .objectReferences.OfType<AudioClip>();
                        foreach (var audioClip in audioClips)
                        {
                            int index = group.datas.FindIndex(
                                m => m.clip == audioClip);
                            if (index == -1)
                            {
                                Undo.RecordObject(database, "Add Audio Data");
                                int newDataId = group.datas.Count == 0 
                                    ? 1000 : (group.datas.Count + 1000);
                                group.datas.Add(new AudioData() 
                                { 
                                    id = newDataId,
                                    clip = audioClip
                                });
                                serializedObject.ApplyModifiedProperties();
                                EditorUtility.SetDirty(database);
                            }
                        }
                        Event.current.Use();
                        Repaint();
                        break;
                }
            }
            Color color = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g,
                GUI.color.b, containsMouse ? .9f : .5f);
            GUI.Box(dropRect, "Drop AudioClips Here", 
                new GUIStyle(GUI.skin.box) { fontSize = 10 });
            GUI.color = color;
        }
        GUILayout.EndHorizontal();
    }

    private void OnAudioDataGUI(AudioGroup group, AudioData data)
    {
        Color cacheColor = GUI.color;
        GUILayout.BeginHorizontal();
        //绘制音频图标
        GUILayout.Label(EditorGUIUtility
            .IconContent("SceneViewAudio"), GUILayout.Width(20f));
        EditorGUI.BeginChangeCheck();
        //Audio Data ID
        int newDataId = EditorGUILayout
            .IntField(data.id, GUILayout.Width(60f));
        if (newDataId != data.id)
        {
            Undo.RecordObject(database, "Audio Data Id");
            data.id = newDataId;
        }
        //Audio Clip
        AudioClip clip = EditorGUILayout.ObjectField(
            data.clip, typeof(AudioClip), false) as AudioClip;
        if (clip != data.clip)
        {
            Undo.RecordObject(database, "Audio Data Clip");
            data.clip = clip;
        }
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(database);
        }
        //若该音频正在播放 计算其播放进度 
        string progress = players.ContainsKey(data) 
            ? ToTimeFormat(players[data].time) : "00:00:000";
        GUI.color = new Color(GUI.color.r, GUI.color.g,
            GUI.color.b, players.ContainsKey(data) ? .9f : .5f);
        //显示信息：播放进度 / 音频时长 (00:00 / 00:00)
        GUILayout.Label(string.Format("({0} / {1})", progress,
            data.clip != null ? ToTimeFormat(data.clip.length) 
            : "00:00:000"), progressStyle, GUILayout.Width(100f));
        GUI.color = cacheColor;
        //播放按钮
        if (GUILayout.Button(EditorGUIUtility
            .IconContent("PlayButton"), GUILayout.Width(20f)))
        {
            if (!players.ContainsKey(data))
            {
                //创建一个物体并添加AudioSource组件 
                var source = EditorUtility.CreateGameObjectWithHideFlags(
                    "Audio Player",HideFlags.HideAndDontSave)
                    .AddComponent<AudioSource>();
                source.clip = data.clip;
                source.Play();
                players.Add(data, source);
            }
        }
        //停止播放按钮
        if (GUILayout.Button(EditorGUIUtility
            .IconContent("PauseButton"), GUILayout.Width(20f)))
        {
            if (players.ContainsKey(data))
            {
                DestroyImmediate(players[data].gameObject);
                players.Remove(data);
            }
        }
        //删除按钮 点击后删除该项音频数据
        if (GUILayout.Button(EditorGUIUtility
            .IconContent("Toolbar Minus"), GUILayout.Width(20f)))
        {
            Undo.RecordObject(database, "Delete Audio Data");
            group.datas.Remove(data);
            if (players.ContainsKey(data))
            {
                DestroyImmediate(players[data].gameObject);
                players.Remove(data);
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(database);
            Repaint();
        }
        GUILayout.EndHorizontal();
    }

    //将描述转换为mm:ss:fff时间格式字符串
    private string ToTimeFormat(float time)
    {
        int millSecounds = (int)(time * 1000);
        int minutes = millSecounds / 60000;
        int seconds = millSecounds % 60000 / 1000;
        millSecounds = millSecounds % 60000 % 1000;
        return string.Format("{0:D2}:{1:D2}:{2:D3}",
            minutes, seconds, millSecounds);
    }
}