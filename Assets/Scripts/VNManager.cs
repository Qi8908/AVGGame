using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakingContent;
    public TypewriterEffect typewriterEffect;
    public Image avatarImage;
    public AudioSource vocalAudio;
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public GameObject speakerPanel;
    public Image characterImage1;
    public Image characterImage2;
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public GameObject bottomButtons;
    public Button autoButton;
    public Button skipButton;
    public GameObject topButtons;
    public Button saveButton;
    public Button loadButton;
    public Button historyButton;
    public Button settingsButton;
    public Button homeButton;
    //public Button closeButton;

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine;
    private string currentStoryFileName;

    private bool isAutoPlay = false;
    private bool isSkip = false;
    private int maxReachedLineIndex = 0;
    private Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>(); // 全局储存每个文件的最远行索引

    public static VNManager Instance { get; private set; }
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
        BottomButtonsAddListener();
        TopButtonsAddListener();
        gamePanel.SetActive(false);
    }

    void Update()
    {
        if (gamePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsHittingBottomButtons() && !IsHittingTopButtons())
            {
                if (!SaveLoadManager.Instance.saveLoadPanel.activeSelf)
                {
                    DisplayNextLine();
                }
            }
        }
    }

    void InitializeAndLoadStory(string fileName)
    {
        Initialize();
        LoadStoryFromFile(fileName);
        DisplayNextLine();
        typewriterEffect.onTypingComplete = StopVocalAudio;
    }

    void BottomButtonsAddListener()
    {
        autoButton.onClick.AddListener(OnAutoButtonClick);
        skipButton.onClick.AddListener(OnSkipButtonClick);
    }

    void TopButtonsAddListener()
    {
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    public void StartGame()
    {
        InitializeAndLoadStory(defaultStoryFileName);
    }

    void Initialize()
    {
        currentLine = Constants.DEFAULT_START_LINE;

        avatarImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        choicePanel.SetActive(false);
    }

    void LoadStoryFromFile(string fileName)
    {
        currentStoryFileName = fileName;
        var path = storyPath + fileName + excelFileExtension;
        storyData = ExcelReader.ReadExcel(path);
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
        if (globalMaxReachedLineIndices.ContainsKey(currentStoryFileName))
        {
            maxReachedLineIndex = globalMaxReachedLineIndices[currentStoryFileName];
        }
        else
        {
            maxReachedLineIndex = 0;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }
    }

    void DisplayNextLine()
    {
        if (currentLine > maxReachedLineIndex)
        {
            maxReachedLineIndex = currentLine;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }

        if (currentLine >= storyData.Count - 1)
        {
            if (isAutoPlay)
            {
                isAutoPlay = false;
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);
            }
            if (storyData[currentLine].speakerName == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
                return;
            }
            if (storyData[currentLine].speakerName == Constants.CHOICE)
            {
                ShowChoices();
                return;
            }
        }
        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
            StopVocalAudio();
        }
        else
        {
            DisplayThisLine();
        }
    }

    void DisplayThisLine()
    {
        var data = storyData[currentLine];
        //speakerName.text = data.speakerName;
        speakingContent.text = data.speakingContent;
        typewriterEffect.StartTyping(data.speakingContent);

        // Avatar
        if (NotNullNorEmpty(data.avatarImageFileName))
        {
            UpdateAvatarImage(data.avatarImageFileName);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }

        // SpeakerName(RQ)
        if (NotNullNorEmpty(data.speakerName))
        {
            if (speakerName != null)
                speakerName.text = data.speakerName;

            if (speakerPanel != null)
                speakerPanel.SetActive(true);
        }
        else
        {
            if (speakerName != null)
                speakerName.text = "";

            if (speakerPanel != null)
                speakerPanel.SetActive(false);
        }

        // Vocal
        if (NotNullNorEmpty(data.vocalAudioFileName))
        {
            PlayVocalAudio(data.vocalAudioFileName);
        }

        // BG
        if (NotNullNorEmpty(data.backgroundImageFileName))
        {
            UpdateBackgroundImage(data.backgroundImageFileName);
        }

        // BGM
        if (NotNullNorEmpty(data.backgroundMusicFileName))
        {
            PlayBackgroundMusic(data.backgroundMusicFileName);
        }

        // Character Action
        if (NotNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName, characterImage1, data.coordinateX1);
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName, characterImage2, data.coordinateX2);
        }

        currentLine++;
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    void UpdateImage(string imagePath, Image image)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }

    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            if (audioSource == vocalAudio)
            {
                Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
            }
            else if (audioSource == backgroundMusic)
            {
                Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
            }
        }
    }

    void UpdateAvatarImage(string imageFileName)
    {
        string imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }

    void PlayVocalAudio(string audioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + audioFileName;
        PlayAudio(audioPath, vocalAudio, false);
    }

    void StopVocalAudio()
    {
        if (vocalAudio.isPlaying)
        {
            vocalAudio.Stop();
        }
    }

    void UpdateBackgroundImage(string imageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        UpdateImage(imagePath, backgroundImage);
    }

    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
    }

    void UpdateCharacterImage(string action, string imageFileName, Image characterImage, string x)
    {
        if (action.StartsWith(Constants.Appear_At)) // 解析appearAt(x,y)动作并在该位置显示立绘
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            if (NotNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var newPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = newPosition;
                characterImage.DOFade(1, Constants.DURATION_TIME).From(0); // 淡出效果
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }

        }
        else if (action == Constants.Disappear) // 隐藏角色立绘
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false));
        }
        else if (action.StartsWith(Constants.Move_To)) // 解析moveTo(x,y)动作并移动立绘
        {
            if (NotNullNorEmpty(x))
            {
                characterImage.rectTransform.DOAnchorPosX(float.Parse(x), Constants.DURATION_TIME);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
    }

    void ShowChoices()
    {
        var data = storyData[currentLine];

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choicePanel.SetActive(true);

        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.speakingContent; // 选项1文本
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName)); // 选项1跳转文件

        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.vocalAudioFileName; // 选项2文本
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundImageFileName)); // 选项2跳转文件
    }

    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null
        );
    }

    bool IsHittingTopButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            topButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null
        );
    }

    void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;
        UpdateButtonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);
        if (isAutoPlay)
        {
            StartCoroutine(StartAutoPlay());
        }
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffect.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.AUTOPLAY_WAITING_SECONDS);
        }
    }

    void OnSkipButtonClick()
    {
        if (!isSkip && CanSkip())
        {
            StartSkip();
        }
        else if (isSkip)
        {
            StopCoroutine(SkipToMaxReachedLine());
            EndSkip();
        }
    }

    bool CanSkip()
    {
        return currentLine < maxReachedLineIndex;
    }

    void StartSkip()
    {
        isSkip = true;
        UpdateButtonImage(Constants.SKIP_ON, skipButton);
        typewriterEffect.typingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipToMaxReachedLine());
    }

    private IEnumerator SkipToMaxReachedLine()
    {
        while (isSkip)
        {
            if (CanSkip())
            {
                DisplayThisLine();
            }
            else
            {
                EndSkip();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_SECONDS);
        }
    }

    void EndSkip()
    {
        isSkip = false;
        typewriterEffect.typingSpeed = Constants.DEFAULT_TYPING_SECONDS;
        UpdateButtonImage(Constants.SKIP_OFF, skipButton);
    }

    void OnSaveButtonClick()
    {
        SaveLoadManager.Instance.ShowSaveLoadUI(true);
    }

    void OnLoadButtonClick()
    {
        SaveLoadManager.Instance.ShowSaveLoadUI(false);
    }

    void UpdateButtonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }

    void OnHomeButtonClick()
    {
        gamePanel.SetActive(false);
        MenuManager.Instance.menuPanel.SetActive(true);
    }
}

