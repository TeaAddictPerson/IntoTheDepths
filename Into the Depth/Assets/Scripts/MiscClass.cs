using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "item/Misc")]
public class MiscClass : ItemsClass
{
    public override ItemsClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return this; }
    public override ConsumableClass GetComsumable() { return null; }
}
