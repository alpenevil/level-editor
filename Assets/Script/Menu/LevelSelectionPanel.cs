using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectionPanel : MonoBehaviour
{
    public GameObject levelButtonPrefab; 
    public Transform contentPanel; 
    public string savedLevelsDirectory = "Assets/SavedLevels"; 
    public GameObject panel; 

    void Start()
    {
        PopulateLevelButtons();
        panel.SetActive(false); 
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    void PopulateLevelButtons()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        if (!Directory.Exists(savedLevelsDirectory))
        {
            Directory.CreateDirectory(savedLevelsDirectory);
        }

        string[] files = Directory.GetFiles(savedLevelsDirectory, "*.json");
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject buttonObject = Instantiate(levelButtonPrefab, contentPanel);
            buttonObject.GetComponentInChildren<TMP_Text>().text = fileName;

            // Load and display screenshot
            string screenshotPath = Path.Combine(savedLevelsDirectory, fileName + ".png");
            if (File.Exists(screenshotPath))
            {
                byte[] fileData = File.ReadAllBytes(screenshotPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);
                buttonObject.GetComponentInChildren<RawImage>().texture = texture;
            }

            buttonObject.GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(fileName));
        }
    }

    void OnLevelButtonClicked(string levelName)
    {
        PlayerPrefs.SetString("levelToLoad", levelName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("TestScene"); 
    }
}
