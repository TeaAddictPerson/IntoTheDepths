using System.Collections;
using UnityEngine;

public abstract class ItemsClass : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    public Sprite itemIcon;
    public bool IsStackable = true;
    public abstract ItemsClass GetItem();
    public abstract ToolClass GetTool();
    public abstract MiscClass GetMisc();
    public abstract ConsumableClass GetComsumable();
}
