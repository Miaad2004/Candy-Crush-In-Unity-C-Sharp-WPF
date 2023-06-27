using UnityEngine;

public sealed class BackgroundAudio : MonoBehaviour
{
    public static BackgroundAudio Instance { get; private set; }
    public static bool IsPlaying { get; private set; } = true;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Assuming the audio source is set to loop, so you don't need to manually restart it.
        audioSource.Play();
    }

    private void Update()
    {
        if (IsPlaying && (PauseMenu.IsGamePaused || GameTimer.IsTimeOver))
        {
            StopPlaying();
        }
        else if (!IsPlaying && (!PauseMenu.IsGamePaused && !GameTimer.IsTimeOver))
        {
            StartPlaying();
        }
    }

    public void StopPlaying()
    {
        Debug.Log("Stopped BG audio.");
        audioSource.Pause();
        IsPlaying = false;
    }

    public void StartPlaying()
    {
        Debug.Log("Started BG audio.");
        audioSource.UnPause();
        IsPlaying = true;
    }
}
