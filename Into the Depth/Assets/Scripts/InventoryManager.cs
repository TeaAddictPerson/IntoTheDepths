using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipeClass> craftingRecipes= new List<CraftingRecipeClass>();

    [SerializeField] private GameObject itemCursor;
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject hotbarslotHolder;
    [SerializeField] private ItemsClass itemToAdd;
    [SerializeField] private ItemsClass itemToRemove;
    [SerializeField] private GameObject itemPrefab; 


    [SerializeField] private GameObject InventoryUI;

    [SerializeField] private SlotClass[] startingItems;
    private SlotClass[] items;

    private GameObject[] slots;
    private GameObject[] hotbarSlots;

    private SlotClass movingSlot;
    private SlotClass TempSlot;
    private SlotClass originalSlot;
    bool IsMovingItem;

    [SerializeField] private int selectedSlotIndex = 0;
    [SerializeField] private GameObject hotbarSelector;
    public ItemsClass selectedItem;
    private void Start()
  {
    slots = new GameObject[slotHolder.transform.childCount];
    items = new SlotClass[slots.Length];

     InventoryUI.gameObject.SetActive(false);

    hotbarSlots = new GameObject[hotbarslotHolder.transform.childCount];
        for(int i=0;i<hotbarSlots.Length; i++)
        {
            hotbarSlots[i] = hotbarslotHolder.transform.GetChild(i).gameObject;
        }

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
        if(Input.GetKeyDown(KeyCode.C))
            Craft(craftingRecipes[4]);

        if(Input.GetKeyDown(KeyCode.B))
        {
            InventoryUI.gameObject.SetActive(!InventoryUI.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ThrowItem();
        }

        itemCursor.SetActive(IsMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (IsMovingItem)
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsMovingItem)
            {
                EndItemMove();
            }
            else
            {
                BeginItemMove();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (IsMovingItem)
            {
                EndItemMoveSingle();
            }
            else
            {
                BeginItemMoveHalf();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedSlotIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedSlotIndex--;
        }

 
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex, 0, hotbarSlots.Length - 1);

        hotbarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;

        int index = selectedSlotIndex + (hotbarSlots.Length * 3);
        if (index < items.Length)
        {
            selectedItem = items[index].GetItem();
        }
        else
        {
            selectedItem = null; 
        }
    }

    private void Craft(CraftingRecipeClass recipe)
    {
        if(recipe.CanCraft(this))
            recipe.Craft(this);
        else
        {
            Debug.Log("Нельзя скрафтить");
        }
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem()==null)
            {
                return false;
            }
        }
        return true;
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

        RefreshHotbar();
    }

    public void RefreshHotbar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            try
            {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i+ (hotbarSlots.Length*3)].GetItem().itemIcon;

                if (items[i + (hotbarSlots.Length * 3)].GetItem().IsStackable)
                    hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = items[i + (hotbarSlots.Length * 3)].GetQuantity() + "";
                else
                    slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
            catch
            {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
            }
        }

    }

  public bool Add(ItemsClass item, int quantity)
  {
    

    SlotClass slot = Contains(item);
    if(slot!=null && slot.GetItem().IsStackable)
    {
       slot.AddQuantity(quantity);
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

    private void ThrowItem()
    {
        Debug.Log("ThrowItem called");

        if (selectedItem == null)
        {
            Debug.Log("Selected item is null");
            return;
        }

        if (selectedItem is ConsumableClass || selectedItem is MiscClass)
        {
            Remove(selectedItem, 1);

           
            Vector3 spawnPosition = transform.position; 
            spawnPosition.z = 10; 

            GameObject itemObject = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = selectedItem.itemIcon;

            Rigidbody2D rb = itemObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1;
            rb.isKinematic = false;

   
            Vector2 throwDirection = transform.right; 
            rb.AddForce(throwDirection * 5f, ForceMode2D.Impulse); 

            Debug.Log("Spawned Item Position: " + spawnPosition);
        }
    }





    public bool Remove(ItemsClass item, int quantity)
{
    int totalRemoved = 0;
    for (int i = 0; i < items.Length; i++)
    {
        if (items[i].GetItem() == item)
        {
            int removeAmount = Mathf.Min(quantity - totalRemoved, items[i].GetQuantity());
            items[i].SubQuantity(removeAmount);
            totalRemoved += removeAmount;

            if (items[i].GetQuantity() == 0)
                items[i].Clear();

            if (totalRemoved >= quantity)
                break;
        }
    }

    RefreshUI();
    return totalRemoved >= quantity;
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

    public bool Contains(ItemsClass item, int quantity)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item && items[i].GetQuantity()>=quantity)
                return true;

        }
        return false;
    }


    #endregion Inventory Utils

    #region Moving Stuff

    private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem()==null)
            return false;

        movingSlot = new SlotClass(originalSlot.GetItem(), originalSlot.GetQuantity());
        originalSlot.Clear();
        IsMovingItem= true;
        RefreshUI();
        return true;
    }

private bool BeginItemMoveHalf()
{
    originalSlot = GetClosestSlot();
    if (originalSlot == null || originalSlot.GetItem() == null || originalSlot.GetQuantity() <= 1)
        return false; 

    int halfQuantity = Mathf.CeilToInt(originalSlot.GetQuantity() / 2f);
    
    movingSlot = new SlotClass(originalSlot.GetItem(), halfQuantity);
    originalSlot.SubQuantity(halfQuantity);
    
    if(originalSlot.GetQuantity() == 0)
        originalSlot.Clear();
    
    IsMovingItem = true;
    RefreshUI();
    return true;
}


    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();
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

     private bool EndItemMoveSingle()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot == null)
            return false;
        if (originalSlot.GetItem() !=null && originalSlot.GetItem() != movingSlot.GetItem())
        {
            return false;
        }
        movingSlot.SubQuantity(1);
        if (originalSlot.GetItem() != null && originalSlot.GetItem()== movingSlot.GetItem())
        {
            originalSlot.AddQuantity(1);
        }
        else
            originalSlot.AddItem(movingSlot.GetItem(),1);

        if(movingSlot.GetQuantity() < 1)
        {
            IsMovingItem=false;
            movingSlot.Clear();
        }
        else
            IsMovingItem = true;

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

    private void OnDrawGizmos()
    {
        if (selectedItem != null)
        {
            Gizmos.color = Color.red;


            Vector3 spawnPosition = transform.position + new Vector3(0, 1, 0);

            Gizmos.DrawSphere(spawnPosition, 0.2f); 
        }
    }

}
