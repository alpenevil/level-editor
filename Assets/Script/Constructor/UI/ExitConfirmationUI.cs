using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitConfirmationUI : MonoBehaviour
{
    public GameObject exitConfirmationPanel;
    public Button exitButton; 
    public Button yesButton; 
    public Button cancelButton; 

    void Start()
    {
        exitConfirmationPanel.SetActive(false);

        exitButton.onClick.AddListener(ShowExitConfirmation);
        yesButton.onClick.AddListener(ExitToMainMenu);
        cancelButton.onClick.AddListener(CloseExitConfirmation);
    }

    void ShowExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    void CloseExitConfirmation()
    {
        exitConfirmationPanel.SetActive(false);
    }

    void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
