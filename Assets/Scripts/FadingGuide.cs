using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class FadingGuide : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI guideText;
    public Image Guide;
    
    [Header("Timing Settings")]
    public float displayTime = 5f;
    public float fadeDuration = 1f;
    
    void Start()
    {
        StartCoroutine(ShowAndFadeInstructions());
    }
    
    IEnumerator ShowAndFadeInstructions()
    {
        guideText.gameObject.SetActive(true);
        guideText.alpha = 1f;

        if (Guide != null)
        {
            Guide.gameObject.SetActive(true);
            Guide.color = new Color(Guide.color.r, Guide.color.g, Guide.color.b, 1f);
        }
        
        yield return new WaitForSeconds(displayTime);
        
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            guideText.alpha = alpha;
            if (Guide != null)
            {
                Guide.color = new Color(Guide.color.r, Guide.color.g, Guide.color.b, alpha);
            }
            yield return null;
        }
        
        guideText.gameObject.SetActive(false);
        if (Guide != null)
        {
            Guide.gameObject.SetActive(false);
        }
    }
}