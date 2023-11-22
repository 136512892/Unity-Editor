using UnityEngine;
using UnityEditor;

public class MenuItemExample
{
    [MenuItem("Window/Function1 #E")]
    private static void Function1()
    {
        Debug.Log("Function1 Invoke.");
    }
    [MenuItem("Example/Function2 &R")]
    private static void Function2()
    {
        Debug.Log("Function2 Invoke.");
    }
    [MenuItem("Example/Function3 #&E")]
    private static void Function3()
    {
        Debug.Log("Function3 Invoke.");
    }

    [MenuItem("GameObject/LogName", priority = 0)]
    private static void LogName()
    {
        Debug.Log(Selection.activeGameObject.name);
    }
    [MenuItem("GameObject/LogName", true)]
    private static bool LogNameValidate()
    {
        return Selection.activeGameObject != null;
    }

    [MenuItem("Example/Function4", false, 20)]
    static void Function4() { }
    [MenuItem("Example/Function5", false, 10)]
    static void Function5() { }
    [MenuItem("Example/Function6", false, 31)]
    static void Function6() { }

    [MenuItem("GameObject/GoFunction1", false, -1000)]
    static void GoFunction1() { }
    [MenuItem("GameObject/GoFunction2", false, -100)]
    static void GoFunction2() { }
    [MenuItem("GameObject/GoFunction3", false, 0)]
    static void GoFunction3() { }
    [MenuItem("GameObject/GoFunction4", false, 10)]
    static void GoFunction4() { }
    [MenuItem("GameObject/GoFunction5", false, 49)]
    static void GoFunction5() { }
    [MenuItem("GameObject/GoFunction6", false, 50)]
    static void GoFunction6() { }
    [MenuItem("GameObject/GoFunction7", false, 100)]
    static void GoFunction7() { }

    [MenuItem("Assets/Function7")]
    private static void Function7() 
    {
        Debug.Log("Function7 Invoke.");
    }
    [MenuItem("Assets/Function8")]
    private static void Function8() 
    {
        Debug.Log("Function8 Invoke.");
    }
    [MenuItem("Assets/Function9")]
    private static void Function9() 
    {
        Debug.Log("Function9 Invoke.");
    }
}