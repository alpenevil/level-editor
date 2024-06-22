using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image backgroundImage;
    public Image iconImage;
    public Text nameText;
    public Button button;

    public Sprite normalBackgroundSprite;
    public Sprite selectedBackgroundSprite;

    private int itemId;  
    public int ItemId => itemId;  

    private ItemPanelUI itemPanelUI;

    public void Initialize(ItemPanelUI panelUI, int id, string name, Sprite icon, Sprite normalSprite, Sprite selectedSprite)
    {
        itemPanelUI = panelUI;
        itemId = id;
        nameText.text = name;
        iconImage.sprite = icon;
        normalBackgroundSprite = normalSprite;
        selectedBackgroundSprite = selectedSprite;
        button.onClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked()
    {
        itemPanelUI.OnItemButtonClicked(itemId);
        itemPanelUI.OnItemSelected(this);  
    }

    public void SetSelected(bool isSelected)
    {
        backgroundImage.sprite = isSelected ? selectedBackgroundSprite : normalBackgroundSprite;
    }
}
