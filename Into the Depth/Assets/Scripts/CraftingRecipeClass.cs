using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCraftingRecipe",menuName ="Crafting/Recipe")] 
public class CraftingRecipeClass : ScriptableObject
{
    public SlotClass[] inputItems;
    public SlotClass[] outputItem;

    public bool CanCraft(InventoryManager inventory)
    {
        if(inventory.IsFull())
            return false;


        for(int i=0; i<inputItems.Length;i++)
        {
            if(!inventory.Contains(inputItems[i].GetItem(), inputItems[i].GetQuantity()))
            {
                return false;
            }
        }

        return true;
    }
    public void Craft(InventoryManager inventory)
    {
        for (int i = 0; i < inputItems.Length; i++)
        {
            inventory.Remove(inputItems[i].GetItem(), inputItems[i].GetQuantity());
        }

        for (int i = 0; i < outputItem.Length; i++)
        {
            inventory.Add(outputItem[i].GetItem(), outputItem[i].GetQuantity());
        }
    }

}
