using UnityEngine;

public class Table : MonoBehaviour
{
    public bool HasFood = false;
    public GameObject foodOnTable;


    public void FoodDelivered(GameObject food)
    {
        Debug.Log("Food delivered to the table!");
        HasFood = true;
        foodOnTable = food;
    }


    public void ClearTable()
    {
        if (foodOnTable != null)
        {
            Debug.Log("Clearing table. Destroying food: " + foodOnTable.name);
            Destroy(foodOnTable); 
            foodOnTable = null;
        }
        else
        {
            Debug.LogWarning("No food to clear!");
        }
        HasFood = false; 
    }

    //public void PlayerCleanTable()
    //{
    //    Debug.Log("Player is manually cleaning the table.");
    //    ClearTable(); // Call the existing ClearTable logic
    //}

    public void PlayerCleanTable()
    {
        if (HasFood)
        {
            if (foodOnTable != null) 
            {
                Destroy(foodOnTable); // Destroy the food on the table
            }
            HasFood = false; // Update the table state
            Debug.Log("Table cleaned: " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("No food on table to clean!");
        }
    }

}
