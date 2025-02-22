using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemsClass itemToAdd;
    public ItemsClass itemToRemove;
  public List<ItemsClass> items = new List<ItemsClass>();

  public void Start()
  {
    items.Add(itemToAdd);
    items.Remove(itemToRemove);
  }

  public void Add(ItemsClass item)
  {
    items.Add(item);
  }

  public void Remove(ItemsClass item)
  {
    items.Remove(item);
  }
}
