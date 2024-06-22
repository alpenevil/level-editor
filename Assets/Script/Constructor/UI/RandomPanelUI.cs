using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomPanelUI : MonoBehaviour
{
    public ObjectsDatabaseSO objectsDatabase;
    public GameObject itemUIPrefab; 
    public Transform itemPanel;
    public PlacementSystem placementSystem; 

    public Sprite normalBackgroundSprite;
    public Sprite selectedBackgroundSprite; 

    private List<RandomItemUI> itemUIList = new List<RandomItemUI>(); 

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
            GameObject itemUIGameObject = Instantiate(itemUIPrefab, itemPanel);
            RandomItemUI itemUI = itemUIGameObject.GetComponent<RandomItemUI>();

            if (itemUI != null)
            {
                itemUI.Initialize(this, objectData.ID, objectData.Name, objectData.Icon, objectData.IsRandomSelectionEnabled, normalBackgroundSprite, selectedBackgroundSprite);
                itemUIList.Add(itemUI);
            }
            else
            {
                Debug.LogError("RandomItemUI component not found in the prefab.");
            }
        }
    }

    public void OnItemButtonClicked(int itemId, bool isRandomSelectionEnabled)
    {
        ObjectData objectData = objectsDatabase.objectsData.Find(obj => obj.ID == itemId);
        if (objectData != null)
        {
            objectData.IsRandomSelectionEnabled = isRandomSelectionEnabled;
            UpdateRandomSelections();
            placementSystem.StartRandomPlacement(); 
        }
        else
        {
            Debug.LogError("ObjectData not found for ID: " + itemId);
        }
    }

    private void UpdateRandomSelections()
    {
        placementSystem.UpdateRandomSelections();
    }
}
