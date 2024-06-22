using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelManagerUI : MonoBehaviour
{
    public TMP_InputField saveFileNameInputField;
    public TMP_Dropdown loadLevelDropdown;
    public GameObject confirmLoadLevelPanel;
    public GameObject saveLevelPanel;
    public Button confirmLoadLevelButton;
    public Button cancelLoadLevelButton;
    public Button confirmSaveLevelButton;
    public Button cancelSaveLevelButton;
    public Button showLoadLevelPanelButton;
    public CanvasGroup mainCanvasGroup;

    public Camera playerCamera;
    private CameraController cameraController;
    public InputManager inputManager;

    public GameObject exitConfirmationPanel;
    public Button confirmExitButton;
    public Button cancelExitButton;
    public Button showExitConfirmationPanelButton;

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        if (playerCamera != null)
        {
            cameraController = playerCamera.GetComponent<CameraController>();

            if (cameraController == null)
                Debug.LogError("CameraController component not found on playerCamera.");
        }
        else
        {
            Debug.LogError("Player camera not assigned.");
        }

        inputManager = FindObjectOfType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogError("InputManager not found in the scene.");
        }

        if (confirmLoadLevelPanel != null)
        {
            confirmLoadLevelPanel.SetActive(false);
        }

        if (saveLevelPanel != null)
        {
            saveLevelPanel.SetActive(false);
        }

        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
        }

        if (PlayerPrefs.HasKey("levelToLoad"))
        {
            string levelName = PlayerPrefs.GetString("levelToLoad");
            PlayerPrefs.DeleteKey("levelToLoad");
            levelManager.LoadLevelFromFile(levelName);
        }
        PopulateLoadDropdown();
        AddButtonListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOpenPanels();
        }
    }

    private void AddButtonListeners()
    {
        if (confirmLoadLevelButton != null)
            confirmLoadLevelButton.onClick.AddListener(() => ConfirmLoadLevel());

        if (cancelLoadLevelButton != null)
            cancelLoadLevelButton.onClick.AddListener(() => CloseOpenPanels());

        if (confirmSaveLevelButton != null)
            confirmSaveLevelButton.onClick.AddListener(() => ConfirmSaveLevel());

        if (cancelSaveLevelButton != null)
            cancelSaveLevelButton.onClick.AddListener(() => CloseOpenPanels());

        if (showLoadLevelPanelButton != null)
            showLoadLevelPanelButton.onClick.AddListener(() => ShowLoadLevelPanel());

        if (showExitConfirmationPanelButton != null)
            showExitConfirmationPanelButton.onClick.AddListener(() => ShowExitConfirmationPanel());

        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(() => ExitToMainMenu());

        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(() => CloseExitConfirmationPanel());
    }

    private void CloseOpenPanels()
    {
        if (confirmLoadLevelPanel != null && confirmLoadLevelPanel.activeSelf)
        {
            confirmLoadLevelPanel.SetActive(false);
            SetMainCanvasGroupInteractable(true);
            EnableControls();
        }
        if (saveLevelPanel != null && saveLevelPanel.activeSelf)
        {
            saveLevelPanel.SetActive(false);
            SetMainCanvasGroupInteractable(true);
            EnableControls();
        }
        if (exitConfirmationPanel != null && exitConfirmationPanel.activeSelf)
        {
            exitConfirmationPanel.SetActive(false);
            SetMainCanvasGroupInteractable(true);
            EnableControls();
        }
    }

    private void SetMainCanvasGroupInteractable(bool isInteractable)
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.interactable = isInteractable;
            mainCanvasGroup.blocksRaycasts = isInteractable;
        }
    }

    public void ShowSaveLevelPanel()
    {
        if (saveLevelPanel != null)
        {
            saveLevelPanel.SetActive(true);
            SetMainCanvasGroupInteractable(false);
            levelManager.placementSystem.StopPlacement();
            DisableControls();
        }
    }

    private void ConfirmSaveLevel()
    {
        string fileName = saveFileNameInputField != null ? saveFileNameInputField.text : string.Empty;
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("File name is empty.");
            return;
        }

        levelManager.SaveLevel(fileName);

        if (saveLevelPanel != null)
            saveLevelPanel.SetActive(false);
        PopulateLoadDropdown();
        SetMainCanvasGroupInteractable(true);
        EnableControls();
    }

    public void ShowLoadLevelPanel()
    {
        PopulateLoadDropdown();
        if (confirmLoadLevelPanel != null)
        {
            confirmLoadLevelPanel.SetActive(true);
            SetMainCanvasGroupInteractable(false);
            levelManager.placementSystem.StopPlacement();
            DisableControls();
        }
    }

    private void ConfirmLoadLevel()
    {
        if (loadLevelDropdown == null)
        {
            Debug.LogError("Load dropdown is not set.");
            return;
        }

        string levelName = loadLevelDropdown.options[loadLevelDropdown.value].text;
        PlayerPrefs.SetString("levelToLoad", levelName);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowExitConfirmationPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(true);
            SetMainCanvasGroupInteractable(false);
            levelManager.placementSystem.StopPlacement();
            DisableControls();
        }
    }

    public void CloseExitConfirmationPanel()
    {
        if (exitConfirmationPanel != null)
        {
            exitConfirmationPanel.SetActive(false);
            SetMainCanvasGroupInteractable(true);
            EnableControls();
        }
    }

    private void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void DisableControls()
    {
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        if (inputManager != null)
        {
            inputManager.enabled = false;
        }
    }

    private void EnableControls()
    {
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
        if (inputManager != null)
        {
            inputManager.enabled = true;
        }
    }

    public void PopulateLoadDropdown()
    {
        string savedLevelsDirectory = "Assets/SavedLevels";

        if (!Directory.Exists(savedLevelsDirectory))
        {
            Directory.CreateDirectory(savedLevelsDirectory);
        }

        string[] files = Directory.GetFiles(savedLevelsDirectory, "*.json");
        List<string> levelNames = new List<string>();

        foreach (string filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            levelNames.Add(fileName);
        }

        if (loadLevelDropdown != null)
        {
            loadLevelDropdown.ClearOptions();
            loadLevelDropdown.AddOptions(levelNames);
        }
    }
}
