using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemsClass itemData; 
    public int amount = 1;

    void Start()
    {
        gameObject.tag = "Item"; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = other.GetComponent<InventoryManager>();
            if (inventory != null && itemData != null)
            {
                inventory.Add(itemData, amount); 
                Destroy(gameObject); 
            }
        }
    }
}
