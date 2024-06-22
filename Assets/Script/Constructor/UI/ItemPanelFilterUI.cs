using UnityEngine;
using TMPro; 
using System.Collections.Generic;

public class ItemPanelFilterUI : MonoBehaviour
{
    public ItemPanelUI itemPanelUI;
    public TMP_Dropdown filterDropdown; 

    void Start()
    {
        PopulateFilterDropdown();
        filterDropdown.onValueChanged.AddListener(OnFilterChanged);
    }

    void PopulateFilterDropdown()
    {
        filterDropdown.ClearOptions();
        List<string> options = new List<string> { "All" };

        foreach (ObjectPurpose purpose in System.Enum.GetValues(typeof(ObjectPurpose)))
        {
            if (purpose != ObjectPurpose.None)
            {
                options.Add(purpose.ToString());
            }
        }

        filterDropdown.AddOptions(options);
    }

    void OnFilterChanged(int index)
    {
        if (index == 0)
        {
            itemPanelUI.PopulateItemPanel();
        }
        else
        {
            ObjectPurpose selectedPurpose = (ObjectPurpose)System.Enum.Parse(typeof(ObjectPurpose), filterDropdown.options[index].text);
            itemPanelUI.FilterByPurpose(selectedPurpose);
        }
    }
}
