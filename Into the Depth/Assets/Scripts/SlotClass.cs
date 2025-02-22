using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotClass 
{
    [SerializeField] private ItemsClass item;
    [SerializeField] private int quantity;

    public SlotClass()
    {
        item=null;
        quantity=0;
    }
    public SlotClass(ItemsClass _item, int _quantity)
    {
        item=_item;
        quantity=_quantity;
    }

    public ItemsClass GetItem() { return item; }
    public int GetQuantity() { return quantity; }
    public void AddQuantity(int _quantity) { quantity += _quantity;}
    public void SubQuantity(int _quantity) { quantity -= _quantity; }
}
