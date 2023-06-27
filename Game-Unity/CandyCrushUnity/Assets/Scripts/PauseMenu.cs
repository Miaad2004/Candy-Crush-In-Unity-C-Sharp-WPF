using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuOverlay;
    public static bool IsGamePaused { get; private set; } = false;
    private readonly KeyCode pauseKey = KeyCode.Escape;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (IsGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (GameTimer.IsTimeOver) return;

        pauseMenuOverlay.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuOverlay.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void QuitMidGame()
    {
        Application.Quit((int)UnityExitStatus.MidGameExit);
    }
}
