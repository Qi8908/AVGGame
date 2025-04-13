using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using ExcelDataReader.Log.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject saveLoadPanel;
    public TextMeshProUGUI panelTitle;
    public Button[] saveLoadButtons;
    public Button backButton;

    private bool isSave;
    private readonly int saveSlots = Constants.SAVE_SLOTS;
    private readonly int totalSlots = Constants.TOTAL_SLOTS;

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

    public void ShowSaveLoadUI(bool save)
    {
        isSave = save;
        panelTitle.text = isSave ? Constants.SAVE_GAME : Constants.LOAD_GAME;
        UpdateSaveLoadUI();
        saveLoadPanel.SetActive(true);
        LoadStorylineAndScreenshots();
    }

    private void UpdateSaveLoadUI()
    {
        for (int i = 0; i < totalSlots; i++)
        {
            saveLoadButtons[i].gameObject.SetActive(true);
            saveLoadButtons[i].interactable = true;

            // 更新保存栏位文本和图片
            string slotText = (i + 1) + Constants.COLON + Constants.EMPTY_SLOT;
            var textComponents = saveLoadButtons[i].GetComponentsInChildren<TextMeshProUGUI>();

            if (textComponents.Length >= 2)
            {
                textComponents[0].text = null;
                textComponents[1].text = slotText;
            }

            var previewImage = saveLoadButtons[i].GetComponentInChildren<RawImage>();
            if (previewImage != null)
            {
                previewImage.texture = null;
            }
        }

        for (int i = totalSlots; i < saveLoadButtons.Length; i++)
        {
            saveLoadButtons[i].gameObject.SetActive(false);
        }
    }

    private void GoBack()
    {
        saveLoadPanel.SetActive(false);
    }

    private void LoadStorylineAndScreenshots()
    {

    }
}
