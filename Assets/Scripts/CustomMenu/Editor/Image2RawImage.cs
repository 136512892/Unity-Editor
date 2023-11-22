using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Image2RawImage
{
    [MenuItem("CONTEXT/Image/Convert2RawImage")]
    public static void Execute()
    {
        GameObject selected = Selection.activeGameObject;
        //获取选中物体的Image组件
        Image image = selected.GetComponent<Image>();
        //缓存Sprite
        Sprite sprite = image.sprite;
        //销毁Image组件
        Object.DestroyImmediate(image);
        //添加RawImage组件
        RawImage rawImage = selected.AddComponent<RawImage>();
        //如果Sprite不为null 将其texture赋值给RawImage的texture
        if (sprite != null)
            rawImage.texture = sprite.texture;
    }
}