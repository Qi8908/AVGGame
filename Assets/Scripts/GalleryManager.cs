using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GalleryManager : MonoBehaviour
{
    public GameObject galleryPanel;
    public TextMeshProUGUI panelTitle;
    public Button[] galleryButtons;
    public Button prePageButton;
    public Button nextPageButton;
    public Button backButton;
    public GameObject bigImagePanel; // 大图面板
    public Image bigImage; // 显示大图的Image

    private int currentPage = Constants.DEFAULT_START_INDEX;
    private readonly int slotsPerPage = Constants.GALLERY_SLOTS_PER_PAGE;
    private readonly int totalSlots = Constants.ALL_HISTORY_IMAGES.Length;

    public static GalleryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        prePageButton.onClick.AddListener(PrevPage);
        nextPageButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(GoBack);
        galleryPanel.SetActive(false);
        panelTitle.text = Constants.GALLERY;
        bigImagePanel.SetActive(false);

        Button bigImageButton = bigImagePanel.GetComponent<Button>();
        if (bigImageButton != null)
        {
            bigImageButton.onClick.AddListener(HideBigImage);
        }
        else
        {
            Debug.LogWarning("Where is the Button of BigImagePanel?");
        }
    }

    public void ShowGalleryPanel()
    {
        UpdateUI();
        galleryPanel.SetActive(true);
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slotsPerPage; ++i)
        {
            int slotIndex = currentPage * slotsPerPage + i;
            if (slotIndex < totalSlots)
            {
                UpdateGalleryButtons(galleryButtons[i], slotIndex);
            }
            else
            {
                galleryButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateGalleryButtons(Button button, int index)
    {
        button.gameObject.SetActive(true);
        string historyName = Constants.ALL_HISTORY_IMAGES[index];
        bool isUnlocked = VNManager.Instance.unlockedHistoryImage.Contains(historyName);

        string imagePath = Constants.THUMBNAIL_PATH + (isUnlocked ? historyName : Constants.GALLERY_PLACEHOLDER);
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            button.GetComponentInChildren<Image>().sprite = sprite;
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(button, index));
    }

    private void OnButtonClick(Button button, int index)
    {
        string historyName = Constants.ALL_HISTORY_IMAGES[index];
        bool isUnlocked = VNManager.Instance.unlockedHistoryImage.Contains(historyName);

        if (!isUnlocked) // 如果没有解锁
        {
            return;
        }

        // 如果解锁了
        string imagePath = Constants.BIG_HISTORY_PATH + historyName;
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            bigImage.sprite = sprite;
            bigImagePanel.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.BIG_IMAGE_LOAD_FAILED + imagePath);
        }
    }

    private void HideBigImage()
    {
        bigImagePanel.SetActive(false);
    }

    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    private void NextPage()
    {
        if ((currentPage + 1) * slotsPerPage < totalSlots)
        {
            currentPage++;
            UpdateUI();
        }
    }

    private void GoBack()
    {
        galleryPanel.SetActive(false);
    }
}
