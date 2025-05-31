using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
