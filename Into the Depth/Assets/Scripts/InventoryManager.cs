using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private ItemsClass itemToAdd;
    [SerializeField] private ItemsClass itemToRemove;
  public List<ItemsClass> items = new List<ItemsClass>();

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

    for(int i=0; i<slots.Length; i++)
    {
        try
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled=true;
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite=items[i].itemIcon;
        }
        catch
        {
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite=null;
            slots[i].transform.GetChild(0).GetComponent<Image>().enabled=false;
        }
        
    }
  }

  public void Add(ItemsClass item)
  {
    items.Add(item);
    RefreshUI();
  }

  public void Remove(ItemsClass item)
  {
    items.Remove(item);
    RefreshUI();
  }
}
