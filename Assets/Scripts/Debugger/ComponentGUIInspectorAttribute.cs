using System;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentGUIInspectorAttribute : Attribute
{
    public Type ComponentType { get; private set; }

    public ComponentGUIInspectorAttribute(Type type)
    {
        ComponentType = type;
    }
}