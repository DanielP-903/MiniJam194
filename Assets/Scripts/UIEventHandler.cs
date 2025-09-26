using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIEventHandler : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;
    private Button playButton;
    private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var rootElement = m_UIDocument.rootVisualElement;
        playButton = rootElement.Q<Button>("PlayButton");
        playButton.clickable.clicked += OnPlayButtonClicked;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        playButton.Focus();
        
        quitButton = rootElement.Q<Button>("QuitButton");
        quitButton.clickable.clicked += OnQuitButtonClicked;
    }
    
    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
