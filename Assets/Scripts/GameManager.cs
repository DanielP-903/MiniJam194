using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("HUD")] private Canvas HUDCanvas;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private float STANDARD_SCORE_MULTIPLIER = 0.2f;
    [SerializeField] private float TILE_SCORE_TICK_TIME = 1.0f;
    private float tileTickTimer;
    private int score;
    
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
        GUI.Label(new Rect(Screen.currentResolution.width - 220, 100,200,40), "HEALTH: " + playerController.GetCurrentHealth() + "%" , style);
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
