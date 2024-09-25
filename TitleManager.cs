using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public CanvasGroup imageBackCanvasGroup;
    public CanvasGroup imageTitleCanvasGroup;
    public CanvasGroup buttonStartCanvasGroup;
    public CanvasGroup buttonSelectCanvasGroup;
    public CanvasGroup buttonCreditCanvasGroup;

    public float fadeDuration = 1f;
    public AudioSource sePlayer;

    void Start()
    {
        InitializeCanvasGroups();

        StartCoroutine(FadeInSequence());
    }

    void InitializeCanvasGroups()
    {
        foreach (var canvasGroup in new[] { imageBackCanvasGroup, imageTitleCanvasGroup, buttonStartCanvasGroup, buttonSelectCanvasGroup, buttonCreditCanvasGroup })
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    IEnumerator FadeInSequence()
    {
        yield return StartCoroutine(FadeIn(imageBackCanvasGroup));
        yield return StartCoroutine(FadeIn(imageTitleCanvasGroup));
        yield return StartCoroutine(FadeIn(buttonStartCanvasGroup));
        yield return StartCoroutine(FadeIn(buttonSelectCanvasGroup));
        yield return StartCoroutine(FadeIn(buttonCreditCanvasGroup));
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void PushStartButton()
    {
        sePlayer.Play();
        StartCoroutine(LoadSceneAfterDelay(0.5f));
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("OpeningScene");
    }

    void Awake() => DontDestroyOnLoad(gameObject);
}
