using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OpeningManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public AudioSource audioSource;
    public AudioClip typingSound;
    public float delayBeforeDisplaying = 0.5f;
    public string nextSceneName;

    private List<Dialogue> dialogues;
    private int currentDialogueIndex = 0;
    private Coroutine typingCoroutine;

    [System.Serializable]
    public class Dialogue
    {
        public string sentence;
    }

    void Start()
    {
        dialogues = new List<Dialogue>
        {
            new Dialogue{sentence="さあさあ、\n寄ってらっしゃい\n見てらっしゃい"},
            new Dialogue{sentence="ここは、\n『2人の絆を試す脱出ゲーム』"},
            new Dialogue{sentence="2人の絆があれば\n脱出できるコトでしょう！！"},
            new Dialogue{sentence="それでは、\n楽しんでいって下さい。"}
        };

        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            StartCoroutine(DisplayDialogueWithDelay(dialogues[currentDialogueIndex].sentence));
        }
        else
        {
            ClearDialogue();
            nextButton.gameObject.SetActive(false);
            StartCoroutine(LoadNextSceneWithDelay(1f));
        }
    }

    IEnumerator DisplayDialogueWithDelay(string sentence)
    {
        yield return new WaitForSeconds(delayBeforeDisplaying);

        dialogueText.text = "";
        audioSource.clip = typingSound;
        audioSource.loop = true;
        audioSource.Play();

        typingCoroutine = StartCoroutine(typeSentence(sentence));
    }

    IEnumerator typeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        audioSource.Stop();
        nextButton.interactable = true;
    }

    void ClearDialogue()
    {
        dialogueText.text = "";
    }

    public void OnNextButtonClick()
    {
        nextButton.interactable = false;
        currentDialogueIndex++;
        DisplayDialogue();
    }

    IEnumerator LoadNextSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Chapter1OPScene");
    }
}
