using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerQueueManager : MonoBehaviour
{
    [Header("Customer Settings")]
    public GameObject rabbitPrefab;
    public GameObject pandaPrefab;
    public GameObject dogPrefab;
    public Transform spawnPoint; 
    public int maxCustomersInLevel = 2; 

    [Header("Money Settings")]
    public int moneyGoal = 20; 
    private int currentMoney = 0; 

    [Header("Level Complete Popup")]
    public GameObject levelCompletePopup; 
    public Text levelCompleteText;

    [Header("UI Settings")]
    public Text cashRegisterUI;

    private Queue<GameObject> customerQueue = new Queue<GameObject>();
    private int customersServed = 0; 
    private bool levelComplete = false; 

    void Start()
    {
        if (levelCompletePopup != null)
        {
            levelCompletePopup.SetActive(false);
        }

      
        StartCoroutine(SpawnInitialCustomers());
    }

    IEnumerator SpawnInitialCustomers()
    {
        for (int i = 0; i < maxCustomersInLevel; i++)
        {
            yield return new WaitForSeconds(1f); 
            SpawnRandomCustomer();
        }
    }

    public void SpawnRandomCustomer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Customer"))
            {
                Debug.LogWarning("Spawn point is occupied. Delaying spawn...");
                return; 
            }
        }

        GameObject customerPrefab = GetRandomCustomerPrefab();

        GameObject newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);

        Debug.Log("Spawning customer: " + customerPrefab.name);
        Debug.Log("Queue size after spawn: " + customerQueue.Count);

        customerQueue.Enqueue(newCustomer);
    }

    private GameObject GetRandomCustomerPrefab()
    {
        int randomIndex = Random.Range(0, 3); 
        switch (randomIndex)
        {
            case 0: return rabbitPrefab;
            case 1: return pandaPrefab;
            case 2: return dogPrefab;
            default: return rabbitPrefab;
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Money collected: " + currentMoney);

        if (cashRegisterUI != null)
        {
            cashRegisterUI.text = "Cash: $" + currentMoney;
        }

        if (currentMoney >= moneyGoal && !levelComplete)
        {
            StartCoroutine(HandleLevelComplete());
        }
    }

    IEnumerator HandleLevelComplete()
    {
        levelComplete = true;

        if (levelCompletePopup != null)
        {
            Debug.Log("Activating Level Complete popup...");
            levelCompletePopup.SetActive(true); 
            if (levelCompleteText != null)
            {
                levelCompleteText.text = "Level Complete! You collected $" + currentMoney;
            }
        }
        else
        {
            Debug.LogWarning("LevelCompletePopup is not assigned in the Inspector!");
        }

        yield break;
    }

    //public void LoadNextLevel()
    //{
    //    if (currentLevelIndex < levels.Length - 1)
    //    {
    //        currentLevelIndex++;
    //        Debug.Log("Loading Level: " + levels[currentLevelIndex]);
    //        SceneManager.LoadScene(levels[currentLevelIndex]);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No more levels to load!");
    //    }
    //}


    public void OnCustomerServed()
    {
        if (customerQueue.Count > 0)
        {
            GameObject servedCustomer = customerQueue.Dequeue();

            if (servedCustomer != null)
            {
                Debug.Log("Dequeueing customer: " + servedCustomer.name);
            }
            else
            {
                Debug.LogWarning("Served customer is null!");
            }
        }

        if (customersServed < maxCustomersInLevel)
        {
            SpawnRandomCustomer();
        }
    }


    public void OnCustomerSeated()
    {
        if (customerQueue.Count < maxCustomersInLevel)
        {
            Debug.Log("Spawning a new customer as one is seated.");
            SpawnRandomCustomer();
        }
        AudioManager.Instance.PlaySound("CafeAmbience");
    }


}
