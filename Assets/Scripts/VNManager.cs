using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    #region Variables
    public GameObject gamePanel;
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerName;
    public TypewriterEffect typewriterEffect;
    public ScreenShotter screenShotter;
    public Image avatarImage;
    public AudioSource vocalAudio;
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public GameObject speakerPanel;
    public Image characterImage1;
    public Image characterImage2;
    public Image historyImage;

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

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly int defaultStartLine = Constants.DEFAULT_START_LINE;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;

    private string saveFolderPath;
    private byte[] screenShotData; // 保存截图数据
    private string currentSpeakingContent; // 保存当前对话内容

    private List<ExcelReader.ExcelData> storyData;
    private int currentLine;
    private string currentStoryFileName;
    private float currentTypingSpeed = Constants.DEFAULT_TYPING_SECONDS;

    private bool isAutoPlay = false;
    private bool isSkip = false;
    private bool isLoad = false;
    private int maxReachedLineIndex = 0;

    private Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>(); // 全局储存每个文件的最远行索引
    private LinkedList<string> historyRecords = new LinkedList<string>();
    public HashSet<string> unlockedHistoryImage = new HashSet<string>(); // 保存已经解锁的历史图片
    public static VNManager Instance { get; private set; }
    #endregion

    #region LifeCycle
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
        InitializeSaveFilePath();

        BottomButtonsAddListener();
        TopButtonsAddListener();
    }

    void Update()
    {
        if (!MenuManager.Instance.menuPanel.activeSelf && !SaveLoadManager.Instance.saveLoadPanel.activeSelf && !HistoryManager.Instance.historyScrollView.activeSelf && gamePanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (!dialogueBox.activeSelf)
                {
                    OpenUI();
                }
                else if (!IsHittingBottomButtons() && !IsHittingTopButtons())
                {
                    DisplayNextLine();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (dialogueBox.activeSelf)
                {
                    CloseUI();
                }
                else
                {
                    OpenUI();
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                CtrlSkip();
            }
        }
    }
    #endregion

    #region Initialization
    void InitializeSaveFilePath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath); // 新建一个存档文件夹
        }
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
        historyButton.onClick.AddListener(OnHistoryButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    public void StartGame(string fileName, int startLine)
    {
        InitializeAndLoadStory(defaultStoryFileName, defaultStartLine);
    }

    void InitializeAndLoadStory(string fileName, int lineNumber)
    {
        Initialize(lineNumber);
        //LoadStoryFromFile(fileName);
        StartCoroutine(LoadStoryFromStreamingAssets(fileName));
        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }
        DisplayNextLine();
        typewriterEffect.onTypingComplete = StopVocalAudio;
    }

    void Initialize(int line)
    {
        currentLine = line;

        backgroundImage.gameObject.SetActive(false);
        backgroundMusic.gameObject.SetActive(false);

        avatarImage.gameObject.SetActive(false);
        vocalAudio.gameObject.SetActive(false);

        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);

        historyImage.gameObject.SetActive(false);

        choicePanel.SetActive(false);
    }

    //void LoadStoryFromFile(string fileName)
    //{
    //currentStoryFileName = fileName;
    //var path = storyPath + fileName + excelFileExtension;
    //storyData = ExcelReader.ReadExcel(path);
    //if (storyData == null || storyData.Count == 0)
    //{
    //Debug.LogError(Constants.NO_DATA_FOUND);
    //}
    //if (globalMaxReachedLineIndices.ContainsKey(currentStoryFileName))
    //{
    //maxReachedLineIndex = globalMaxReachedLineIndices[currentStoryFileName];
    //}
    //else
    //{
    //maxReachedLineIndex = 0;
    //globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
    //}
    //}

    IEnumerator LoadStoryFromStreamingAssets(string fileName)
    {
        currentStoryFileName = fileName;
        string path = Path.Combine(Application.streamingAssetsPath, fileName + excelFileExtension);
        string tempPath = Path.Combine(Application.persistentDataPath, fileName + excelFileExtension);

#if UNITY_ANDROID && !UNITY_EDITOR
    byte[] fileData;
    UnityWebRequest www = UnityWebRequest.Get(path);
    yield return www.SendWebRequest();
    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Failed to load Excel file: " + www.error);
        yield break;
    }
    fileData = www.downloadHandler.data;
    File.WriteAllBytes(tempPath, fileData);
    storyData = ExcelReader.ReadExcel(tempPath);
