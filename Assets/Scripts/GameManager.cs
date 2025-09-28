using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 

    [SerializeField] public HUD playerHUD;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public TileManager tileManager;
    [SerializeField] public EnemyManager enemyManager;
    [SerializeField] public ParticleManager particleManager;
    
    private AudioSource audioSource;
    [SerializeField] private AudioClip levelMusic;
    
    [SerializeField] private float STANDARD_SCORE_MULTIPLIER = 0.2f;
    [SerializeField] private float TILE_SCORE_TICK_TIME = 1.0f;
    [SerializeField] private float TILE_FILL_GOAL_PERCENT = 85.0f;
    private float tileTickTimer;
    public int score { get; private set; }

    public bool isGameOver { get; private set; }
    public bool hasWon { get; private set; }

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = levelMusic;
        audioSource.Play();
        
        tileManager.Init();
        enemyManager.Init();
        particleManager.Init();
        
        playerHUD.UpdateScore(0);
        playerHUD.UpdateFillPercentage(0);
        playerHUD.UpdateGoal(TILE_FILL_GOAL_PERCENT);

        isGameOver = false;
        
        Time.timeScale = 1;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        
        if (Time.time - tileTickTimer > TILE_SCORE_TICK_TIME)
        {
            tileTickTimer = Time.time;
            float additionalScore = (tileManager.GetNumberOfTilesCaptured() * STANDARD_SCORE_MULTIPLIER);
            int scoreToAdd = Mathf.RoundToInt(additionalScore);
            //print("Added score: +" + scoreToAdd);
            IncreaseScore(scoreToAdd);
        }

        if ((float)tileManager.GetNumberOfTilesCaptured() / tileManager.GetNumberOfTiles() > (TILE_FILL_GOAL_PERCENT / 100.0f))
        {
            EndGame();
            hasWon = true;
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        
        playerHUD.UpdateScore(score);
    }

    //void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle
    //    {
    //        fontSize = 50,
    //        alignment = TextAnchor.UpperRight,
    //    };
    //    //GUI.Label(new Rect(Screen.currentResolution.width - 220, 40,200,40), "SCORE: " + score.ToString("0000000000") , style);
    //    //GUI.Label(new Rect(Screen.currentResolution.width - 220, 100,200,40), "HEALTH: " + playerController.GetCurrentHealth().ToString("#0.0") + "%" , style);
    //}

    public void OnPlayerDeath()
    {
        if (isGameOver)
        {
            return;
        }

        EndGame();
        hasWon = false;
    }

    private void EndGame()
    {        
        // Finished game
        isGameOver = true;
        
        audioSource.Stop();
        
        playerHUD.gameObject.SetActive(false);
        
        particleManager.DestroyAllParticles();
        
        // Show end screen hud elements
        SceneManager.LoadScene("End Level", LoadSceneMode.Additive);
    
    }
    
    public float GetScoreMultiplier()
    {
        return Mathf.Clamp(1.0f + ((float)tileManager.GetNumberOfTilesCaptured() / tileManager.GetNumberOfTiles()), 1.0f, 1.5f);
    }

    public float GetFillGoal()
    {
        return TILE_FILL_GOAL_PERCENT;
    }
}
