using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    private Canvas canvas;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI filledPercentageText;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private float SCORE_BUMP_DURATION = 0.1f;
    [SerializeField] private float FILL_BUMP_DURATION = 0.1f;

    private float previousFillPercent = 0.0f;
    
    private float scoreBumpTime = float.MinValue;
    private float defaultScoreY;
    
    //private float healthBumpTime = float.MinValue;
    private float fillBumpTime = float.MinValue;
    private bool canBumpFillPercent;

    private bool fillBumpIsPositive = true;
    
    //private UIDocument HUDDocument;
    //Label scoreText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        defaultScoreY = scoreText.rectTransform.localPosition.y;
        canBumpFillPercent = false;
        //HUDDocument = GetComponent<UIDocument>();
        //if (!HUDDocument)
        //{
        //    throw new Exception();
        //}
        //
        //var rootElement = HUDDocument.rootVisualElement;
        //scoreText = rootElement.Q<Label>("Score");
    }

    private void Update()
    {        
        float alpha = (Time.time - scoreBumpTime) / SCORE_BUMP_DURATION;
        alpha = Mathf.Clamp01(alpha);
        float sin = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, alpha)) * 5.0f;
        scoreText.rectTransform.localPosition = new Vector3(scoreText.rectTransform.localPosition.x, defaultScoreY + sin, 0);
               
        alpha = (Time.time - fillBumpTime) / FILL_BUMP_DURATION;
        alpha = Mathf.Clamp01(alpha);
        sin = Mathf.Sin(Mathf.Lerp(0, Mathf.PI, alpha)) * 5.0f;
        float multiplier = fillBumpIsPositive ? 1 : -1;
        filledPercentageText.rectTransform.localPosition = new Vector3(filledPercentageText.rectTransform.localPosition.x, defaultScoreY +
            (sin * multiplier), 0);

        filledPercentageText.color = Color.Lerp(Color.white, (fillBumpIsPositive ? Color.green : Color.red), sin);
        
        if (alpha >= 1)
        {
            canBumpFillPercent = true;
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score " + score.ToString("00000000");
        scoreBumpTime = Time.time;
    }

    public void UpdateFillPercentage(float fillPercentage)
    {
        if (fillPercentage.ToString("0.0") == previousFillPercent.ToString("0.0"))
        {
            // Same thing
            return;
        }
        
        filledPercentageText.text = "Filled " + fillPercentage.ToString("0.0") + "%";
        if (canBumpFillPercent)
        {
            fillBumpTime = Time.time;
            canBumpFillPercent = false;
            fillBumpIsPositive = fillPercentage >= previousFillPercent;
        }
        previousFillPercent = fillPercentage;
    }

    public void UpdateGoal(float tileFillGoalPercent)
    {
        goalText.text = "Goal " + tileFillGoalPercent.ToString("0.0") + "%";
    }
}
