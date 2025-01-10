using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle muteToggle;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.backgroundAudioSource.volume;
            muteToggle.isOn = AudioManager.Instance.backgroundAudioSource.mute;
        }

        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetVolume);
        muteToggle.onValueChanged.AddListener(AudioManager.Instance.MuteAudio);

        settingsPanel.SetActive(false);
    }
    private void Update()
    {
   
        if (settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            ToggleSettingsPanel();
        }
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void ReplayLevel()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
