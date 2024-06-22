using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public Button installButton;
    public Button fillButton;
    public Button randomInstallButton;

    public GameObject installPanel;
    public GameObject fillPanel;
    public GameObject randomInstallPanel;

    public Sprite installButtonNormalSprite;
    public Sprite installButtonSelectedSprite;
    public Sprite fillButtonNormalSprite;
    public Sprite fillButtonSelectedSprite;
    public Sprite randomInstallButtonNormalSprite;
    public Sprite randomInstallButtonSelectedSprite;

    public ItemPanelUI itemPanelUI;

    private GameObject activePanel;
    private Button activeButton;
    private RectTransform installPanelRect;
    private RectTransform fillPanelRect;
    private RectTransform randomInstallPanelRect;
    private Vector3 originalInstallButtonLocalPosition;
    private Vector3 originalFillButtonLocalPosition;
    private Vector3 originalRandomInstallButtonLocalPosition;

    void Start()
    {
        installButton.onClick.AddListener(() => SetMode(installButton, installPanel, installButtonNormalSprite, installButtonSelectedSprite));
        fillButton.onClick.AddListener(() => SetMode(fillButton, fillPanel, fillButtonNormalSprite, fillButtonSelectedSprite));
        randomInstallButton.onClick.AddListener(() => SetMode(randomInstallButton, randomInstallPanel, randomInstallButtonNormalSprite, randomInstallButtonSelectedSprite));

        installPanel.SetActive(false);
        fillPanel.SetActive(false);
        randomInstallPanel.SetActive(false);

        installPanelRect = installPanel.GetComponent<RectTransform>();
        fillPanelRect = fillPanel.GetComponent<RectTransform>();
        randomInstallPanelRect = randomInstallPanel.GetComponent<RectTransform>();

        originalInstallButtonLocalPosition = installButton.GetComponent<RectTransform>().localPosition;
        originalFillButtonLocalPosition = fillButton.GetComponent<RectTransform>().localPosition;
        originalRandomInstallButtonLocalPosition = randomInstallButton.GetComponent<RectTransform>().localPosition;

        SetButtonSprite(installButton, installButtonNormalSprite);
        SetButtonSprite(fillButton, fillButtonNormalSprite);
        SetButtonSprite(randomInstallButton, randomInstallButtonNormalSprite);
    }

    void SetMode(Button button, GameObject panel, Sprite normalSprite, Sprite selectedSprite)
    {
        if (activeButton != null)
        {
            SetButtonSprite(activeButton, GetNormalSprite(activeButton));
        }

        if (activePanel != null)
        {
            activePanel.SetActive(false);
        }

        panel.SetActive(true);
        activePanel = panel;
        SetButtonSprite(button, selectedSprite);
        activeButton = button;

        float offsetX = 0f;

        if (panel == installPanel)
        {
            offsetX = installPanelRect.rect.width;
            itemPanelUI.PopulateItemPanel();
        }
        else if (panel == fillPanel)
        {
            offsetX = fillPanelRect.rect.width;
        }
        else if (panel == randomInstallPanel)
        {
            offsetX = randomInstallPanelRect.rect.width;
        }

        MoveButtons(offsetX);
    }

    void MoveButtons(float offsetX)
    {
        installButton.GetComponent<RectTransform>().localPosition = originalInstallButtonLocalPosition + new Vector3(offsetX, 0, 0);
        fillButton.GetComponent<RectTransform>().localPosition = originalFillButtonLocalPosition + new Vector3(offsetX, 0, 0);
        randomInstallButton.GetComponent<RectTransform>().localPosition = originalRandomInstallButtonLocalPosition + new Vector3(offsetX, 0, 0);
    }

    void SetButtonSprite(Button button, Sprite sprite)
    {
        button.GetComponent<Image>().sprite = sprite;
    }

    Sprite GetNormalSprite(Button button)
    {
        if (button == installButton)
        {
            return installButtonNormalSprite;
        }
        else if (button == fillButton)
        {
            return fillButtonNormalSprite;
        }
        else if (button == randomInstallButton)
        {
            return randomInstallButtonNormalSprite;
        }
        return null;
    }

    Sprite GetSelectedSprite(Button button)
    {
        if (button == installButton)
        {
            return installButtonSelectedSprite;
        }
        else if (button == fillButton)
        {
            return fillButtonSelectedSprite;
        }
        else if (button == randomInstallButton)
        {
            return randomInstallButtonSelectedSprite;
        }
        return null;
    }
}
