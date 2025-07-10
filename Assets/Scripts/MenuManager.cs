using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public AudioClip menuMusicClip;
    private AudioSource audioSource;
    public Button startButton;
    public Button continueButton;
    public Button loadButton;
    public Button galleryButton;
    public Button quitButton;
    public GameObject guidePanel;
    public Image guideImage;
    public List<Sprite> guideSprites;
    private int currentGuideIndex = 0;
    //private bool hasStarted = false;
    public static MenuManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        InitializeAudio();
        guidePanel.SetActive(false);
    }

    void InitializeAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusicClip;
        audioSource.loop = true;
    }

    void Start()
    {
        menuButtonAddListener();

        if (menuMusicClip != null)
        {
            audioSource.Play();
        }
    }

    public void StopMenuMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void menuButtonAddListener()
    {
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        loadButton.onClick.AddListener(LoadGame);
        galleryButton.onClick.AddListener(ShowGalleryPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        // hasStarted = true;
        // VNManager.Instance.StartGame(Constants.DEFAULT_STORY_FILE_NAME, Constants.DEFAULT_START_LINE);
        // StopMenuMusic();
        // ShowGamePanel();

        currentGuideIndex = 0;
        guideImage.sprite = guideSprites[currentGuideIndex];  // 显示第一张引导图
        guidePanel.SetActive(true);

        startButton.interactable = false;
        menuPanel.SetActive(true);
    }

    public void OnGuidePanelClicked()
    {
        currentGuideIndex++;

        if (currentGuideIndex >= guideSprites.Count)
        {
            // 引导页播放完毕，正式进入游戏
            //hasStarted = true;
            guidePanel.SetActive(false);
            VNManager.Instance.StartGame(Constants.DEFAULT_STORY_FILE_NAME, Constants.DEFAULT_START_LINE);
            StopMenuMusic();
            ShowGamePanel();
        }
        else
        {
            // 切换到下一张引导图
            guideImage.sprite = guideSprites[currentGuideIndex];
        }
    }

    private void ContinueGame()
    {
        //if (hasStarted)
        //{
        //ShowGamePanel();
        //}
        if (VNManager.Instance != null && !string.IsNullOrEmpty(VNManager.Instance.lastPlayedStoryFileName))
        {
            menuPanel.SetActive(false);
            VNManager.Instance.gamePanel.SetActive(true);
            VNManager.Instance.InitializeAndLoadStory(VNManager.Instance.lastPlayedStoryFileName, VNManager.Instance.lastPlayedLine);
        }
        else
        {
            Debug.LogWarning("ContinueGame: No previous game state found.");
        }
    }

    private void LoadGame()
    {
        VNManager.Instance.ShowLoadPanel(ShowGamePanel);
    }

    private void ShowGamePanel()
    {
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
    }

    private void ShowGalleryPanel()
    {
        GalleryManager.Instance.ShowGalleryPanel();
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        // 如果在 Unity 编辑器里运行，则停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
