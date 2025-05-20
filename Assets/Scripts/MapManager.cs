using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel;
    public Button closeButton;

    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mapPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseMap);
    }

    public void ShowMap()
    {
        mapPanel.SetActive(true);
    }

    public void CloseMap()
    {
        VNManager.Instance.bottomButtons.SetActive(true);
        VNManager.Instance.topButtons.SetActive(true);
        mapPanel.SetActive(false);
    }
}
