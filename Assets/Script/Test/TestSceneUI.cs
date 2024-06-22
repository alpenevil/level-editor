using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSceneUI : MonoBehaviour
{
    public GameObject exitConfirmationPanel;
    public Button exitYesButton;
    public Button exitCancelButton;

    private FirstPersonController playerController;

    void Start()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
        }

        playerController = FindObjectOfType<FirstPersonController>();

        if (exitYesButton != null)
            exitYesButton.onClick.AddListener(() => ExitToMainMenu());

        if (exitCancelButton != null)
            exitCancelButton.onClick.AddListener(() => CloseExitConfirmationPanel());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowExitConfirmationPanel();
        }
    }

    void ShowExitConfirmationPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
            if (playerController != null)
            {
                playerController.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void CloseExitConfirmationPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
            if (playerController != null)
            {
                playerController.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
