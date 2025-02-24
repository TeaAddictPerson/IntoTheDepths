using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemCursor;
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private ItemsClass itemToAdd;
    [SerializeField] private ItemsClass itemToRemove;

    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] items;

    private GameObject[] slots;

    private SlotClass movingSlot;
    private SlotClass TempSlot;
    private SlotClass originalSlot;
    bool IsMovingItem;
    private void Start()
  {
    slots = new GameObject[slotHolder.transform.childCount];
    items = new SlotClass[slots.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i]=new SlotClass();
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }

        for (int i=0; i<slotHolder.transform.childCount; i++)
        slots[i]= slotHolder.transform.GetChild(i).gameObject;
    
    RefreshUI();

    Add(itemToAdd,1);
    Remove(itemToRemove);
  }

    private void Update()
    {
        itemCursor.SetActive(IsMovingItem);
        itemCursor.transform.position=Input.mousePosition;
        if (IsMovingItem)
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

         if(Input.GetMouseButtonDown(0)) 
         {
            if(IsMovingItem)
            {
                EndItemMove();
            }
            else 
                        BeginItemMove();
         }
    }

    #region Inventory Utils
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

  public bool Add(ItemsClass item, int quantity)
  {
    

    SlotClass slot = Contains(item);
    if(slot!=null && slot.GetItem().IsStackable)
    {
       slot.AddQuantity(1);
    }    
   
    else
    {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, quantity);
                    break;
                }
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
                int slotToRemoveIndex = 0;

                for(int i=0;i<items.Length; i++)    
                {
                    if (items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].Clear();
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
    for (int i = 0; i < items.Length; i++)
    {
        if (items[i].GetItem() == item)
            return items[i];

    }
        return null;
  }
    #endregion Inventory Utils

    #region Moving Stuff

    private bool BeginItemMove()
    {
        originalSlot = (GetClosestSlot());
        if (originalSlot == null || originalSlot.GetItem()==null)
            return false;

        movingSlot = new SlotClass(originalSlot.GetItem(), originalSlot.GetQuantity());
        originalSlot.Clear();
        IsMovingItem= true;
        RefreshUI();
        return true;
    }

    private bool EndItemMove()
    {
        originalSlot = (GetClosestSlot());
        if (originalSlot == null)
        {
            Add(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else
        {



            if (originalSlot.GetItem() != null)
            {

                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    if (originalSlot.GetItem().IsStackable)
                    {
                        originalSlot.AddQuantity(movingSlot.GetQuantity());
                        movingSlot.Clear();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    TempSlot = new SlotClass(originalSlot.GetItem(), originalSlot.GetQuantity());
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    movingSlot.AddItem(TempSlot.GetItem(), TempSlot.GetQuantity());
                    RefreshUI();
                    return true;
                }
            }
            else
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }
        }
        IsMovingItem= false;
        RefreshUI();
        return true;

    }

    private SlotClass GetClosestSlot()
    {
        Debug.Log(Input.mousePosition);

        for(int i=0; i<slots.Length;i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32)
                return items[i];
        }
        return null;
    }
    #endregion
}
