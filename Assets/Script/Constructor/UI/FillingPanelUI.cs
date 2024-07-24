using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FillingPanelUI : MonoBehaviour
{
    public ObjectsDatabaseSO objectsDatabase;
    public GameObject itemUIPrefab;
    public Transform itemPanel;
    public PlacementSystem placementSystem;

    public Sprite normalBackgroundSprite;
    public Sprite selectedBackgroundSprite;

    private List<FillingItemUI> itemUIList = new List<FillingItemUI>();
    private FillingItemUI selectedItem;

    void Start()
    {
        PopulateItemPanel();
    }

    public void PopulateItemPanel()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in itemPanel)
        {
            children.Add(child.gameObject);
        }

        foreach (GameObject child in children)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }

        itemUIList.Clear();

        foreach (ObjectData objectData in objectsDatabase.objectsData)
        {
            if (objectData.Purpose != ObjectPurpose.Ground)
            {
                continue;
            }

            GameObject itemUIGameObject = Instantiate(itemUIPrefab, itemPanel);
            FillingItemUI itemUI = itemUIGameObject.GetComponent<FillingItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(this, objectData.ID, objectData.Name, objectData.Icon, normalBackgroundSprite, selectedBackgroundSprite);
                itemUIList.Add(itemUI);
            }
            else
            {
                Debug.LogError("FillingItemUI component not found in the prefab.");
            }
        }

        // Select and highlight the first item by default
        if (itemUIList.Count > 0)
        {
            OnItemButtonClicked(itemUIList[0].ItemId);
        }
    }

    public void OnItemButtonClicked(int itemId)
    {
        foreach (FillingItemUI itemUI in itemUIList)
        {
            itemUI.UpdateBackground(itemUI.ItemId == itemId);
        }

        placementSystem.StartFilling(itemId);
    }

    public void OnEnable()
    {
        PopulateItemPanel();
    }
}
