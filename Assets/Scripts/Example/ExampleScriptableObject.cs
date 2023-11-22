using UnityEngine;

[CreateAssetMenu(fileName = "New Example SO",
    menuName = "Example SO", order = 0)]
public class ExampleScriptableObject : ScriptableObject
{
    public string title;
    public string description;
}