#else
        if (!File.Exists(path))
        {
            Debug.LogError("Excel file not found: " + path);
            yield break;
        }
        storyData = ExcelReader.ReadExcel(path);
#endif

        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
            yield break;
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

        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }

        DisplayNextLine();
        typewriterEffect.onTypingComplete = StopVocalAudio;
    }

    #endregion

    #region Display
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
            }
            if (storyData[currentLine].speakerName == Constants.CHOICE)
            {
                ShowChoices();
            }
            if (storyData[currentLine].speakerName == Constants.GOTO)
            {
                InitializeAndLoadStory(storyData[currentLine].speakingContent, defaultStartLine);
            }
            return;
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
        speakerName.text = data.speakerName;
        currentSpeakingContent = data.speakingContent;
        typewriterEffect.StartTyping(currentSpeakingContent, currentTypingSpeed);

        RecordHistory(speakerName.text, currentSpeakingContent); // 记录历史文本

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

        // History Image
        if (NotNullNorEmpty(data.historyAction))
        {
            UpdateHistoryImage(data.historyAction, data.historyImageFileName, historyImage);
        }

        currentLine++;
    }

    void RecordHistory(string speaker, string content)
    {
        string historyRecord;

        if (!string.IsNullOrEmpty(speaker))
        {
            historyRecord = speaker + Constants.COLON + content; // 有角色名，加冒号
        }
        else
        {
            historyRecord = content; // 没有角色名（旁白），直接用内容
        }

        if (historyRecords.Count >= Constants.MAX_LENGTH)
        {
            historyRecords.RemoveFirst();
        }
        historyRecords.AddLast(historyRecord);
    }


    void RecoverLastBackgroundAndCharacter()
    {
        var data = storyData[currentLine];
        if (NotNullNorEmpty(data.lastBackgroundImage))
        {
            UpdateBackgroundImage(data.lastBackgroundImage);
        }
        if (NotNullNorEmpty(data.lastBackgroundMusic))
        {
            PlayBackgroundMusic(data.lastBackgroundMusic);
        }
        if (data.character1Action != Constants.APPEAR_AT && NotNullNorEmpty(data.character1ImageFileName))
        {
            UpdateCharacterImage(Constants.APPEAR_AT, data.character1ImageFileName, characterImage1, data.lastCoordinateX1);
        }
        if (data.character2Action != Constants.APPEAR_AT && NotNullNorEmpty(data.character2ImageFileName))
        {
            UpdateCharacterImage(Constants.APPEAR_AT, data.character2ImageFileName, characterImage2, data.lastCoordinateX2);
        }
    }

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }
    #endregion

    #region Choices
    void ShowChoices()
    {
        var data = storyData[currentLine];

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choicePanel.SetActive(true);

        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.speakingContent; // 选项1文本
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName, defaultStartLine)); // 选项1跳转文件

        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.vocalAudioFileName; // 选项2文本
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundImageFileName, defaultStartLine)); // 选项2跳转文件
    }
    #endregion

    #region Audios
    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
        }
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

    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
    }
    #endregion

    #region Images
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

    void UpdateAvatarImage(string imageFileName)
    {
        string imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }

    void UpdateBackgroundImage(string imageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imageFileName;
        UpdateImage(imagePath, backgroundImage);
    }

    void UpdateCharacterImage(string action, string imageFileName, Image characterImage, string x)
    {
        // 根据Action执行对应的动画或操作
        if (action.StartsWith(Constants.APPEAR_AT)) // 解析appearAt(x,y)动作并在该位置显示立绘
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            if (NotNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var newPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = newPosition;

                var duration = Constants.DURATION_TIME;
                if (isLoad || action == Constants.APPEAR_AT_INSTANTLY)
                {
                    duration = 0;
                }
                characterImage.DOFade(1, duration).From(0);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }

        }
        else if (action == Constants.DISAPPEAR) // 隐藏角色立绘
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false));
        }
        else if (action.StartsWith(Constants.MOVE_TO)) // 解析moveTo(x,y)动作并移动立绘
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

    void UpdateHistoryImage(string action, string imageFileName, Image historyImage)
    {
        if (action.StartsWith(Constants.APPEAR_AT))
        {
            string imagePath = Constants.HISTORY_PATH + imageFileName;
            UpdateImage(imagePath, historyImage);
            if (!unlockedHistoryImage.Contains(imageFileName))
            {
                unlockedHistoryImage.Add(imageFileName);
                Debug.Log("历史知识图片已记录");
            }
        }
        else if (action == Constants.DISAPPEAR)
        {
            historyImage.gameObject.SetActive(false);
        }
    }

    void UpdateButtonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }
    #endregion

    #region Buttons
    #region Top & Bottom
    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main
        );
    }

    bool IsHittingTopButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            topButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            Camera.main
        );
    }
    #endregion
    #region Auto
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
    #endregion
    #region Skip
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
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
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
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
        }
    }

    void EndSkip()
    {
        isSkip = false;
        currentTypingSpeed = Constants.DEFAULT_TYPING_SECONDS;
        UpdateButtonImage(Constants.SKIP_OFF, skipButton);
    }

    void CtrlSkip()
    {
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipWhilePressingCtrl());
    }

    private IEnumerator SkipWhilePressingCtrl()
    {
        while (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            DisplayNextLine();
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
        }
    }
    #endregion
    #region Save & Load
    public class SaveData
    {
        public string savedStoryFileName;
        public int savedLine;
        public string savedSpeakingContent;
        public byte[] screenShotData;
        public LinkedList<string> savedHistoryRecords;
    }
    void OnSaveButtonClick()
    {
        CloseUI();
        Texture2D screenShot = screenShotter.CaptureScreenShot(); // 截取当前画面并生成 Texture2D
        screenShotData = screenShot.EncodeToPNG(); // 将 Texture2D 转换为 PNG 格式的字节数组
        SaveLoadManager.Instance.ShowSavePanel(SaveGame);
        OpenUI();
    }

    void SaveGame(int slotIndex)
    {
        var saveData = new SaveData
        {
            savedStoryFileName = currentStoryFileName,
            savedLine = currentLine,
            savedSpeakingContent = currentSpeakingContent,
            screenShotData = screenShotData,
            savedHistoryRecords = historyRecords // 历史记录也要跟随每个存档
        };
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    void OnLoadButtonClick()
    {
        ShowLoadPanel(null);
    }

    public void ShowLoadPanel(Action action)
    {
        SaveLoadManager.Instance.ShowLoadPanel(LoadGame, action);
    }

    void LoadGame(int slotIndex)
    {
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        if (File.Exists(savePath))
        {
            isLoad = true;
            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            historyRecords = saveData.savedHistoryRecords;
            historyRecords.RemoveLast(); // 加载存档时移除最后一条历史记录
            MenuManager.Instance.StopMenuMusic();
            var lineNumber = saveData.savedLine - 1;
            InitializeAndLoadStory(saveData.savedStoryFileName, lineNumber);
        }
    }
    #endregion
    #region Home
    void OnHomeButtonClick()
    {
        gamePanel.SetActive(false);
        MenuManager.Instance.menuPanel.SetActive(true);
    }

    #endregion
    #region OpenUI & CloseUI
    void OpenUI()
    {
        dialogueBox.SetActive(true);
        bottomButtons.SetActive(true);
        topButtons.SetActive(true);
    }

    void CloseUI()
    {
        dialogueBox.SetActive(false);
        bottomButtons.SetActive(false);
        topButtons.SetActive(false);
    }
    #endregion 
    #region History
    void OnHistoryButtonClick()
    {
        bottomButtons.SetActive(false);
        topButtons.SetActive(false);
        HistoryManager.Instance.ShowHistory(historyRecords);
    }
    #endregion
    #endregion
}

