using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemPanelUI : MonoBehaviour
{
    public ObjectsDatabaseSO objectsDatabase;
    public GameObject itemUIPrefab;
    public Transform itemPanel;
    public PlacementSystem placementSystem;

    public Sprite normalBackgroundSprite;
    public Sprite selectedBackgroundSprite;

    private List<ItemUI> itemUIList = new List<ItemUI>();
    private ItemUI selectedItemUI;

    void Start()
    {
        PopulateItemPanel();
    }

    public void PopulateItemPanel()
    {
        foreach (Transform child in itemPanel)
        {
            Destroy(child.gameObject);
        }

        itemUIList.Clear();

        foreach (ObjectData objectData in objectsDatabase.objectsData)
        {
            GameObject itemUIGameObject = Instantiate(itemUIPrefab, itemPanel);
            ItemUI itemUI = itemUIGameObject.GetComponent<ItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(this, objectData.ID, objectData.Name, objectData.Icon, normalBackgroundSprite, selectedBackgroundSprite);
                itemUIList.Add(itemUI);
            }
            else
            {
                Debug.LogError("ItemUI component not found in the prefab.");
            }
        }
    }

    public void OnItemButtonClicked(int itemId)
    {
        ObjectData objectData = objectsDatabase.objectsData.Find(obj => obj.ID == itemId);
        if (objectData != null)
        {
            placementSystem.StartPlacement(itemId);
        }
        else
        {
            Debug.LogError("ObjectData not found for ID: " + itemId);
        }
    }

    public void OnItemSelected(ItemUI itemUI)
    {
        if (selectedItemUI != null)
        {
            selectedItemUI.SetSelected(false);
        }

        selectedItemUI = itemUI;
        selectedItemUI.SetSelected(true);
    }

    public void FilterByPurpose(ObjectPurpose purpose)
    {
        foreach (Transform child in itemPanel)
        {
            Destroy(child.gameObject);
        }

        itemUIList.Clear();

        foreach (ObjectData objectData in objectsDatabase.objectsData)
        {
            if (objectData.Purpose == purpose)
            {
                GameObject itemUIGameObject = Instantiate(itemUIPrefab, itemPanel);
                ItemUI itemUI = itemUIGameObject.GetComponent<ItemUI>();

                if (itemUI != null)
                {
                    itemUI.Initialize(this, objectData.ID, objectData.Name, objectData.Icon, normalBackgroundSprite, selectedBackgroundSprite);
                    itemUIList.Add(itemUI);
                }
                else
                {
                    Debug.LogError("ItemUI component not found in the prefab.");
                }
            }
        }
    }
}
