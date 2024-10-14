using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlertSkin", menuName = "New AlertSkin")]
public class AlertSkin : ScriptableObject
{
    public Sprite background;
    public Color backgroundColor;
    public Sprite buttonSprite;
    public Sprite mainButtonSprite;
    public Color textColor;
    public Color buttonTextColor;
    public Color mainButtonTextColor;
}
