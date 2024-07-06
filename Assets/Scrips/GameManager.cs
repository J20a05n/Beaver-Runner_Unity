using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    #endregion   
    
    public bool isPlaying = false;
    public event System.Action OnGameStart;
    public event System.Action OnGameOver;
    private float timeAlive = 1f;
    private bool coolDown = false;

    private void Update() {

        if(Input.anyKeyDown && isPlaying == false && coolDown == false) {
            isPlaying = true;
            OnGameStart?.Invoke();
        }
        if (isPlaying) timeAlive += Time.deltaTime;
    }
    
    public void GameOver() {
        coolDown = true;
        timeAlive = 1;
        isPlaying = false;
        OnGameOver?.Invoke();
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown() {
        yield return new WaitForSeconds(3);
        coolDown = false;
    }

    public float TimeAlive() {
        return timeAlive;
    }
}