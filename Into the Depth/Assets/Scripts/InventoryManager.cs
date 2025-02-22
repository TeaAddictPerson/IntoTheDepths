using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private ItemsClass itemToAdd;
    [SerializeField] private ItemsClass itemToRemove;
  public List<SlotClass> items = new List<SlotClass>();

  private GameObject[] slots;

  public void Start()
  {
    slots = new GameObject[slotHolder.transform.childCount];
    for(int i=0; i<slotHolder.transform.childCount; i++)
        slots[i]= slotHolder.transform.GetChild(i).gameObject;
    
    RefreshUI();
    items.Add(itemToAdd);
    items.Remove(itemToRemove);
  }

  public void RefreshUI()
{
    for (int i = 0; i < slots.Length; i++)
    {
        try
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;     
            slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = items[i].GetQuantity() + ""; 
        }
        catch
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = ""; 
        }
    }
}

  public void Add(ItemsClass item)
  {
 
    SlotClass slot = Contains(item);
        if(slot!=null)
    slot.AddQuantity(1);
    else
    {
        items.Add(new SlotClass(item,1));
    }

    RefreshUI();
  }

  public void Remove(ItemsClass item)
  {
 
    SlotClass slotToRemove = new SlotClass();

    foreach(SlotClass slot in items)
    {
        if(slot.GetItem()==item)
        {
            slotToRemove = slot;
            break;
        }
    }
    items.Remove(slotToRemove);

    RefreshUI();
  }

  public SlotClass Contains(ItemsClass item)
  {
     foreach(SlotClass slot in items)
    {
        if(slot.GetItem() == item)
        return slot;
    }

    return null;
  }
}
