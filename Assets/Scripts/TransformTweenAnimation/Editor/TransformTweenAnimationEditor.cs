using UnityEngine;
using UnityEditor;
using DG.Tweening;

[CustomEditor(typeof(TransformTweenAnimation))]
public class TransformTweenAnimationEditor : Editor
{
    private TransformTweenAnimation animation;
    
    private readonly static float alpha = .4f;
    readonly static float labelWidth = 60f;
    readonly static GUIContent duration = new GUIContent("Duration", "动画时长");
    readonly static GUIContent delay = new GUIContent("Delay", "延时时长");
    readonly static GUIContent from = new GUIContent("From", "初始值");
    readonly static GUIContent to = new GUIContent("To", "目标值");
    readonly static GUIContent ease = new GUIContent("Ease");
    readonly static GUIContent rotateMode = new GUIContent("Mode", "旋转模式");

    private PreviewRenderUtility previewRenderUtility;
    private GameObject previewInstance;
    private bool isPlaying;
    private Vector2 dragRot;
    private float distance = 5f;

    private void OnEnable()
    {
        animation = target as TransformTweenAnimation;

        previewInstance = Instantiate(animation.gameObject, 
            Vector3.zero, Quaternion.identity);
        previewRenderUtility = new PreviewRenderUtility(true);
        previewRenderUtility.AddSingleGO(previewInstance);
        EditorApplication.update += Update;
    }
    private void OnDisable()
    {
        EditorApplication.update -= Update;

        previewRenderUtility.Cleanup();
        previewRenderUtility = null;
        DestroyImmediate(previewInstance);
        previewInstance = null;
    }
    private void Update()
    {
        if (isPlaying)
        {
            Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        OnMenuGUI();
        OnMoveAnimationGUI();
        GUILayout.Space(3f);
        OnRotateAnimationGUI();
        GUILayout.Space(3f);
        OnScaleAnimationGUI();
    }
    private void OnMenuGUI()
    {
        GUILayout.BeginHorizontal();
        Color cacheColor = GUI.color;
        Color alphaColor = new Color(cacheColor.r,
            cacheColor.g, cacheColor.b, alpha);
        //Move
        GUI.color = animation.move.toggle ? cacheColor : alphaColor;
        if (GUILayout.Button(EditorGUIUtility.IconContent("MoveTool"),
            "ButtonLeft", GUILayout.Width(25f)))
        {
            Undo.RecordObject(target, "Move Toggle");
            animation.move.toggle = !animation.move.toggle;
            EditorUtility.SetDirty(target);
        }
        //Rotate
        GUI.color = animation.rotate.toggle ? cacheColor : alphaColor;
        if (GUILayout.Button(EditorGUIUtility.IconContent("RotateTool"),
            "ButtonMid", GUILayout.Width(25f)))
        {
            Undo.RecordObject(target, "Rotate Toggle");
            animation.rotate.toggle = !animation.rotate.toggle;
            EditorUtility.SetDirty(target);
        }
        //Scale
        GUI.color = animation.scale.toggle ? cacheColor : alphaColor;
        if (GUILayout.Button(EditorGUIUtility.IconContent("ScaleTool"),
            "ButtonRight", GUILayout.Width(25f)))
        {
            Undo.RecordObject(target, "Scale Toggle");
            animation.scale.toggle = !animation.scale.toggle;
            EditorUtility.SetDirty(target);
        }
        GUI.color = cacheColor;
        GUILayout.EndHorizontal();
    }
    private void OnMoveAnimationGUI()
    {
        if (animation.move.toggle)
        {
            GUILayout.BeginHorizontal("Badge");
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(30f);
                    GUILayout.Label(EditorGUIUtility.IconContent("MoveTool"));
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    //Duration、Delay
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(duration, GUILayout.Width(labelWidth));
                        var newDuration = EditorGUILayout.FloatField(animation.move.duration);
                        if (newDuration != animation.move.duration)
                        {
                            Undo.RecordObject(target, "Move Duration");
                            animation.move.duration = newDuration;
                            EditorUtility.SetDirty(target);
                        }

                        GUILayout.Label(delay, GUILayout.Width(labelWidth - 20f));
                        var newDelay = EditorGUILayout.FloatField(animation.move.delay);
                        if (newDelay != animation.move.delay)
                        {
                            Undo.RecordObject(target, "Move Delay");
                            animation.move.delay = newDelay;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //From
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(from, GUILayout.Width(labelWidth));
                        Vector3 newStartValue = EditorGUILayout.Vector3Field(GUIContent.none, animation.move.startValue);
                        if (newStartValue != animation.move.startValue)
                        {
                            Undo.RecordObject(target, "Move From");
                            animation.move.startValue = newStartValue;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //To
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(to, GUILayout.Width(labelWidth));
                        Vector3 newEndValue = EditorGUILayout.Vector3Field(GUIContent.none, animation.move.endValue);
                        if (newEndValue != animation.move.endValue)
                        {
                            Undo.RecordObject(target, "Move To");
                            animation.move.endValue = newEndValue;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //Ease
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(ease, GUILayout.Width(labelWidth));
                        var newEase = (Ease)EditorGUILayout.EnumPopup(animation.move.ease);
                        if (newEase != animation.move.ease)
                        {
                            Undo.RecordObject(target, "Move Ease");
                            animation.move.ease = newEase;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
    private void OnRotateAnimationGUI()
    {
        if (animation.rotate.toggle)
        {
            GUILayout.BeginHorizontal("Badge");
            {
                GUILayout.BeginVertical();
                GUILayout.Space(40f);
                GUILayout.Label(EditorGUIUtility.IconContent("RotateTool"));
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    //Duration、Delay
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(duration, GUILayout.Width(labelWidth));
                    var newDuration = EditorGUILayout.FloatField(
                        animation.rotate.duration);
                    if (newDuration != animation.rotate.duration)
                    {
                        Undo.RecordObject(target, "Rotate Duration");
                        animation.rotate.duration = newDuration;
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.Label(delay, GUILayout.Width(labelWidth - 20f));
                    var newDelay = EditorGUILayout.FloatField(
                        animation.rotate.delay);
                    if (newDelay != animation.rotate.delay)
                    {
                        Undo.RecordObject(target, "Rotate Delay");
                        animation.rotate.delay = newDelay;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.EndHorizontal();

                    //From
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(from, GUILayout.Width(labelWidth));
                    Vector3 newStartValue = EditorGUILayout.Vector3Field(
                        GUIContent.none, animation.rotate.startValue);
                    if (newStartValue != animation.rotate.startValue)
                    {
                        Undo.RecordObject(target, "Rotate From");
                        animation.rotate.startValue = newStartValue;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.EndHorizontal();

                    //To
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(to, GUILayout.Width(labelWidth));
                    Vector3 newEndValue = EditorGUILayout.Vector3Field(
                        GUIContent.none, animation.rotate.endValue);
                    if (newEndValue != animation.rotate.endValue)
                    {
                        Undo.RecordObject(target, "Rotate To");
                        animation.rotate.endValue = newEndValue;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.EndHorizontal();

                    //Rotate Mode
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(rotateMode, GUILayout.Width(labelWidth));
                    var newRotateMode = (RotateMode)EditorGUILayout.EnumPopup(
                        animation.rotate.mode);
                    if (newRotateMode != animation.rotate.mode)
                    {
                        Undo.RecordObject(target, "Rotate Mode");
                        animation.rotate.mode = newRotateMode;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.EndHorizontal();

                    //Ease
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(ease, GUILayout.Width(labelWidth));
                    var newEase = (Ease)EditorGUILayout.EnumPopup(
                        animation.rotate.ease);
                    if (newEase != animation.rotate.ease)
                    {
                        Undo.RecordObject(target, "Rotate Ease");
                        animation.rotate.ease = newEase;
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
    private void OnScaleAnimationGUI()
    {
        if (animation.scale.toggle)
        {
            GUILayout.BeginHorizontal("Badge");
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(30f);
                    GUILayout.Label(EditorGUIUtility.IconContent("ScaleTool"));
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    //Duration、Delay
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(duration, GUILayout.Width(labelWidth));
                        var newDuration = EditorGUILayout.FloatField(animation.scale.duration);
                        if (newDuration != animation.scale.duration)
                        {
                            Undo.RecordObject(target, "Scale Duration");
                            animation.scale.duration = newDuration;
                            EditorUtility.SetDirty(target);
                        }

                        GUILayout.Label(delay, GUILayout.Width(labelWidth - 20f));
                        var newDelay = EditorGUILayout.FloatField(animation.scale.delay);
                        if (newDelay != animation.scale.delay)
                        {
                            Undo.RecordObject(target, "Scale Delay");
                            animation.scale.delay = newDelay;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //From
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(from, GUILayout.Width(labelWidth));
                        Vector3 newStartValue = EditorGUILayout.Vector3Field(GUIContent.none, animation.scale.startValue);
                        if (newStartValue != animation.scale.startValue)
                        {
                            Undo.RecordObject(target, "Scale From");
                            animation.scale.startValue = newStartValue;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //To
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(to, GUILayout.Width(labelWidth));
                        Vector3 newEndValue = EditorGUILayout.Vector3Field(GUIContent.none, animation.scale.endValue);
                        if (newEndValue != animation.scale.endValue)
                        {
                            Undo.RecordObject(target, "Scale To");
                            animation.scale.endValue = newEndValue;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();

                    //Ease
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(ease, GUILayout.Width(labelWidth));
                        var newEase = (Ease)EditorGUILayout.EnumPopup(animation.scale.ease);
                        if (newEase != animation.scale.ease)
                        {
                            Undo.RecordObject(target, "Scale Ease");
                            animation.scale.ease = newEase;
                            EditorUtility.SetDirty(target);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }
    public override void OnPreviewSettings()
    {
        if (GUILayout.Button(EditorGUIUtility.IconContent(
            !isPlaying ? "PlayButton" : "PauseButton",
            !isPlaying ? "|开始预览" : "|停止预览"),
            EditorStyles.toolbarButton))
        {
            isPlaying = !isPlaying;
            //点击播放按钮 开始动画预览
            if (isPlaying)
            {
                Sequence sequence = DOTween.Sequence();
                //移动动画
                if (animation.move.toggle)
                {
                    sequence.Insert(0f, previewInstance.transform
                        .DOMove(animation.move.endValue,
                            animation.move.duration)
                        .SetEase(animation.move.ease)
                        .SetDelay(animation.move.delay)
                        .From(animation.move.startValue));
                }
                //旋转动画
                if (animation.rotate.toggle)
                {
                    sequence.Insert(0f, previewInstance.transform
                        .DORotate(animation.rotate.endValue,
                            animation.rotate.duration, animation.rotate.mode)
                        .SetEase(animation.rotate.ease)
                        .SetDelay(animation.rotate.delay)
                        .From(animation.rotate.startValue));
                }
                //缩放动画
                if (animation.scale.toggle)
                {
                    sequence.Insert(0f, previewInstance.transform
                        .DOScale(animation.scale.endValue,
                            animation.scale.duration)
                        .SetEase(animation.scale.ease)
                        .SetDelay(animation.scale.delay)
                        .From(animation.scale.startValue));
                }
                //循环播放
                sequence.SetLoops(-1);
                //开始预览
                DG.DOTweenEditor.DOTweenEditorPreview
                    .PrepareTweenForPreview(sequence);
                DG.DOTweenEditor.DOTweenEditorPreview.Start();
            }
            //点击停止按钮 停止动画预览
            else
            {
                DG.DOTweenEditor.DOTweenEditorPreview.Stop();
                //重置坐标、旋转、缩放
                previewInstance.transform.position = Vector3.zero;
                previewInstance.transform.rotation = Quaternion.identity;
                previewInstance.transform.localScale = Vector3.one;
            }
        }
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        //获取各类型输入
        OnPreviewGUIInput(r, ref dragRot, ref distance);
        //重绘事件
        if (Event.current.type == EventType.Repaint)
        {
            //开启预览区域
            previewRenderUtility.BeginPreview(r, background);
            //调整相机旋转、坐标
            Camera camera = previewRenderUtility.camera;
            camera.transform.rotation = Quaternion.Euler(
                new Vector3(-dragRot.y, -dragRot.x, 0));
            camera.transform.position = camera.transform.forward * -distance;
            //相机相关设置
            EditorUtility.SetCameraAnimateMaterials(camera, true);
            camera.cameraType = CameraType.Preview;
            camera.enabled = false;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 30f;
            camera.farClipPlane = 50f;
            camera.nearClipPlane = 2f;
            //相机渲染
            camera.Render();
            //结束并预览
            previewRenderUtility.EndAndDrawPreview(r);

            EditorGUI.LabelField(new Rect(r.x, r.y, r.width, 20f), 
                string.Format("Position：{0}",
                    previewInstance.transform.position));
            EditorGUI.LabelField(new Rect(r.x, r.y + 20f, r.width, 20f), 
                string.Format("Rotation：{0}", 
                    previewInstance.transform.eulerAngles));
            EditorGUI.LabelField(new Rect(r.x, r.y + 40f, r.width, 20f),
                string.Format("Scale：{0}", 
                    previewInstance.transform.localScale));
        }
    }
    //预览窗口中的输入事件
    private void OnPreviewGUIInput(Rect r, ref Vector2 dragRot, 
        ref float distance)
    {
        int hashCode = GetType().GetHashCode();
        int controlID = GUIUtility.GetControlID(hashCode, FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID)) 
        {
            case EventType.MouseDown:
                //鼠标按下并且是预览窗口区域
                if (r.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    Event.current.Use();
                    //鼠标移出屏幕外后从另一侧移入
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                    GUIUtility.hotControl = 0;
                EditorGUIUtility.SetWantsMouseJumping(0);
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    dragRot -= Event.current.delta / 
                        Mathf.Min(r.width, r.height) * 140f;
                    Event.current.Use();
                    GUI.changed = true;
                }
                break;
            case EventType.ScrollWheel:
                distance += Event.current.delta.y * .1f;
                distance = Mathf.Clamp(distance, 3f, 30f);
                Event.current.Use();
                GUI.changed = true;
                break;
        }
    }
}