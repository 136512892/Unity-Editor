using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public sealed class GUIIconPreviewer : EditorWindow
{
    [MenuItem("Example/Built-In GUIIcon Previewer")]
    public static void Open()
    {
        GetWindow<GUIIconPreviewer>().Show();
    }

    private Vector2 scrollPosition;
    private string searchContent = "";
    private const float width = 50f;

    private List<string> iconNames;
    private string[] matchedIconNames;

    private void OnEnable()
    {
        //存储图标名称的文件路径
        string filePath = Path.GetFullPath(".").Replace("\\", "/") 
            + "/Library/built-in gui icon names.txt";
        if (!File.Exists(filePath))
        {
            iconNames = new List<string>();
            StringBuilder sb = new StringBuilder();
            Texture[] textures = Resources.FindObjectsOfTypeAll<Texture>();
            for (int i = 0; i < textures.Length; i++)
            {
                string name = textures[i].name;
                if (string.IsNullOrEmpty(name)) continue;
                if (EditorGUIUtility.IconContent(name).image != null)
                {
                    sb.Append(name);
                    sb.Append("\r");
                    iconNames.Add(name);
                }
            }
            //写入缓存
            File.WriteAllText(filePath, sb.ToString().TrimEnd());
        }
        else
        {
            string fileContent = File.ReadAllText(filePath);
            iconNames = new List<string>(fileContent.Split('\r'));
        }
        matchedIconNames = iconNames.Where(m 
            => m.ToLower().Contains(searchContent.ToLower())).ToArray();
    }

    private void OnGUI()
    {
        //搜索栏
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUILayout.Label("Search:", GUILayout.Width(50f));
            EditorGUI.BeginChangeCheck();
            searchContent = GUILayout.TextField(searchContent, 
                EditorStyles.toolbarSearchField);
            //检索的内容发生变更
            if (EditorGUI.EndChangeCheck())
                matchedIconNames = iconNames.Where(m => m.ToLower()
                .Contains(searchContent.ToLower())).ToArray();
        }
        GUILayout.EndHorizontal();

        //滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        {
            int count = Mathf.RoundToInt(position.width / (width + 3f));
            for (int i = 0; i < matchedIconNames.Length; i += count)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < count; j++)
                {
                    int index = i + j;
                    if (index < matchedIconNames.Length)
                    {
                        if (GUILayout.Button(EditorGUIUtility
                            .IconContent(matchedIconNames[index]),
                            GUILayout.Width(width), GUILayout.Height(30)))
                        {
                            //按钮点击时 复制图标的名称到粘贴板中
                            EditorGUIUtility.systemCopyBuffer
                                = matchedIconNames[index];
                            Debug.Log(matchedIconNames[index]);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }
}