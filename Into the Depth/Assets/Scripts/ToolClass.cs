using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "item/Tool")]
public class ToolClass : ItemsClass
{
    [Header("Tool")]

    public ToolType toolType;
    public enum ToolType
    {
        drill
    } 

    public override ItemsClass GetItem() { return this; }
    public override ToolClass GetTool() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override ConsumableClass GetComsumable() { return null; }
}
