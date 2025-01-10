using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float levelTime = 180f; 
    private float currentTime;

    [Header("UI Elements")]
    public Text timerText; 
    public GameObject timesUpPanel;
    public Button retryButton; 

    private bool isTimeUp = false;

    void Start()
    {
        currentTime = levelTime;
        UpdateTimerUI();

        if (timesUpPanel != null)
            timesUpPanel.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);
    }

    void Update()
    {
        if (!isTimeUp)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                TimeUp();
            }
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format as MM:SS
    }

    void TimeUp()
    {
        isTimeUp = true;

        if (timesUpPanel != null)
            timesUpPanel.SetActive(true);

        Debug.Log("Time's Up! Displaying panel.");
    }

    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
