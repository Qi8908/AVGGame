using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspectManager : MonoBehaviour
{
    public GameObject suspectPanel;
    public Button closeButton;

    public static SuspectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        suspectPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseSuspect);
    }

    public void ShowSuspect()
    {
        Debug.Log("ShowSuspect方法触发了");
        suspectPanel.SetActive(true);
    }

    public void CloseSuspect()
    {
        VNManager.Instance.bottomButtons.SetActive(true);
        VNManager.Instance.topButtons.SetActive(true);
        suspectPanel.SetActive(false);
    }
}
