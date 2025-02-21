using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName = "item/Consumable")]
public class ConsumableClass : ItemsClass
{
    [Header("Consumable")]
    public float healthAdded;
    public override ItemsClass GetItem() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override ConsumableClass GetComsumable() { return this; }
}
