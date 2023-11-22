using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetBundleConfigure : EditorWindow
{
    [MenuItem("Example/Resource/AssetBundle Configure")]
    public static void Open()
    {
        GetWindow<AssetBundleConfigure>().Show();
    }

    private Vector2 lScrollPosition, rScrollPosition;
    private Vector2 abDetailScrollPosition;
    private Vector2 assetDetailScrollPosition;
    //�ָ��߿��
    private const float splitterWidth = 2f;
    //�ָ���λ��
    private float splitterPos;
    private Rect splitterRect;
    //�Ƿ�������ק�ָ���
    private bool isDragging;

    //AssetBundle���Ƽ���
    private string[] assetBundleNames;
    //<AssetBundle���ƣ�Assets·������>
    private Dictionary<string, string[]> map;
    //��ǰѡ�е�AssetBundle����
    private string selectedAssetBundleName;
    //��ǰѡ�е�Asset·��
    private string selectedAssetPath;

    //����AssetBundle
    private string searchAssetBundle;
    //����Asset·��
    private string searchAssetPath;

    private void OnEnable()
    {
        splitterPos = position.width * .5f;
        Init();
    }

    private void OnDisable()
    {
        map = null;
        searchAssetBundle = null;
        selectedAssetBundleName = null;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        lScrollPosition = GUILayout.BeginScrollView(
            lScrollPosition,
            GUILayout.Width(splitterPos),
            GUILayout.MaxWidth(splitterPos),
            GUILayout.MinWidth(splitterPos));
        OnLeftGUI();
        GUILayout.EndScrollView();

        //�ָ���
        GUILayout.Box(string.Empty,
            GUILayout.Width(splitterWidth),
            GUILayout.MaxWidth(splitterWidth),
            GUILayout.MinWidth(splitterWidth),
            GUILayout.ExpandHeight(true));
        splitterRect = GUILayoutUtility.GetLastRect();

        rScrollPosition = GUILayout.BeginScrollView(
            rScrollPosition, GUILayout.ExpandWidth(true));
        OnRightGUI();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();

        if (Event.current != null)
        {
            //���
            EditorGUIUtility.AddCursorRect(splitterRect,
                MouseCursor.ResizeHorizontal);
            switch (Event.current.rawType)
            {
                //��ʼ��ק�ָ���
                case EventType.MouseDown:
                    isDragging = splitterRect.Contains(
                        Event.current.mousePosition);
                    break;
                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        splitterPos += Event.current.delta.x;
                        //�����������Сֵ
                        splitterPos = Mathf.Clamp(splitterPos,
                            position.width * .2f, position.width * .8f);
                        Repaint();
                    }
                    break;
                //������ק�ָ���
                case EventType.MouseUp:
                    if (isDragging)
                        isDragging = false;
                    break;
            }
        }
    }

    private void Init(bool reselect = true)
    {
        if (reselect)
        {
            selectedAssetBundleName = null;
            selectedAssetPath = null;
        }
        //��ȡ����AssetBundle����
        assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        //��ʼ��map�ֵ�
        map = new Dictionary<string, string[]>();
        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            map.Add(assetBundleNames[i], AssetDatabase
                .GetAssetPathsFromAssetBundle(assetBundleNames[i]));
        }
    }

    private void OnLeftGUI()
    {
        //ˢ�� ���¼���AssetBundle��Ϣ
        if (GUILayout.Button("Refresh"))
        {
            Init();
            Repaint();
        }
        //�Ƴ�δʹ�õ�AssetBundle����
        if (GUILayout.Button("RemoveUnusedNames"))
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            Init();
            Repaint();
        }
        //���������
        searchAssetBundle = GUILayout.TextField(
            searchAssetBundle, EditorStyles.toolbarSearchField);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        //�������������λ�ò����������ʱ ȡ���ؼ��ľ۽�
        if (Event.current.type == EventType.MouseDown 
            && !lastRect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
            Repaint();
        }

        //�б�����
        Rect listRect = new Rect(0f, lastRect.y + 20f, 
            lastRect.width, position.height - lastRect.y - 25f);
        //�����ק�ʲ����б�����Ϊ�ʲ�����AssetBundle
        if (DragObjects2RectCheck(listRect, out Object[] objects))
        {
            bool flag = false;
            for (int i = 0; i < objects.Length; i++)
            {
                if (AssetBundleUtility
                    .CreateAssetBundle4Object(objects[i]))
                    flag = true;
            }
            //�������µ�AssetBundle ˢ��
            if (flag)
            {
                Init();
                Repaint();
            }
        }

        if (assetBundleNames.Length == 0) return;
        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            string assetBundleName = assetBundleNames[i];
            if (!string.IsNullOrEmpty(searchAssetBundle) 
                && !assetBundleName.ToLower()
                    .Contains(searchAssetBundle.ToLower()))
                continue;
            GUILayout.BeginHorizontal(selectedAssetBundleName
                == assetBundleName ? "MeTransitionSelectHead" 
                : "ProjectBrowserHeaderBgTop", 
                GUILayout.Height(20f));
            GUILayout.Label(EditorGUIUtility.TrTextContentWithIcon(
                 assetBundleName, "GameObject Icon"), GUILayout.Height(18f));
            GUILayout.EndHorizontal();
            //������¼�
            if (Event.current.type == EventType.MouseDown 
                && GUILayoutUtility.GetLastRect()
                    .Contains(Event.current.mousePosition))
            {
                //����������� ѡ�и���
                if (Event.current.button == 0)
                {
                    selectedAssetBundleName = assetBundleName;
                    Repaint();
                }
                //������Ҽ���� �����˵�
                if (Event.current.button == 1)
                {
                    GenericMenu gm = new GenericMenu();
                    //ɾ��AssetBundle
                    gm.AddItem(new GUIContent("Delete AssetBundle"), false, () =>
                    {
                        //����ȷ�ϵ���
                        if (EditorUtility.DisplayDialog("����", 
                            string.Format("�Ƿ�ȷ��ɾ��{0}��",
                                assetBundleName), "ȷ��", "ȡ��"))
                        {
                            AssetBundleUtility.DeleteAssetBundleName(
                                assetBundleName);
                            Init();
                            Repaint();
                        }
                    });
                    gm.ShowAsContext();
                }
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(100f), 
            GUILayout.ExpandWidth(true));
        string[] dependencies = AssetDatabase.GetAssetBundleDependencies(
            selectedAssetBundleName, true);
        GUILayout.Label("Dependencies:");
        abDetailScrollPosition = GUILayout.BeginScrollView(
            abDetailScrollPosition);
        for (int i = 0; i < dependencies.Length; i++)
        {
            GUILayout.Label(dependencies[i]);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OnRightGUI()
    {
        //���������
        searchAssetPath = GUILayout.TextField(searchAssetPath, 
            EditorStyles.toolbarSearchField);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        //�������������λ�ò����������ʱ ȡ���ؼ��ľ۽�
        if (Event.current.type == EventType.MouseDown 
            && !lastRect.Contains(Event.current.mousePosition))
        {
            GUI.FocusControl(null);
            Repaint();
        }
        if (selectedAssetBundleName == null) return;

        //�ʲ���ק����
        Rect dragRect = new Rect(lastRect.x, lastRect.y + 20f,
            lastRect.width - 5f, position.height - lastRect.y - 25f);
        //������ʲ���ק��������Ϊ��Щ�ʲ�����AssetBundle��
        if (DragObjects2RectCheck(dragRect, out Object[] objects))
        {
            for (int i = 0; i < objects.Length; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(objects[i]);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                    importer.assetBundleName = selectedAssetBundleName;
            }
            Init(false);
            Repaint();
            return;
        }

        //��AssetBundle�е��ʲ�·������
        string[] assetPaths = map[selectedAssetBundleName];
        if (assetPaths.Length == 0) return;
        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];
            //��ǰ���Ƿ���ϼ�������
            if (!string.IsNullOrEmpty(searchAssetPath) 
                && !assetPath.ToLower().Contains(
                    searchAssetPath.ToLower())) 
                continue;
            GUILayout.BeginHorizontal(selectedAssetPath == assetPath 
                ? "MeTransitionSelectHead" 
                : "ProjectBrowserHeaderBgTop", 
                GUILayout.Height(20f));
            Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            Texture texture = AssetPreview.GetMiniTypeThumbnail(type);
            GUILayout.Label(texture, GUILayout.Width(18f),
                GUILayout.Height(18f));
            GUILayout.Label(assetPaths[i]);
            GUILayout.EndHorizontal();
            //������¼�
            if (Event.current.type == EventType.MouseDown
                && GUILayoutUtility.GetLastRect()
                    .Contains(Event.current.mousePosition))
            {
                //����������� ѡ�и���
                if (Event.current.button == 0)
                {
                    selectedAssetPath = assetPath;
                    Repaint();
                    EditorGUIUtility.PingObject(
                        AssetDatabase.LoadMainAssetAtPath(
                            selectedAssetPath));
                }
                //������Ҽ���� �����˵�
                else if (Event.current.button == 1)
                {
                    GenericMenu gm = new GenericMenu();
                    //ɾ��
                    gm.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        //�����ʲ�·����ȡ���ʲ�������
                        AssetImporter importer = AssetImporter
                            .GetAtPath(assetPath);
                        //���AssetBundle��
                        if (importer != null)
                            importer.assetBundleName = null;
                        Init(false);
                        Repaint();
                    });
                    gm.ShowAsContext();
                }
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(100f)
            , GUILayout.ExpandWidth(true));
        string[] dependencies = AssetDatabase.GetDependencies(
            selectedAssetPath, true);
        GUILayout.Label("Dependencies:");
        assetDetailScrollPosition = GUILayout.BeginScrollView(
            assetDetailScrollPosition);
        for (int i = 0; i < dependencies.Length; i++)
        {
            GUILayout.Label(dependencies[i]);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    //�Ƿ���ק�ʲ�������������
    private bool DragObjects2RectCheck(Rect rect, out Object[] objects)
    {
        objects = null;
        //����Ƿ��ھ���������
        if (rect.Contains(Event.current.mousePosition))
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    //�Ƿ���ק���ʲ�
                    bool containsObjects = DragAndDrop
                        .objectReferences.Any();
                    DragAndDrop.visualMode = containsObjects
                        ? DragAndDropVisualMode.Copy
                        : DragAndDropVisualMode.Rejected;
                    Event.current.Use();
                    Repaint();
                    return false;
                case EventType.DragPerform:
                    //��ק���ʲ�
                    objects = DragAndDrop.objectReferences;
                    Event.current.Use();
                    Repaint();
                    return true;
            }
        }
        return false;
    }
}