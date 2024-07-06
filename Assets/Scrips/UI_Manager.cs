using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI activeScoreUI;
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI highScoreUI;
    [SerializeField] private GameObject startGameUI;
    [SerializeField] private Image backgroundImageStart;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Image backgroundImageGameOver;
    [SerializeField] private TMP_Text versionText;
    [SerializeField] private AudioSource player_dies;

    
    GameManager gm;
    PlayerCollision pc;
    private void Start() {
        versionText.text = Application.version;
        gm = GameManager.Instance;
        pc = PlayerCollision.Instance;
        gm.OnGameStart += StartGame;
        gm.OnGameOver += EndGame;
    }

    private void StartGame() {
        gameOverUI.SetActive(false);
        StartCoroutine(FadeOutImage(backgroundImageStart));
        StartCoroutine(FadeOutImage(backgroundImageGameOver));
    }

    private IEnumerator FadeOutImage(Image backgroundImage) {
        float duration = 1.0f; // Duration of the fade-out in seconds
        float elapsedTime = 0f;
        Color startColor = backgroundImage.color;
        startGameUI.SetActive(false); // Disable the UI element after fade-out

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.5f, 0, elapsedTime / duration);
            startColor.a = alpha;
            backgroundImage.color = startColor;
            yield return null;
        }

        startColor.a = 0.5f;
        backgroundImage.color = startColor;
        backgroundImage.gameObject.SetActive(false);
        activeScoreUI.gameObject.SetActive(true);
    }

    private void EndGame() {
        player_dies.Play();
        scoreUI.text = pc.Score();
        highScoreUI.text = pc.Highscore();
        backgroundImageGameOver.gameObject.SetActive(true);
        StartCoroutine(FadeInImage());
    }

    private IEnumerator FadeInImage() {
        float duration = 1.0f;
        float elapsedTime = 0f;
        Color startColor = backgroundImageGameOver.color;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 0.5f, elapsedTime / duration);
            startColor.a = alpha;
            backgroundImageGameOver.color = startColor;
            yield return null;
        }

        startColor.a = 0.5f;
        backgroundImageGameOver.color = startColor;
        gameOverUI.SetActive(true);
        activeScoreUI.gameObject.SetActive(false);
    }
    private void OnGUI()
    {
        activeScoreUI.text = pc.Score();
    }

    private void OnDestroy() {
        gm.OnGameStart -= StartGame;
    }
}
