using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public LevelSelectionPanel levelSelectionPanel;

    public void LoadConstructor()
    {
        SceneManager.LoadScene("GridPlacementSystem"); 
    }

    public void LoadTestScene()
    {
        levelSelectionPanel.ShowPanel();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}

