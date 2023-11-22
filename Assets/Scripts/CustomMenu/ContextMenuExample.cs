using UnityEngine;

public class ContextMenuExample : MonoBehaviour
{
    [ContextMenu("Function1")]
    private void Function1()
    {
        Debug.Log("TODO Something.");
    }
}