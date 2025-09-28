using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndLevelEventHandler : MonoBehaviour
{
    private UIDocument document;
    private Button continueButton;
    private Label scoreLabel;
    private Label titleLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!GameManager.Instance)
        {
            return;
        }
        
        document = GetComponent<UIDocument>();
        var rootElement = document.rootVisualElement;
        continueButton = rootElement.Q<Button>("ContinueButton");
        continueButton.clickable.clicked += OnContinueButtonClicked;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        continueButton.Focus();
        
        scoreLabel = rootElement.Q<Label>("ScoreLabel");
        titleLabel = rootElement.Q<Label>("TitleLabel");

        scoreLabel.text = "Score: " + GameManager.Instance.score.ToString("00000000");
        titleLabel.text = GameManager.Instance.hasWon ? "You win!" : "You lose!";
    }
    
    private void OnContinueButtonClicked()
    {
        SceneManager.LoadScene("Main Menu Scene", LoadSceneMode.Single);
    }
}
