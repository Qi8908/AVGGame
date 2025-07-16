using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Schema;
using ExcelDataReader.Log.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject saveLoadPanel;
    public TextMeshProUGUI panelTitle;
    public Button[] saveLoadButtons;
    public Button backButton;

    private bool isSave;
    private readonly int saveSlots = Constants.SAVE_SLOTS;
    private readonly int totalSlots = Constants.TOTAL_SLOTS;
    private System.Action<int> currentAction;
    private System.Action menuAction;

    // 单例模式
    public static SaveLoadManager Instance { get; private set; }

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
        backButton.onClick.AddListener(GoBack);
        saveLoadPanel.SetActive(false);
    }

    public void ShowSavePanel(System.Action<int> action)
    {
        isSave = true;
        panelTitle.text = Constants.SAVE_GAME;
        currentAction = action;
        UpdateUI();
        saveLoadPanel.SetActive(true);
    }

    public void ShowLoadPanel(System.Action<int> action, System.Action menuAction)
    {
        isSave = false;
        panelTitle.text = Constants.LOAD_GAME;
        currentAction = action;
        this.menuAction = menuAction;
        UpdateUI();
        saveLoadPanel.SetActive(true);
    }

    private void UpdateUI()
    {
        for (int i = 0; i < saveLoadButtons.Length; ++i)
        {
            if (i < totalSlots)
            {
                UpdateSaveLoadButtons(saveLoadButtons[i], i);
                LoadStorylineAndScreenshots(saveLoadButtons[i], i);
                saveLoadButtons[i].gameObject.SetActive(true);
            }
            else
            {
                saveLoadButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /*private void UpdateSaveLoadButtons(Button button, int index)
    {
        button.gameObject.SetActive(true);
        button.interactable = true;

        var savePath = GenerateDataPath(index);
        var fileExists = File.Exists(savePath);

        if (!isSave && !fileExists)
        {
            button.interactable = false;
        }

        var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
        textComponents[0].text = null;
        textComponents[1].text = (index + 1) + Constants.COLON + Constants.EMPTY_SLOT;
        button.GetComponentInChildren<RawImage>().texture = null;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(button, index));
    }*/

    private void UpdateSaveLoadButtons(Button button, int index)
    {
        button.gameObject.SetActive(true);

        var savePath = GenerateDataPath(index);
        var fileExists = File.Exists(savePath);

        var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
        var image = button.GetComponentInChildren<RawImage>();

        if (fileExists)
        {
            // ✅ 有存档：设置默认文字、等待 LoadStorylineAndScreenshots 填入截图和文字
            button.interactable = true;
            textComponents[0].text = ""; // 内容文本
            textComponents[1].text = (index + 1) + Constants.COLON + Constants.EMPTY_SLOT; // 时间或 slot 名称
            image.texture = null;
        }
        else
        {
            // ✅ 无存档：显示占位图 + Locked 文本
            button.interactable = true;

            // 设置“Locked”文字
            textComponents[0].text = "Empty Slot";
            textComponents[1].text = "";

            // 加载 Resources 中的占位图
            Texture2D placeholder = Resources.Load<Texture2D>(Constants.THUMBNAIL_PATH + Constants.SAVE_PLACEHOLDER);
            if (placeholder != null)
            {
                image.texture = placeholder;
            }
            else
            {
                Debug.LogWarning("占位图未找到：" + Constants.THUMBNAIL_PATH + Constants.SAVE_PLACEHOLDER);
                image.texture = null;
            }
        }

        // 更新点击事件
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(button, index));
    }


    private void OnButtonClick(Button button, int index)
    {
        menuAction?.Invoke();
        currentAction?.Invoke(index);
        if (isSave)
        {
            LoadStorylineAndScreenshots(button, index);
        }
        else
        {
            GoBack();
        }
    }

    private void LoadStorylineAndScreenshots(Button button, int index)
    {
        var savePath = GenerateDataPath(index);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<VNManager.SaveData>(json);
            if (saveData.screenShotData != null)
            {
                Texture2D screenShot = new Texture2D(2, 2);
                screenShot.LoadImage(saveData.screenShotData);
                button.GetComponentInChildren<RawImage>().texture = screenShot;
            }
            if (saveData.savedSpeakingContent != null)
            {
                // var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();

                // string shortContent = saveData.savedSpeakingContent.Length > 20
                //                         ? saveData.savedSpeakingContent.Substring(0, 20) + "..."
                //                         : saveData.savedSpeakingContent;

                // textComponents[0].text = shortContent;
                // textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G"); // G 表示不同的时间格式
                var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();

                string cleanedContent = System.Text.RegularExpressions.Regex.Replace(
                    saveData.savedSpeakingContent,
                    @"<color=.*?>|</color>",
                    ""
                );
                string shortContent = cleanedContent.Length > 27
                                        ? cleanedContent.Substring(0, 27) + "..."
                                        : cleanedContent;

                textComponents[0].text = shortContent;
                textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G");
            }
        }
    }

    private string GenerateDataPath(int index)
    {
        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, index + Constants.SAVE_FILE_EXTENSION);
    }

    private void GoBack()
    {
        saveLoadPanel.SetActive(false);
    }
}
