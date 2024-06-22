using UnityEngine;
using UnityEngine.UI;

public class GridVisualizationUI : MonoBehaviour
{
    public GameObject gridVisualization; 
    public Button toggleGridButton;
    public Sprite gridOnSprite; 
    public Sprite gridOffSprite; 

    private const string GridVisualizationKey = "GridVisualizationState";

    void Start()
    {
        bool isGridVisible = PlayerPrefs.GetInt(GridVisualizationKey, 1) == 1;
        SetGridVisualization(isGridVisible);

        toggleGridButton.onClick.AddListener(ToggleGridVisualization);
    }

    void ToggleGridVisualization()
    {
        bool isCurrentlyVisible = gridVisualization.activeSelf;
        SetGridVisualization(!isCurrentlyVisible);
    }

    void SetGridVisualization(bool isVisible)
    {
        gridVisualization.SetActive(isVisible);

        PlayerPrefs.SetInt(GridVisualizationKey, isVisible ? 1 : 0);
        PlayerPrefs.Save();

        toggleGridButton.image.sprite = isVisible ? gridOnSprite : gridOffSprite;
    }
}
