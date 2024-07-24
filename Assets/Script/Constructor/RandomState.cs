using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomState : IBuildingState
{
    private ObjectsDatabaseSO database;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    private Vector3 originalPosition;

    private Vector3 objectRotation = Vector3.zero;

    private int randomIndex = -1;

    private List<ObjectData> randomSelections;

    public enum Direction
    {
        Down,
        Left,
        Up,
        Right
    }

    private Direction currentDirection = Direction.Down;

    public RandomState(ObjectsDatabaseSO database,
                    Grid grid,
                    PreviewSystem previewSystem,
                    GridData floorData,
                    GridData furnitureData,
                    ObjectPlacer objectPlacer,
                    SoundFeedback soundFeedback)
    {
        this.database = database;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        this.originalPosition = grid.CellToWorld(Vector3Int.zero);

        this.randomSelections = new List<ObjectData>();
        foreach (ObjectData objData in database.objectsData)
        {
            if (objData.IsRandomSelectionEnabled)
            {
                randomSelections.Add(objData);
            }
        }

        UpdateRandomIndex();
        if (randomIndex != -1)
        {
            this.currentDirection = GetRandomDirection();
            if (randomSelections[this.randomIndex].Prefab != null)
            {
                previewSystem.StartShowingPlacementPreview(
                       randomSelections[this.randomIndex].Prefab,
                       randomSelections[this.randomIndex].Size);

                Vector3 initialPreviewRotation = new Vector3(0, GetRotationAngle(this.currentDirection), 0);
                Vector3 modelOffset = GetRotationOffset(this.currentDirection, randomSelections[this.randomIndex].Size);
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                previewSystem.UpdatePreviewRotation(initialPreviewRotation, cursorPosition, modelOffset);
            }
            else
            {
                //Debug.LogWarning("No prefab available for random selection.");
            }
        }
        else
        {
            Debug.LogWarning("No objects available for random selection.");
            previewSystem.StartShowingPlacementPreview(null, Vector2Int.one); 
        }
    }

    public void StartPlacement(Vector3Int gridPosition)
    {
        OnAction(gridPosition);
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (randomSelections == null || randomSelections.Count == 0)
        {
            Debug.LogWarning("No objects available for random selection.");
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            previewSystem.StopShowingPreview();
            return;
        }

        if (randomIndex != -1)
        {
            PlaceRandomObject(randomIndex, gridPosition);
            randomIndex = GetRandomIndex(randomSelections);
            currentDirection = GetRandomDirection();
            if (randomIndex != -1 && randomSelections[randomIndex].Prefab != null)
            {
                Vector3 modelOffset = originalPosition + GetRotationOffset(currentDirection, randomSelections[randomIndex].Size);
                ObjectData newSelectedObject = randomSelections[randomIndex];
                previewSystem.StartShowingPlacementPreview(
                    newSelectedObject.Prefab,
                    newSelectedObject.Size);

                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 previewRotation = new Vector3(0, GetRotationAngle(currentDirection), 0);
                previewSystem.UpdatePreviewRotation(previewRotation, cursorPosition, modelOffset);
            }
            else
            {
               // Debug.LogWarning("No new preview");
                previewSystem.StartShowingPlacementPreview(null, Vector2Int.one); 
            }
        }
        else
        {
            Debug.LogWarning("No objects available for random selection.");
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            previewSystem.StopShowingPreview();
        }
    }

    public void UpdatePreview()
    {
        if (randomSelections == null || randomSelections.Count == 0)
        {
           // Debug.LogWarning("No objects available for random selection.");
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            previewSystem.StopShowingPreview();
            return;
        }

        randomIndex = GetRandomIndex(randomSelections);
        currentDirection = GetRandomDirection();

        if (randomIndex != -1 && randomSelections[randomIndex].Prefab != null)
        {
            Vector3 modelOffset = originalPosition + GetRotationOffset(currentDirection, randomSelections[randomIndex].Size);
            ObjectData newSelectedObject = randomSelections[randomIndex];
            previewSystem.StartShowingPlacementPreview(newSelectedObject.Prefab, newSelectedObject.Size);

            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 previewRotation = new Vector3(0, GetRotationAngle(currentDirection), 0);
            previewSystem.UpdatePreviewRotation(previewRotation, cursorPosition, modelOffset);
        }
        else
        {
           // Debug.LogWarning("No new preview");
            previewSystem.StartShowingPlacementPreview(null, Vector2Int.one); 
        }
    }


    private int GetRandomIndex(List<ObjectData> randomSelections)
    {
        if (randomSelections == null)
        {
           // Debug.LogError("Random selections list is not initialized.");
            return -1;
        }

        if (randomSelections.Count > 0)
        {
            return Random.Range(0, randomSelections.Count);
        }

        return -1;
    }

    private void PlaceRandomObject(int index, Vector3Int gridPosition)
    {
        ObjectData selectedObject = randomSelections[index];
        GameObject prefab = selectedObject.Prefab;
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        int rotationAngle = GetRotationAngle(currentDirection);

        Vector3 modelOffset = originalPosition + GetRotationOffset(currentDirection, selectedObject.Size);

        bool placementValidity = CheckPlacementValidity(selectedObject, gridPosition);

        if (placementValidity)
        {
            int placedObjectIndex = objectPlacer.PlaceObject(prefab, worldPosition, modelOffset);

            objectPlacer.SetObjectRotation(placedObjectIndex, new Vector3(0, rotationAngle, 0));

            Vector2Int size = selectedObject.Size;
            if (Mathf.Abs(rotationAngle % 180) > 0.01f)
            {
                size = new Vector2Int(size.y, size.x);
            }

            if (selectedObject.Type == ObjectType.Floor)
            {
                floorData.AddObjectAt(gridPosition, size, selectedObject.ID, placedObjectIndex);
            }
            else if (selectedObject.Type == ObjectType.Furniture)
            {
                furnitureData.AddObjectAt(gridPosition, size, selectedObject.ID, placedObjectIndex);
            }

            soundFeedback.PlaySound(SoundType.Place);
        }
        else
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
        }

        if (randomIndex != -1 && randomSelections.Count > 0 && randomSelections[randomIndex].Prefab != null)
        {
            ObjectData newSelectedObject = database.objectsData[randomIndex];
            previewSystem.StartShowingPlacementPreview(newSelectedObject.Prefab, newSelectedObject.Size);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 previewRotation = new Vector3(0, GetRotationAngle(currentDirection), 0);
            previewSystem.UpdatePreviewRotation(previewRotation, cursorPosition, modelOffset);
        }
    }

    private bool CheckPlacementValidity(ObjectData selectedObject, Vector3Int gridPosition)
    {
        GridData selectedData = selectedObject.Type == ObjectType.Floor ? floorData : furnitureData;
        Vector2Int size = selectedObject.Size;
        int rotationAngle = GetRotationAngle(currentDirection);

        if (Mathf.Abs(rotationAngle % 180) > 0.01f)
        {
            size = new Vector2Int(size.y, size.x);
        }

        return selectedData.CanPlaceObjectAt(gridPosition, size);
    }

    private Direction GetRandomDirection()
    {
        int randomIndex = Random.Range(0, 4);

        switch (randomIndex)
        {
            case 0:
                return Direction.Down;
            case 1:
                return Direction.Left;
            case 2:
                return Direction.Up;
            case 3:
                return Direction.Right;
            default:
                return Direction.Down; 
        }
    }

    public int GetRotationAngle(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }

    private Vector3 GetRotationOffset(Direction direction, Vector2Int objectSize)
    {
        switch (direction)
        {
            case Direction.Down:
                return Vector3.zero;

            case Direction.Left:
                return new Vector3(0, 0, objectSize.x);

            case Direction.Up:
                return new Vector3(objectSize.x, 0, objectSize.y);

            case Direction.Right:
                return new Vector3(objectSize.y, 0, 0);

            default:
                return Vector3.zero;
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (randomSelections == null || randomSelections.Count == 0)
        {
           // Debug.LogWarning("Random selections list is empty.");
            return;
        }

        if (randomIndex < 0 || randomIndex >= randomSelections.Count)
        {
           // Debug.LogError("randomIndex is out of range: " + randomIndex);
            return;
        }

        ObjectData selectedObject = randomSelections[randomIndex];
        bool placementValidity = CheckPlacementValidity(selectedObject, gridPosition);
        Vector3 modelOffset = originalPosition + GetRotationOffset(currentDirection, selectedObject.Size);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity, modelOffset);
    }

    public void UpdateRandomSelections(List<ObjectData> objectsData)
    {
        randomSelections.Clear();
        foreach (ObjectData objData in objectsData)
        {
            if (objData.IsRandomSelectionEnabled)
            {
                randomSelections.Add(objData);
            }
        }
       // Debug.Log("Updated Random Selections for Placement:");
        UpdateRandomIndex();
    }

    public void UpdateRandomIndex()
    {
        if (randomSelections.Count > 0)
        {
            randomIndex = GetRandomIndex(randomSelections);
        }
        else
        {
            randomIndex = -1;
        }
    }
}
