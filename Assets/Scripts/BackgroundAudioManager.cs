using UnityEngine;

public class BackgroundAudioManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

