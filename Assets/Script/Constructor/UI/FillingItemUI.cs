using UnityEngine;
using UnityEngine.UI;

public class FillingItemUI : MonoBehaviour
{
    public Button itemButton;
    public Image iconImage;
    public Image background;

    private FillingPanelUI panelUI;
    private int itemId;

    public int ItemId => itemId;

    public void Initialize(FillingPanelUI panelUI, int itemId, string itemName, Sprite icon, Sprite normalBackground, Sprite selectedBackground)
    {
        this.panelUI = panelUI;
        this.itemId = itemId;

        if (iconImage == null || itemButton == null || background == null)
        {
            Debug.LogError("One or more UI components are not assigned in the inspector.");
            return;
        }

        iconImage.sprite = icon;
        itemButton.onClick.AddListener(() => OnButtonClick());
        background.sprite = normalBackground;
    }

    private void OnButtonClick()
    {
       // Debug.Log("Button clicked for item ID: " + itemId);

        panelUI.OnItemButtonClicked(itemId);
    }

    public void UpdateBackground(bool isSelected)
    {
        background.sprite = isSelected ? panelUI.selectedBackgroundSprite : panelUI.normalBackgroundSprite;
    }
}
