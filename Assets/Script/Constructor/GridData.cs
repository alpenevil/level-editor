using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
           
        }

    }

    public List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public bool HasObjectAt(Vector3Int gridPosition)
    {
        return placedObjects.ContainsKey(gridPosition);
    }

    public void ClearGrid()
    {
        Debug.Log("Clearing the grid with " + placedObjects.Count + " entries.");
        placedObjects.Clear();
    }

    public void ClearOccupiedCells()
    {
        foreach (var kvp in placedObjects)
        {
            PlacementData data = kvp.Value;
            data.occupiedPositions.Clear();
        }
    }

    public void SaveOccupiedCells()
    {
        foreach (var kvp in placedObjects)
        {
            PlacementData data = kvp.Value;
            data.occupiedPositions = new List<Vector3Int>(data.occupiedPositions);
        }
    }

    public void LoadOccupiedCells()
    {
        foreach (var kvp in placedObjects)
        {
            PlacementData data = kvp.Value;
            data.occupiedPositions = new List<Vector3Int>(data.occupiedPositions);
        }
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {

        if (placedObjects.TryGetValue(gridPosition, out PlacementData data))
        {
            foreach (var pos in data.occupiedPositions)
            {
                placedObjects.Remove(pos);
                Debug.Log("Successfully removed position from grid: " + pos);
            }
        }
        else
        {
            Debug.Log("No object at position " + gridPosition + " to remove.");
        }

    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
