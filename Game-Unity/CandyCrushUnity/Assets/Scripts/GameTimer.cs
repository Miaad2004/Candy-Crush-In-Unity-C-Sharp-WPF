using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public float TotalTimeInSeconds;
 
    [SerializeField] private GameObject timeOverOverlay;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioClip winSoundEffect;
    [SerializeField] private AudioSource audioSource;

    [Header("Timer Settings")]
    public float currentTime;

    public static bool IsTimeOver { get; private set; } = false;

    public int FinalScore => ScoreCounter.Instance.Score;

    private void Awake() => Instance = this;

    // Start is called before the first frame update
    void Start()
    {
        IsTimeOver = false;
        timeOverOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        float remainingTime = Mathf.Max(0f, TotalTimeInSeconds - currentTime);
        int remainingMinutes = Mathf.FloorToInt(remainingTime / 60f);
        int remainingSeconds = Mathf.FloorToInt(remainingTime % 60f);
        int totalMinutes = Mathf.FloorToInt(TotalTimeInSeconds / 60f);
        int totalSeconds = Mathf.FloorToInt(TotalTimeInSeconds % 60f);
        timerText.SetText($"Time: {remainingMinutes:00}:{remainingSeconds:00}/{totalMinutes:00}:{totalSeconds:00}");

        if (remainingTime <= 0f)
        {
            OnTimeOver();
        }

    }

    public async void OnTimeOver()
    {
        if (IsTimeOver) return;

        IsTimeOver = true;
        finalScoreText.SetText($"{FinalScore} Points");
        BackgroundAudio.Instance.StopPlaying();
        audioSource.PlayOneShot(winSoundEffect);
        timeOverOverlay.SetActive(true);
        try
        {
            await LoginSystemConnectionHandeler.Instance.SendScoreToWPF();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }

    public void QuitGameOnTimeOver()
    {
        Application.Quit((int)UnityExitStatus.Success);
    }
}
