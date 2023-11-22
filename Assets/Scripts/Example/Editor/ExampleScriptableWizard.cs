using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExampleScriptableWizard : ScriptableWizard
{


    [SerializeField] private string s;
    public string a;

    [MenuItem("Example/Example Scriptable Wizard")]
    public static void Open()
    {
        ScriptableWizard.DisplayWizard<ExampleScriptableWizard>("标题", "确认", "取消");
    }

    private void OnWizardCreate()
    {
        Debug.Log("确认");
    }

    private void OnWizardOtherButton()
    {
        Debug.Log("取消");
    }
}