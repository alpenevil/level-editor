using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> placedGameObjects = new();

    public GameObject Prefab { get; private set; } 

    public int PlaceObject(GameObject prefab, Vector3 position, Vector3 modelOffset)
    {
        GameObject newObject = Instantiate(prefab);

        Vector3 adjustedPosition = position + modelOffset;
        newObject.transform.position = adjustedPosition;

        placedGameObjects.Add(newObject);
        Prefab = prefab; 
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }

    public void SetObjectRotation(int index, Vector3 rotation)
    {
        if (index < 0 || index >= placedGameObjects.Count || placedGameObjects[index] == null)
        {
            Debug.LogWarning("Invalid index or object not found at index.");
            return;
        }

        placedGameObjects[index].transform.rotation = Quaternion.Euler(rotation);
    }
}
