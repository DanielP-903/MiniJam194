using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 

    [SerializeField, Tooltip("HUD")] public Canvas HUDCanvas;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public TileManager tileManager;
    [SerializeField] public EnemyManager enemyManager;
    
    [SerializeField] private float STANDARD_SCORE_MULTIPLIER = 0.2f;
    [SerializeField] private float TILE_SCORE_TICK_TIME = 1.0f;
    private float tileTickTimer;
    private int score;

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
        
        tileManager.Init();
        enemyManager.Init();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time - tileTickTimer > TILE_SCORE_TICK_TIME)
        {
            tileTickTimer = Time.time;
            float additionalScore = (tileManager.GetNumberOfTilesCaptured() * STANDARD_SCORE_MULTIPLIER);
            int scoreToAdd = Mathf.RoundToInt(additionalScore);
            //print("Added score: +" + scoreToAdd);
            IncreaseScore(scoreToAdd);
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 50,
            alignment = TextAnchor.UpperRight,
        };
        GUI.Label(new Rect(Screen.currentResolution.width - 220, 40,200,40), "SCORE: " + score.ToString("0000000000") , style);
        GUI.Label(new Rect(Screen.currentResolution.width - 220, 100,200,40), "HEALTH: " + playerController.GetCurrentHealth().ToString("#0.0") + "%" , style);
    }

    public void OnPlayerDeath()
    {
        SceneManager.LoadScene("Main Menu Scene", LoadSceneMode.Single);
    }

    public float GetScoreMultiplier()
    {
        return Mathf.Clamp(1.0f + ((float)tileManager.GetNumberOfTilesCaptured() / tileManager.GetNumberOfTiles()), 1.0f, 1.5f);
    }
}
