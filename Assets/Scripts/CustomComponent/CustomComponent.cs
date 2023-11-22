using UnityEngine;

public class CustomComponent : MonoBehaviour
{
    public int intValue;
    [SerializeField] private string stringValue;
    [SerializeField] private bool boolValue;
    [SerializeField] private GameObject go;

    public enum ExampleEnum
    {
        Enum1,
        Enum2,
        Enum3,
    }
    [SerializeField] private ExampleEnum enumValue;
}