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

    Add(itemToAdd);
    Remove(itemToRemove);
  }

  public void RefreshUI()
{
    for (int i = 0; i < slots.Length; i++)
    {
        try
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;   

            if(items[i].GetItem().IsStackable)
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = items[i].GetQuantity() + ""; 
            else
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
        catch
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = ""; 
        }
    }
}

  public bool Add(ItemsClass item)
  {
    

    SlotClass slot = Contains(item);
    if(slot!=null && slot.GetItem().IsStackable)
    {
       slot.AddQuantity(1);
    }    
   
    else
    {
            if(items.Count<slots.Length)
                items.Add(new SlotClass(item,1));
            else
            {
                return false;
            }
    }

    RefreshUI();
        return true;
  }

  public bool Remove(ItemsClass item)
  {
        SlotClass temp = Contains(item);
        if (temp != null)
        {
            if(temp.GetQuantity()>1)
                temp.SubQuantity(1);
            else
            {
                SlotClass slotToRemove = new SlotClass();

                foreach (SlotClass slot in items)
                {
                    if (slot.GetItem() == item)
                    {
                        slotToRemove = slot;
                        break;
                    }
                }
                items.Remove(slotToRemove);
            }
        }

        else
        {
            return false; 
        }
          
        RefreshUI();
        return true;
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
