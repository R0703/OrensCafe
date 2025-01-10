using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting the game...");
        SceneManager.LoadScene("Level1");
        AudioManager.Instance.PlaySound("ButtonClicked");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
        AudioManager.Instance.PlaySound("ButtonClicked");
    }
}
