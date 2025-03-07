using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemsClass itemData;
    public int amount = 1;

    private void Start()
    {
        gameObject.tag = "Item";
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError($"DroppedItem на {gameObject.name} не имеет Collider2D!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>(); 
            if (inventory != null && itemData != null)
            {
                bool added = inventory.Add(itemData, amount);
                if (added)
                {
                    Debug.Log($"Игрок подобрал {itemData.itemName} x{amount}");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Инвентарь заполнен!");
                }
            }
            else
            {
                Debug.LogError("InventoryManager не найден или itemData = null!");
            }
        }
    }

}
