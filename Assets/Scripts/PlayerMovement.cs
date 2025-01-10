using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigidbodynew;
    public float speed = 3.0f;
    float xMovement;
    float yMovement;

    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private GameObject currentCustomer = null;
    private GameObject currentChair = null;   
    private GameObject foodReady = null;     

    public Transform kitchenSpawnPoint; 
    public GameObject saladPrefab; 
    public GameObject saladBubblePrefab; 
    private GameObject playerSaladBubble = null;

    public Slider foodPreparationSlider;
    private float foodPreparationTime = 3f; 
    private float foodPreparationTimer = 0f;

    private bool isWaitingForFood = false;
    private bool isDeliveringSalad = false;
    private GameObject pickedUpSalad = null; 

    public int cashRegisterAmount = 0; 
    public UnityEngine.UI.Text cashRegisterUI;
    private GameObject currentTable = null; 


    void Start()
    {
        rigidbodynew = GetComponent<Rigidbody2D>();

        if (foodPreparationSlider == null)
        {
        
            foodPreparationSlider = GameObject.Find("FoodPreparationSlider").GetComponent<Slider>();
            if (foodPreparationSlider == null)
            {
                Debug.LogError("FoodPreparationSlider not found in the scene!");
            }
        }

        if (foodPreparationSlider != null)
        {
            foodPreparationSlider.gameObject.SetActive(false); 
        }

    }

    private void spriteFlip(float xMovement)
    {
        spriteRenderer.flipX = xMovement < 0;
    }

    private void FixedUpdate()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        animator.SetFloat("moving", Mathf.Abs(xMovement * speed) + Mathf.Abs(yMovement * speed));
        transform.Translate(new Vector3(xMovement * speed, yMovement * speed, 0f) * Time.deltaTime);

        spriteFlip(xMovement);
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", Mathf.Abs(xMovement));
        animator.SetFloat("Vertical", yMovement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteractions();
        }
    }
    private void HandleInteractions()
    {
        if (currentCustomer != null && currentChair != null)
        {
            Debug.Log("Handling interaction for customer: " + currentCustomer.name + " and chair: " + currentChair.name);
            AudioManager.Instance.PlaySound("PopNotification");

            // Seat the customer at the chair
            Vector3 chairPosition = currentChair.transform.position;
            currentCustomer.transform.position = new Vector3(chairPosition.x, chairPosition.y + 0.6f, chairPosition.z);

            // Assign the table linked to the chair to the customer
            Chair chairScript = currentChair.GetComponent<Chair>();
            if (chairScript != null && chairScript.linkedTable != null)
            {
                Table linkedTable = chairScript.linkedTable.GetComponent<Table>();
                Customer customerScript = currentCustomer.GetComponent<Customer>();
                if (customerScript != null)
                {
                    customerScript.linkedTable = linkedTable;
                    customerScript.Seated(); // Mark the customer as seated
                    Debug.Log("Customer seated: " + currentCustomer.name + " at table: " + linkedTable.name);
                }
                else
                {
                    Debug.LogWarning("Customer script is missing on the current customer!");
                }
            }
            else
            {
                Debug.LogWarning("Chair script or linked table is missing!");
            }

            currentCustomer = null;
            currentChair = null;

            //=============dor
            CustomerQueueManager customerQueueManager = FindObjectOfType<CustomerQueueManager>();
            if (customerQueueManager != null)
            {
                customerQueueManager.OnCustomerSeated(); // Notify about seating
            }
            //==============
        }
        else if (currentCustomer != null && currentCustomer.GetComponent<Customer>().IsWaitingForOrder)
        {
            // Take the order
            AudioManager.Instance.PlaySound("PopNotification");

            Debug.Log("Taking order from customer: " + currentCustomer.name);
            currentCustomer.GetComponent<Customer>().OrderTaken();
            StartFoodPreparation();
        }
        else if (foodReady != null && !isDeliveringSalad)
        {
            Debug.Log("Picking up salad.");
            PickUpSalad();
        }
        else if (currentTable != null)
        {
            Table table = currentTable.GetComponent<Table>();
            if (table != null)
            {
                table.PlayerCleanTable(); 
                Debug.Log("Cleaning table: " + currentTable.name);
                currentTable = null; 
            }
        }

        if (currentCustomer == null)
        {
            Debug.LogWarning("No current customer selected!");
        }
        if (currentChair == null)
        {
            Debug.LogWarning("No chair selected!");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Customer"))
        {
            currentCustomer = collision.gameObject;
            Debug.Log(currentCustomer);
        }
        else if (collision.gameObject.CompareTag("Chair"))
        {
            currentChair = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("Money"))
        {
            var moneyComponent = collision.gameObject.GetComponent<Money>();
            if (moneyComponent != null)
            {
                cashRegisterAmount += moneyComponent.value;
                cashRegisterUI.text = "Cash: $" + cashRegisterAmount;

                CustomerQueueManager queueManager = FindObjectOfType<CustomerQueueManager>();
                if (queueManager != null)
                {
                    queueManager.AddMoney(moneyComponent.value);
                }

                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Salad"))
        {
            if (pickedUpSalad == null)
            {
                pickedUpSalad = collision.gameObject;
                Debug.Log("Picked up salad!");
            }
        }
        else if (collision.gameObject.CompareTag("Table") && isDeliveringSalad)
        {
            // Deliver salad to the customer's table
            DeliverSaladToCustomerTable(collision.transform.position);
        }
        else if (collision.gameObject.CompareTag("Table"))
        {
            // Track the table the player is near
            currentTable = collision.gameObject;
        }
    }

    //=========
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            currentTable = null;
        }
    }
  

    private void StartFoodPreparation()
    {
        if (!isWaitingForFood)
        {
            isWaitingForFood = true;
            foodPreparationTimer = 0f;

            AudioManager.Instance.PlaySound("Cooking");


            if (foodPreparationSlider != null)
            {
                foodPreparationSlider.gameObject.SetActive(true); 
                foodPreparationSlider.maxValue = foodPreparationTime; 
                foodPreparationSlider.value = 0f; 
            }
            Debug.Log("Food preparation started...");
            StartCoroutine(FoodCookingTimer());
        }
    }

    private void DeliverSaladToCustomerTable(Vector3 tablePosition)
    {
        if (playerSaladBubble != null)
        {
            Destroy(playerSaladBubble);
            playerSaladBubble = null;
        }

        // Spawn the salad GameObject on the table
        if (saladPrefab != null)
        {
            GameObject salad = Instantiate(saladPrefab, new Vector3(tablePosition.x, tablePosition.y + 0.4f, tablePosition.z), Quaternion.identity);

            Table table = FindObjectOfType<Table>();
            if (table != null)
            {
                table.FoodDelivered(salad); 
            }
            else
            {
                Debug.LogWarning("No table found to deliver salad!");
            }
        }
        else
        {
            Debug.LogError("Salad prefab is not assigned in the Inspector!");
        }

        if (currentCustomer != null)
        {
            currentCustomer.GetComponent<Customer>().ServeFood();
        }

        isDeliveringSalad = false;
        Debug.Log("Salad delivered to customer's table!");
    }


    public void PickUpSalad()
    {
        if (foodReady != null)
        {
            Debug.Log("Picking up the salad from the kitchen.");

            Destroy(foodReady);

            if (saladBubblePrefab != null)
            {
                playerSaladBubble = Instantiate(saladBubblePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                playerSaladBubble.transform.SetParent(transform); 
            }

            isDeliveringSalad = true;
            Debug.Log("Player is now delivering salad!");
        }
        else
        {
            Debug.LogWarning("No salad available to pick up!");
        }
    }


    private IEnumerator FoodCookingTimer()
    {
        while (foodPreparationTimer < foodPreparationTime)
        {
            foodPreparationTimer += Time.deltaTime;

            if (foodPreparationSlider != null)
            {
                foodPreparationSlider.value = foodPreparationTimer;
            }

            yield return null; 
        }

        if (saladPrefab != null)
        {
            foodReady = Instantiate(saladPrefab, kitchenSpawnPoint.position, Quaternion.identity); 
            Debug.Log("Food is ready! Salad instantiated at kitchen spawn point.");
        }
        else
        {
            Debug.LogError("Salad prefab is not assigned in the inspector!");
        }

        AudioManager.Instance.StopCookingAudio();

        isWaitingForFood = false;

        if (foodPreparationSlider != null)
        {
            foodPreparationSlider.gameObject.SetActive(false); 
        }
    }

}
