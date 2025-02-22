using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotClass 
{
    private ItemsClass item;
    private int quantity;

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
}
