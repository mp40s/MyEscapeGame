using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Chapter1OPManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string speaker;
        public string sentence;
        public Sprite characterSprite;
    }

    public TextMeshProUGUI dialogueText, speakerText;
    public Button nextButton;
    public Image characterImage;
    public AudioSource audioSource;
    public AudioClip typingSound;
    public AudioSource bgmSource; // BGM用のAudioSource
    public AudioClip firstHalfBGM; // 前半のBGM
    public AudioClip secondHalfBGM; // 後半のBGM
    public AudioClip specialSE; // 特定のセリフの時に再生するSE
    public float delayBeforeSwitching = 0.3f;
    public Sprite hatShowOwnerSprite;
    public Sprite womanSprite1, womanSprite2, womanSprite3, womanSprite4, womanSprite5, womanSprite6;
    public Sprite manSprite1, manSprite2, manSprite3, manSprite4, manSprite5, manSprite6;
    public Image fadePanel; // 暗転・明転に使用するパネル
    public float fadeDuration = 2.0f; // フェードの時間を2秒に設定
    public float delayBeforeFadeOut = 1.0f; // ボタンクリック後の遅延時間
    public string nextSceneName; // 次のシーンの名前

    private List<Dialogue> firstHalfDialogues;
    private List<Dialogue> secondHalfDialogues;
    private int currentDialogueIndex = 0;
    private bool isFirstHalfFinished = false; // 前半が終わったかどうかのフラグ
    private Coroutine typingCoroutine;

    void Start()
    {
        // 前半の会話
        firstHalfDialogues = new List<Dialogue>
        {
            new Dialogue { speaker = "帽子屋", sentence = "いらっしゃい！\nそこの素敵なお二人さん。", characterSprite = hatShowOwnerSprite },
             new Dialogue { speaker = "帽子屋", sentence = "よかったら遊んでいかない？", characterSprite = hatShowOwnerSprite },
            new Dialogue { speaker = "女性", sentence = "楽しそうですね、\n何をやっていらっしゃるんでしょうか？", characterSprite = womanSprite1 },
            new Dialogue { speaker = "帽子屋", sentence = "二人の絆を試す脱出ゲームだよ。", characterSprite = hatShowOwnerSprite },
            new Dialogue { speaker = "男性", sentence = "面白そうだな、\nやってみるか。", characterSprite = manSprite2 },
            new Dialogue { speaker = "帽子屋", sentence = "ありがとう！\nそれじゃあこのアンケートだけ\n答えてくれ。", characterSprite = hatShowOwnerSprite },
            new Dialogue { speaker = "帽子屋", sentence = "そしたら中に案内するよ。", characterSprite = hatShowOwnerSprite }
        };

        // 後半の会話
        secondHalfDialogues = new List<Dialogue>
        {
            new Dialogue { speaker = "女性", sentence = "(うう、、、頭が痛い、、、)", characterSprite = womanSprite6 },
            new Dialogue { speaker = "女性", sentence = "私たち、、、\n監禁されてる！？！？", characterSprite = womanSprite5 },
            new Dialogue { speaker = "男性", sentence = "どうなっているんだ?", characterSprite = manSprite6 },
            new Dialogue { speaker = "男性", sentence = "まさかこの状態から脱出しろと\n言うのか？", characterSprite = manSprite6 },
            new Dialogue { speaker = "女性", sentence = "突然気を失わせるとか首輪をつけるとか\n明らかにやりすぎじゃないですか？", characterSprite = womanSprite6 },
            new Dialogue { speaker = "女性", sentence = "犯罪でしょう、、、", characterSprite = womanSprite6 },
            new Dialogue { speaker = "男性", sentence = "本当に犯罪組織だったら危ない。", characterSprite = manSprite4 },
            new Dialogue { speaker = "男性", sentence = "とりあえず脱出することだけ\n今は考えよう", characterSprite = manSprite4 },
            new Dialogue { speaker = "女性", sentence = "そうですね、、、", characterSprite = womanSprite6 }
        };

        // 前半のBGMを再生
        bgmSource.clip = firstHalfBGM;
        bgmSource.Play();

        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        // 前半が終わった後に暗転・明転処理を行う
        if (currentDialogueIndex >= firstHalfDialogues.Count && !isFirstHalfFinished)
        {
            StartCoroutine(FadeOutThenContinueToSecondHalf());
            return;
        }

        // 後半の会話が終了したらシーンを切り替える
        if (isFirstHalfFinished && currentDialogueIndex >= secondHalfDialogues.Count)
        {
            StartCoroutine(EndDialogueAndSwitchScene());
            return;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        Dialogue currentDialogue;

        // 前半か後半かを判定して、表示する会話を選択
        if (!isFirstHalfFinished)
        {
            currentDialogue = firstHalfDialogues[currentDialogueIndex];
        }
        else
        {
            currentDialogue = secondHalfDialogues[currentDialogueIndex];
        }

        typingCoroutine = StartCoroutine(SwitchDialogueWithDelay(currentDialogue));
    }

    IEnumerator SwitchDialogueWithDelay(Dialogue dialogue)
    {
        yield return new WaitForSeconds(delayBeforeSwitching);

        characterImage.sprite = dialogue.characterSprite;
        speakerText.text = dialogue.speaker;
        dialogueText.text = "";

        characterImage.gameObject.SetActive(true);

        audioSource.clip = typingSound;
        audioSource.loop = true;
        audioSource.Play();

        // 特定のセリフのときにUIを揺らす
        if (dialogue.sentence.Contains("監禁されてる"))
        {
            // SEを再生する
            audioSource.PlayOneShot(specialSE);
            StartCoroutine(ShakeUI());
        }

        yield return TypeSentence(dialogue.sentence);
    }

    IEnumerator TypeSentence(string sentence)
    {
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        audioSource.Stop();
        nextButton.interactable = true;
    }

    IEnumerator EndDialogueAndSwitchScene()
    {
        // セリフとキャラクター画像を消す
        ClearDialogue();

        // フェードアウト
        nextButton.gameObject.SetActive(false);
        yield return StartCoroutine(FadeOut());

        // 次のシーンに切り替え
        SceneManager.LoadScene("Chapter1GameScene");
    }

    void ClearDialogue()
    {
        dialogueText.text = "";
        speakerText.text = "";
        characterImage.gameObject.SetActive(false);
    }

    public void OnNextButtonClick()
    {
        nextButton.interactable = false;
        currentDialogueIndex++;
        DisplayDialogue();
    }

    // 暗転後に後半の会話を開始する処理
    IEnumerator FadeOutThenContinueToSecondHalf()
    {
        // フェードアウト処理
        yield return StartCoroutine(FadeOut());

        // 前半のセリフとキャラクター画像をクリアする
        ClearDialogue();

        // BGMを切り替え
        bgmSource.Stop(); // 前半のBGMを停止

        // インデックスをリセットし、後半の会話を開始
        currentDialogueIndex = 0;
        isFirstHalfFinished = true;

        // 明転処理
        yield return StartCoroutine(FadeIn());

        // 後半のBGMを再生
        bgmSource.clip = secondHalfBGM;
        bgmSource.Play();

        // 後半の会話を表示
        DisplayDialogue();
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadePanel.color;
        while (elapsedTime < fadeDuration)
        {
            fadeColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadePanel.color = fadeColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeColor.a = 1f;
        fadePanel.color = fadeColor;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadePanel.color;
        while (elapsedTime < fadeDuration)
        {
            fadeColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadePanel.color = fadeColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeColor.a = 0f;
        fadePanel.color = fadeColor;
    }

    IEnumerator ShakeUI()
    {
        Vector3 originalPositionDialogueText = dialogueText.transform.position;
        Vector3 originalPositionSpeakerText = speakerText.transform.position;
        Vector3 originalPositionCharacterImage = characterImage.transform.position;

        float shakeDuration = 0.2f; // 揺れる時間
        float shakeAmount = 0.3f; // 揺れる範囲（小さめ）
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-shakeAmount, shakeAmount);

            dialogueText.transform.position = originalPositionDialogueText + new Vector3(offsetX, 0, 0);
            speakerText.transform.position = originalPositionSpeakerText + new Vector3(offsetX, 0, 0);
            characterImage.transform.position = originalPositionCharacterImage + new Vector3(offsetX, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 元の位置に戻す
        dialogueText.transform.position = originalPositionDialogueText;
        speakerText.transform.position = originalPositionSpeakerText;
        characterImage.transform.position = originalPositionCharacterImage;
    }
}
