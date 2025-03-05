using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{
    public ItemsClass itemData; 
    public float pickupRadius = 2f;
    private Vector3 initialPosition; 

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerScript>().SetNearbyItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerScript>().SetNearbyItem(null);
        }
    }

    public void PickUpItem(InventoryManager inventory)
    {
        if (inventory.Add(itemData, 1)) 
        {
            StartCoroutine(RespawnItem()); 
            gameObject.SetActive(false); 
        }
    }

    private IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(5);
        gameObject.SetActive(true); 
        transform.position = initialPosition; 
    }
}
