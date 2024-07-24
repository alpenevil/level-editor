using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public Button installButton;
    public Button fillButton;
    public Button randomInstallButton;
    public Button exitButton; 

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

    public GameObject exitConfirmationPanel;
    public Button exitYesButton;
    public Button exitCancelButton;

    private GameObject activePanel;
    private Button activeButton;
    private RectTransform installPanelRect;
    private RectTransform fillPanelRect;
    private RectTransform randomInstallPanelRect;
    private RectTransform installButtonRect;
    private RectTransform fillButtonRect;
    private RectTransform randomInstallButtonRect;
    private CanvasGroup mainCanvasGroup;
    private Vector2 originalInstallButtonPos;
    private Vector2 originalFillButtonPos;
    private Vector2 originalRandomInstallButtonPos;

    void Start()
    {
        installButton.onClick.AddListener(() => SetMode(installButton, installPanel, installButtonNormalSprite, installButtonSelectedSprite));
        fillButton.onClick.AddListener(() => SetMode(fillButton, fillPanel, fillButtonNormalSprite, fillButtonSelectedSprite));
        randomInstallButton.onClick.AddListener(() => SetMode(randomInstallButton, randomInstallPanel, randomInstallButtonNormalSprite, randomInstallButtonSelectedSprite));
        exitButton.onClick.AddListener(ShowExitConfirmationPanel);

        installPanel.SetActive(false);
        fillPanel.SetActive(false);
        randomInstallPanel.SetActive(false);

        installPanelRect = installPanel.GetComponent<RectTransform>();
        fillPanelRect = fillPanel.GetComponent<RectTransform>();
        randomInstallPanelRect = randomInstallPanel.GetComponent<RectTransform>();
        installButtonRect = installButton.GetComponent<RectTransform>();
        fillButtonRect = fillButton.GetComponent<RectTransform>();
        randomInstallButtonRect = randomInstallButton.GetComponent<RectTransform>();

        originalInstallButtonPos = installButtonRect.anchoredPosition;
        originalFillButtonPos = fillButtonRect.anchoredPosition;
        originalRandomInstallButtonPos = randomInstallButtonRect.anchoredPosition;

        SetButtonSprite(installButton, installButtonNormalSprite);
        SetButtonSprite(fillButton, fillButtonNormalSprite);
        SetButtonSprite(randomInstallButton, randomInstallButtonNormalSprite);

        Canvas.ForceUpdateCanvases();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        exitYesButton.onClick.AddListener(ExitToMainMenu);
        exitCancelButton.onClick.AddListener(CloseExitConfirmationPanel);

        exitConfirmationPanel.SetActive(false);

        mainCanvasGroup = GetComponent<CanvasGroup>();
        if (mainCanvasGroup == null)
        {
            mainCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void SetMode(Button button, GameObject panel, Sprite normalSprite, Sprite selectedSprite)
    {
        if (activeButton == button)
        {
            CloseActivePanel();
            return;
        }

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

        float offsetX = 0;
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
        installButtonRect.anchoredPosition = new Vector2(offsetX, installButtonRect.anchoredPosition.y);
        fillButtonRect.anchoredPosition = new Vector2(offsetX, fillButtonRect.anchoredPosition.y);
        randomInstallButtonRect.anchoredPosition = new Vector2(offsetX, randomInstallButtonRect.anchoredPosition.y);
    }

    void ResetButtonPositions()
    {
        installButtonRect.anchoredPosition = originalInstallButtonPos;
        fillButtonRect.anchoredPosition = originalFillButtonPos;
        randomInstallButtonRect.anchoredPosition = originalRandomInstallButtonPos;
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

    void CloseActivePanel()
    {
        if (activeButton != null)
        {
            SetButtonSprite(activeButton, GetNormalSprite(activeButton));
            activeButton = null;
        }

        if (activePanel != null)
        {
            activePanel.SetActive(false);
            activePanel = null;
        }

        ResetButtonPositions();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (activePanel != null)
        {
            float offsetX = activePanel.GetComponent<RectTransform>().rect.width;
            MoveButtons(offsetX);
        }
    }

    public void ShowExitConfirmationPanel()
    {
        exitConfirmationPanel.SetActive(true);
        SetMainCanvasGroupInteractable(false);
    }

    public void CloseExitConfirmationPanel()
    {
        exitConfirmationPanel.SetActive(false);
        SetMainCanvasGroupInteractable(true);
    }

    void ExitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void SetMainCanvasGroupInteractable(bool isInteractable)
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.interactable = isInteractable;
            mainCanvasGroup.blocksRaycasts = isInteractable;
        }
    }
}
