using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryManager : MonoBehaviour
{
    public Transform historyContent;
    public GameObject historyItemPrefab;
    public GameObject historyScrollView;
    public Button closeButton;

    private LinkedList<string> historyRecords;

    public static HistoryManager Instance { get; private set; }

    private void Awake()
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
        historyScrollView.SetActive(false);
        closeButton.onClick.AddListener(CloseHistory);
    }

    public void ShowHistory(LinkedList<string> records)
    {
        // 清空现有的历史记录
        foreach (Transform child in historyContent)
        {
            Destroy(child.gameObject);
        }
        // 开始处理传过来的历史记录
        historyRecords = records;
        LinkedListNode<string> currentNode = historyRecords.Last; // 叠罗汉
        while (currentNode != null)
        {
            AddHistoryItem(currentNode.Value);
            currentNode = currentNode.Previous; // 移动到前一个节点
        }

        historyContent.GetComponent<RectTransform>().localPosition = Vector3.zero; // 将滚动视图的位置重置为顶部
        historyScrollView.SetActive(true);
    }

    private void AddHistoryItem(string text)
    {
        GameObject historyItem = Instantiate(historyItemPrefab, historyContent);
        historyItem.GetComponentInChildren<TextMeshProUGUI>().text = text;
        historyItem.transform.SetAsFirstSibling(); // 将历史记录项放在顶部
    }

    public void CloseHistory()
    {
        VNManager.Instance.bottomButtons.SetActive(true);
        VNManager.Instance.topButtons.SetActive(true);
        historyScrollView.SetActive(false);
    }
}
