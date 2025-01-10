using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip backgroundClip;
    public AudioClip coinsClip;
    public AudioClip cookingClip;
    public AudioClip popNotificationClip;
    public AudioClip levelClearClip;
    public AudioClip buttonClickedClip;

    public AudioSource backgroundAudioSource;

    private AudioSource effectsAudioSource; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        effectsAudioSource = gameObject.AddComponent<AudioSource>();

       
        backgroundAudioSource.clip = backgroundClip;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "Coins":
                effectsAudioSource.PlayOneShot(coinsClip);
                break;
            case "Cooking":
                effectsAudioSource.PlayOneShot(cookingClip);
                break;
            case "PopNotification":
                effectsAudioSource.PlayOneShot(popNotificationClip);
                break;
            case "LevelClear":
                effectsAudioSource.PlayOneShot(levelClearClip);
                break;
            case "ButtonClicked":
                effectsAudioSource.PlayOneShot(buttonClickedClip);
                break;
        }
    }

    public void SetVolume(float volume)
    {
        backgroundAudioSource.volume = volume;
        effectsAudioSource.volume = volume; 
    }

    public void MuteAudio(bool isMuted)
    {
        backgroundAudioSource.mute = isMuted;
        effectsAudioSource.mute = isMuted;
    }

    public void StopBackgroundAudio()
    {
        if (backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }
    }

    public void ResumeBackgroundAudio()
    {
        if (!backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
        }
    }

    public void StopCookingAudio()
    {
        if (effectsAudioSource.clip == cookingClip && effectsAudioSource.isPlaying)
        {
            effectsAudioSource.Stop();
        }
    }

}
