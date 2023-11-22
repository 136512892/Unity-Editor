using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// 人物创建向导窗口
/// </summary>
public class AvatarCreateScriptableWizard : ScriptableWizard
{
    [MenuItem("Example/Avatar Creator")]
    public static void Open()
    {
        DisplayWizard<AvatarCreateScriptableWizard>(
            "Avatar Creator", "创建人物角色");
    }

    public AnimatorController animatorController;
    public bool applyRootMotion;

    private void OnWizardCreate()
    {
        //未选中任何对象
        if (Selection.activeGameObject == null)
            return;
        //实例化
        GameObject instance = Instantiate(Selection.activeGameObject);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        //为人物添加角色控制器组件
        instance.AddComponent<CharacterController>();
        //为人物添加自定义的人物驱动组件
        instance.AddComponent<AvatarController>();
        //为人物添加动画组件
        Animator animator = instance.GetComponent<Animator>();
        if (animator == null) animator = instance.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.applyRootMotion = applyRootMotion;
    }
}