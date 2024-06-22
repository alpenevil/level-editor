using UnityEngine;
using UnityEngine.UI;

public class RandomItemUI : MonoBehaviour
{
    public Button itemButton;
    public Image iconImage;
    public Image background;

    private RandomPanelUI panelUI;
    private int itemId;
    private bool isRandomSelectionEnabled;

    public void Initialize(RandomPanelUI panelUI, int itemId, string itemName, Sprite icon, bool isRandomSelectionEnabled, Sprite normalBackground, Sprite selectedBackground)
    {
        this.panelUI = panelUI;
        this.itemId = itemId;
        this.isRandomSelectionEnabled = isRandomSelectionEnabled;

        iconImage.sprite = icon;
        itemButton.onClick.AddListener(() => OnButtonClick());
        UpdateBackground();
    }

    private void OnButtonClick()
    {
        isRandomSelectionEnabled = !isRandomSelectionEnabled;
        panelUI.OnItemButtonClicked(itemId, isRandomSelectionEnabled);
        UpdateBackground();
    }

    private void UpdateBackground()
    {
        background.sprite = isRandomSelectionEnabled ? panelUI.selectedBackgroundSprite : panelUI.normalBackgroundSprite;
    }
}
