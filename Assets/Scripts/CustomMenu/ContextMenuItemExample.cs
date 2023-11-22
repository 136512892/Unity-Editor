using UnityEngine;

public class ContextMenuItemExample : MonoBehaviour
{
    [ContextMenuItem("Reset", "ResetSize")]
    public int size = 0;

    void ResetSize()
    {
        size = 0;
    }
}