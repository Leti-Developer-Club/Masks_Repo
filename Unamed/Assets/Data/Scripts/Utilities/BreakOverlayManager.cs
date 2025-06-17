using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class BreakOverlayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image slideImage;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private TMP_Text continueIndicator;
    [SerializeField] private GameObject slideshowCanvas;

    [Header("Slideshow Content")]
    public Sprite[] slides;
    [TextArea] public string[] titles;
    [TextArea] public string[] bodyTexts;

    [Header("Timing")]
    [SerializeField] private float breakDuration = 10f;
    [SerializeField] private float timePerSlide = 3f;
    [SerializeField] private float fadeDuration = 3f;

    public void StartBreakOverlay(float timeer)
    {
        StartCoroutine(DelayedBreakStart(timeer));
    }

    IEnumerator PlayBreakSequence()
    {
        int count = Mathf.Min(slides.Length, titles.Length);
        for (int i = 0; i < count; i++)
        {
            yield return StartCoroutine(FadeInSlideWithText(slides[i], titles[i], bodyTexts[i]));
            yield return new WaitForSeconds(timePerSlide);
        }

        float timeLeft = breakDuration;
        while (timeLeft > 0)
        {
            continueIndicator.text = $"Next Wave in {Mathf.Ceil(timeLeft)}...";
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        slideshowCanvas.SetActive(false);
    }
    private IEnumerator DelayedBreakStart(float timeer)
    {
        yield return new WaitForSeconds(timeer); // wait the passed-in time
        slideshowCanvas.SetActive(true);
        StartCoroutine(PlayBreakSequence());
    }
    private IEnumerator FadeInSlideWithText(Sprite newSprite, string title, string body)
    {
        // Set content
        slideImage.sprite = newSprite;
        titleText.text = title;
        bodyText.text = body;

        // Start with full transparent colors
        slideImage.color = new Color(1f, 1f, 1f, 0f);
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f);
        bodyText.color = new Color(bodyText.color.r, bodyText.color.g, bodyText.color.b, 0f);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);

            slideImage.color = new Color(1f, 1f, 1f, alpha);
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, alpha);
            bodyText.color = new Color(bodyText.color.r, bodyText.color.g, bodyText.color.b, alpha);

            yield return null;
        }
    }

}

