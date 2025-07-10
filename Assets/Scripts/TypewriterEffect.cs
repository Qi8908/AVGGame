using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public float waitingSeconds = Constants.DEFAULT_TYPING_SECONDS;
    private Coroutine typingCoroutine;
    private bool isTyping;
    private float typingSpeed = 0.01f;

    public System.Action onTypingComplete;

    public void StartTyping(string text, float speed)
    {
        typingSpeed = speed;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        textDisplay.text = text;
        textDisplay.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++)
        {
            textDisplay.maxVisibleCharacters = i;
            yield return new WaitForSeconds(waitingSeconds);
        }
        isTyping = false;
        onTypingComplete?.Invoke();
    }

    public void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        textDisplay.maxVisibleCharacters = textDisplay.text.Length;
        isTyping = false;
    }

    public bool IsTyping() => isTyping;
}
