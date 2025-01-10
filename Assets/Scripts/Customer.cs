using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    private bool seated = false;
    public bool IsOrderReady = false;
    private bool foodServed = false;

    public GameObject orderBubblePrefab;
    private GameObject orderBubble;

    public GameObject moneyPrefab;

    public Sprite sittingSprite;
    public Sprite eatingSprite;

    public bool IsWaitingForOrder = true;
    public Table linkedTable;

    public void Seated()
    {
        if (!seated)
        {
            seated = true;
            ShowOrderBubble(); 
            IsWaitingForOrder = true; 
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && sittingSprite != null)
        {
            spriteRenderer.sprite = sittingSprite;
            Debug.Log("Sitting sprite updated to: " + sittingSprite.name);
        }
        else
        {
            Debug.LogWarning("Sitting sprite or SpriteRenderer is missing!");
        }
    }


    void ShowOrderBubble()
    {
        if (orderBubblePrefab != null)
        {
            orderBubble = Instantiate(orderBubblePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        //IsWaitingForOrder = true;
    }

    public void OrderTaken()
    {
        if (IsWaitingForOrder)
        {
            Debug.Log("Order has been taken!");
            IsWaitingForOrder = false;

            if (orderBubble != null)
            {
                Destroy(orderBubble);
                orderBubble = null;
            }

            IsOrderReady = true; 
        }
    }

    public void ServeFood()
    {
        if (IsOrderReady && !foodServed)
        {
            foodServed = true;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && eatingSprite != null)
            {
                spriteRenderer.sprite = eatingSprite;
            }
            Debug.Log("Customer started eating: " + gameObject.name);
            StartCoroutine(EatFood());
        }
        else
        {
            Debug.LogWarning("ServeFood called, but customer is not ready or already served.");
        }
    }

    IEnumerator EatFood()
    {
        Debug.Log("Customer is eating: " + gameObject.name);
        yield return new WaitForSeconds(5f);
        LeaveMoney();
    }

    void LeaveMoney()
    {
        Debug.Log("Customer is leaving money.");

        AudioManager.Instance.PlaySound("Coins");

        if (moneyPrefab != null)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        }

        // Signal the table to clear
        if (linkedTable != null)
        {
            linkedTable.ClearTable();
        }

        // Notify the CustomerQueueManager
        CustomerQueueManager queueManager = FindObjectOfType<CustomerQueueManager>();
        if (queueManager != null)
        {
            Debug.Log("LeaveMoney called. Notifying CustomerQueueManager.");
            queueManager.OnCustomerServed();
        }

        // Destroy this customer
        Destroy(gameObject);
    }
}
