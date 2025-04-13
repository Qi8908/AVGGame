using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button startButton;
    public Button continueButton;
    public Button loadButton;
    public Button settingsButton;
    public Button quitButton;

    private bool hasStarted = false;
    public static MenuManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        menuButtonAddListener();
    }

    void menuButtonAddListener()
    {
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
    }

    private void StartGame()
    {
        hasStarted = true;
        VNManager.Instance.StartGame();
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
    }

    private void ContinueGame()
    {
        if (hasStarted)
        {
            menuPanel.SetActive(false);
            VNManager.Instance.gamePanel.SetActive(true);
        }
    }
}
