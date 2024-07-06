using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.U2D.IK;

public class PlayerCollision : MonoBehaviour
{
    #region Singleton

    public static PlayerCollision Instance;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    #endregion   
    
    GameManager gm;
    private int score = 0;
    private int highScore;
    public event System.Action OnFirePickup;
    public event System.Action OnArrowPickup;
    bool isOnFire;
    [SerializeField] ParticleSystem playerParticleSystem;
    [SerializeField] private GameObject FireBackground;
    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource multipleCoinsSound;
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource fireMusic;
    [SerializeField] private AudioSource bassSound;
    [SerializeField] private AudioSource pickup_speed;
    [SerializeField] private AudioSource pickup_fire;
    private void Start() {
        highScore = PlayerPrefs.GetInt("Highscore");
        gm = GameManager.Instance;
        gm.OnGameStart += ActivatePlayer;
    }

    private void ActivatePlayer() {
        gameObject.SetActive(true);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.tag == "Obstacle") {
            if (isOnFire) {
                multipleCoinsSound.Play();
                other.gameObject.SetActive(false);
                score = score + 3;
                StartCoroutine(CoinParticles());  
            }
            else {
                Highscore();
                GameManager.Instance.GameOver();
                gameObject.SetActive(false);
                score = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.transform.tag == "Coin") {
            coinSound.Play();
            other.gameObject.SetActive(false);
            score++;
            if (score > highScore)
            {
                PlayerPrefs.SetInt("Highscore", score);
                highScore = score;
            }
            Score();
        }

        if(other.transform.tag == "Arrow")
        {
            pickup_speed.Play();
            other.gameObject.SetActive(false);
            OnFirePickup?.Invoke();
        }
        if(other.transform.tag == "Fire")
        {
            pickup_fire.Play();
            other.gameObject.SetActive(false);
            OnFirePickup?.Invoke();
            StartCoroutine(FireMode());
        }
    }
    
    public string Score() {
        return score.ToString();
    }
    public string Highscore() {
        return highScore.ToString();
    }

    private IEnumerator CoinParticles() {
        playerParticleSystem.Play();
        yield return new WaitForSeconds(1);
        playerParticleSystem.Stop();
    }
    private IEnumerator FireMode() {

        isOnFire = true;
        OnArrowPickup?.Invoke();
        FireBackground.SetActive(true);
        // Start Fire Music
        fireMusic.volume = 1f;
        fireMusic.Play(); 
        // Fade out the main music
        float startTime = Time.time;
        float t; // Declare 't' here
        float fadeTime = 1f; // Adjust this value to control the speed of the fade
        while (Time.time < startTime + fadeTime)
        {
            t = (Time.time - startTime) / fadeTime;
            mainMusic.volume = Mathf.Lerp(mainMusic.volume, 0f, t);
            yield return null;
        }
        mainMusic.volume = 0f; // Ensure the volume reaches 0
        yield return new WaitForSeconds(5.5f);
        bassSound.Play();
        yield return new WaitForSeconds(1.5f);
        
        // Reverse the fire mode state
        isOnFire = false;
        FireBackground.SetActive(false);
        
        // Fade in the main music
        startTime = Time.time;
        fadeTime = 1f; // Adjust this value to control the speed of the fade
        while (Time.time < startTime + fadeTime)
        {
            t = (Time.time - startTime) / fadeTime;
            mainMusic.volume = Mathf.Lerp(mainMusic.volume, 1f, t);
            yield return null;
        }
        mainMusic.volume = 1f; // Ensure the volume reaches 1
        
        // Fade out the fire music
        startTime = Time.time;
        fadeTime = 1f; // Adjust this value to control the speed of the fade
        while (Time.time < startTime + fadeTime)
        {
            t = (Time.time - startTime) / fadeTime;
            fireMusic.volume = Mathf.Lerp(fireMusic.volume, 0f, t);
            yield return null;
        }
        fireMusic.volume = 0f; // Ensure the volume reaches 0
        fireMusic.Stop();
        
    }

    public bool IsOnFire() {
        return isOnFire;
    }
}
