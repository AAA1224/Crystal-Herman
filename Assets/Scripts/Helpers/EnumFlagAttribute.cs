using UnityEngine;

public class EnumFlagAttribute : PropertyAttribute
{
    public bool convertToInt = false;

    public EnumFlagAttribute() { }

    public EnumFlagAttribute(bool _convertToInt)
    {
        convertToInt = _convertToInt;
    }
}
