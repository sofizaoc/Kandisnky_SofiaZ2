using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour
{
    public CanvasGroup canvasGroup1;  // CanvasGroup for Canvas1
    public CanvasGroup canvasGroup2;  // CanvasGroup for Canvas2
    public CanvasGroup canvasGroupFigures;  // CanvasGroup for Canvas3 (renombrado a 'Figures')
    public float canvas1Duration = 10f;  // Duration for Canvas1 to stay visible
    public float canvas2Duration = 50f;  // Duration for Canvas2 to stay visible before moving to 'Figures' canvas
    public float fadeDuration = 1f;      // Duration of the fade effect (both fade-out and fade-in)
    public float zMovementDistance = 5f;  // How far the figures should move on the Z axis

    private float timer = 0f;           // Timer to track the duration
    private bool isFading = false;      // To prevent multiple fades happening at the same time
    private bool hasFadedToCanvas2 = false;  // To check if the fade to Canvas2 happened
    private bool hasFadedToFigures = false;  // To check if the fade to 'Figures' happened

    void Start()
    {
        // Initially show Canvas1 and hide Canvas2 and 'Figures' (Canvas3)
        canvasGroup1.alpha = 1f;
        canvasGroup1.interactable = true;
        canvasGroup1.blocksRaycasts = true;

        canvasGroup2.alpha = 0f;
        canvasGroup2.interactable = false;
        canvasGroup2.blocksRaycasts = false;

        canvasGroupFigures.alpha = 0f;
        canvasGroupFigures.interactable = false;
        canvasGroupFigures.blocksRaycasts = false;
    }

    void Update()
    {
        // Update the timer and check if it's time to start fading
        timer += Time.deltaTime;

        // Fade from Canvas1 to Canvas2 after 10 seconds
        if (timer >= canvas1Duration && !isFading && !hasFadedToCanvas2)
        {
            StartCoroutine(FadeCanvas(canvasGroup1, canvasGroup2));
            hasFadedToCanvas2 = true;  // Mark that the fade to Canvas2 has happened
        }

        // Fade from Canvas2 to 'Figures' after 50 seconds
        if (timer >= canvas1Duration + canvas2Duration && !isFading && !hasFadedToFigures)
        {
            StartCoroutine(FadeCanvas(canvasGroup2, canvasGroupFigures));
            StartCoroutine(MoveFiguresInZ());  // Start moving the figures in Z axis
            hasFadedToFigures = true;  // Mark that the fade to 'Figures' has happened
        }
    }

    // Coroutine to fade out one canvas and fade in the other
    IEnumerator FadeCanvas(CanvasGroup fadeOutCanvas, CanvasGroup fadeInCanvas)
    {
        isFading = true;

        // Fade out the first canvas
        float fadeOutTime = 0f;
        while (fadeOutTime < fadeDuration)
        {
            fadeOutTime += Time.deltaTime;
            fadeOutCanvas.alpha = 1f - Mathf.Clamp01(fadeOutTime / fadeDuration);
            yield return null;
        }
        fadeOutCanvas.alpha = 0f;
        fadeOutCanvas.interactable = false;
        fadeOutCanvas.blocksRaycasts = false;

        // Fade in the second canvas
        float fadeInTime = 0f;
        while (fadeInTime < fadeDuration)
        {
            fadeInTime += Time.deltaTime;
            fadeInCanvas.alpha = Mathf.Clamp01(fadeInTime / fadeDuration);
            yield return null;
        }
        fadeInCanvas.alpha = 1f;
        fadeInCanvas.interactable = true;
        fadeInCanvas.blocksRaycasts = true;

        isFading = false;
    }

    // Coroutine to move figures on 'Figures' canvas along the Z axis
    IEnumerator MoveFiguresInZ()
    {
        float moveTime = 0f;
        Vector3 startPosition = canvasGroupFigures.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0f, 0f, zMovementDistance);

        // Smoothly move the figures towards the front in the Z axis
        while (moveTime < fadeDuration)
        {
            moveTime += Time.deltaTime;
            canvasGroupFigures.transform.position = Vector3.Lerp(startPosition, endPosition, moveTime / fadeDuration);
            yield return null;
        }

        canvasGroupFigures.transform.position = endPosition;  // Ensure the final position is reached
    }
}
