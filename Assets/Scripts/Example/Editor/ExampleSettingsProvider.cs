using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ExampleSettingsProvider : SettingsProvider
{
    [SettingsProvider]
    private static SettingsProvider CreateProvider()
    {
        return new ExampleSettingsProvider(
            "Project/Custom/Example Settings", SettingsScope.Project);
    }

    public ExampleSettingsProvider(string path, 
        SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords) { }
    
    public override void OnGUI(string searchContext)
    {
        GUILayout.Label("Hello World.", EditorStyles.boldLabel);
        GUILayout.Button("Button");
    }
}