using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int winAmount = 20; 
    public Text cashRegisterUI; 
    public string[] levels; 
    public static int currentLevelIndex = 0; 

    [Header("UI Panels")]
    public GameObject levelCompletePanel;
    public GameObject gameWinPanel; 

    void Start()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false); 

        if (gameWinPanel != null)
            gameWinPanel.SetActive(false);
    }

    void Update()
    {
        if (cashRegisterUI != null)
        {
            int currentAmount = int.Parse(cashRegisterUI.text.Split('$')[1]);

            if (currentAmount >= winAmount)
            {
                HandleLevelComplete();
            }
        }
    }

    void HandleLevelComplete()
    {
        Debug.Log("Level Cleared!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("LevelClear");
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance is null. Ensure AudioManager exists in the scene.");
        }

        if (currentLevelIndex < levels.Length - 1)
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }
        }
        else
        {
            if (gameWinPanel != null)
            {
                gameWinPanel.SetActive(true);
            }
        }
    }
    public void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level1")
        {
            Debug.Log("Loading Level 2...");
            SceneManager.LoadScene("Level2");
        }
        else if (currentSceneName == "Level2")
        {
            Debug.Log("Loading Level 3...");
            SceneManager.LoadScene("Level3");
        }
        else if (currentSceneName == "Level3")
        {
            Debug.Log("Game Complete! No more levels.");
            SceneManager.LoadScene("GameWinScene"); 
        }
        else
        {
            Debug.LogWarning("Unknown level. Unable to load the next level.");
        }
    }

    //public void LoadNextLevel()
    //{
    //    if (levels == null || levels.Length == 0)
    //    {
    //        Debug.LogError("No levels defined in the Levels array.");
    //        return;
    //    }

    //    if (currentLevelIndex < levels.Length - 1)
    //    {
    //        currentLevelIndex++;
    //        string nextLevelName = levels[currentLevelIndex];

    //        Debug.Log("Loading Level: " + nextLevelName);
    //        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No more levels to load! Current Level Index: " + currentLevelIndex);
    //    }
    //}



    public void RestartCurrentLevel()
    {
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(levels[0]);
    }
}


