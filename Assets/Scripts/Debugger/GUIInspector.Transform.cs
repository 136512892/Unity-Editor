using UnityEngine;

[ComponentGUIInspector(typeof(Transform))]
public class TransformGUIInspector : ComponentGUIInspector
{
    protected override void OnDraw(Component component)
    {
        Transform transform = component.transform;

        transform.localPosition = DrawVector3("Position", 
            transform.localPosition, 125f, Screen.width * .03f);

        transform.localEulerAngles = DrawVector3("Rotation",
            transform.localEulerAngles, 125f, Screen.width * .03f);

        transform.localScale = DrawVector3("Scale", 
            transform.localScale, 125f, Screen.width * .03f);
    }
}