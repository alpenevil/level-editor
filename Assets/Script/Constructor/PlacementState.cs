using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback;

    private Vector3 originalPosition;
    private Vector3 objectRotation = Vector3.zero;

    public enum Direction
    {
        Down,
        Left,
        Up,
        Right
    }

    private Direction currentDirection = Direction.Down;

    public PlacementState(int iD,
                       Grid grid,
                       PreviewSystem previewSystem,
                       ObjectsDatabaseSO database,
                       GridData floorData,
                       GridData furnitureData,
                       ObjectPlacer objectPlacer,
                       SoundFeedback soundFeedback)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        originalPosition = grid.CellToWorld(Vector3Int.zero);

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            Vector3 modelOffset = CalculateModelOffset(database.objectsData[selectedObjectIndex].Size, originalPosition);
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {iD}");
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
       // Debug.Log("Attempting to place an object...");

        if ((database.objectsData[selectedObjectIndex].Type == ObjectType.SpawnPoint || database.objectsData[selectedObjectIndex].Type == ObjectType.EnemySpawnPoint)
            && !IsFloorBelow(gridPosition))
        {
            Debug.LogWarning("Cannot place spawn point or enemy patrol point without a floor below.");
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false, Vector3.zero);
            return;
        }

        if (database.objectsData[selectedObjectIndex].Type == ObjectType.EnemySpawnPoint)
        {
           // Debug.Log("Checking for existing enemy patrol points...");

            if (AreEnemyPatrolPointsPlaced())
            {
                Debug.LogWarning("Two enemy patrol points already exist. Remove existing points before adding new ones.");
                soundFeedback.PlaySound(SoundType.wrongPlacement);
                return;
            }

            int patrolPointCount = 0;
            foreach (var placedObject in objectPlacer.placedGameObjects)
            {
                if (placedObject == null)
                {
                    continue;
                }

                string placedObjectName = placedObject.name.Replace("(Clone)", "").Trim();
                ObjectData objectData = database.objectsData.Find(obj => obj.Prefab.name == placedObjectName);
                if (objectData != null && objectData.Type == ObjectType.EnemySpawnPoint)
                {
                    patrolPointCount++;
                }
            }

            if (patrolPointCount == 0)
            {
              //  Debug.Log("Placing the first enemy patrol point (A).");
                SetEnemyPatrolPointA(grid.CellToWorld(gridPosition));
            }
            else if (patrolPointCount == 1)
            {
               // Debug.Log("Placing the second enemy patrol point (B).");
                SetEnemyPatrolPointB(grid.CellToWorld(gridPosition));
            }
        }
        else if (database.objectsData[selectedObjectIndex].Type == ObjectType.SpawnPoint)
        {
        //    Debug.Log("Checking for existing player spawn point...");
            if (IsPlayerSpawnPointPlaced())
            {
                Debug.LogWarning("A player spawn point already exists. Remove the existing one before placing a new one.");
                soundFeedback.PlaySound(SoundType.wrongPlacement);
                return;
            }
            SetPlayerSpawnPoint(grid.CellToWorld(gridPosition));
        }

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            soundFeedback.PlaySound(SoundType.wrongPlacement);
            previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false, Vector3.zero);
            return;
        }

        soundFeedback.PlaySound(SoundType.Place);
        Vector3 modelOffset = CalculateModelOffset(database.objectsData[selectedObjectIndex].Size, originalPosition);
        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition), modelOffset);

        GridData selectedData = database.objectsData[selectedObjectIndex].Type == ObjectType.Floor ? floorData : furnitureData;
        Vector2Int size = database.objectsData[selectedObjectIndex].Size;
        if (Mathf.Abs(objectRotation.y % 180) > 0.01f)
        {
            size = new Vector2Int(size.y, size.x);
        }
        selectedData.AddObjectAt(gridPosition, size, database.objectsData[selectedObjectIndex].ID, index);

        objectPlacer.SetObjectRotation(index, objectRotation);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity, modelOffset);
    }

    public bool IsPlayerSpawnPointPlaced()
    {
        foreach (var placedObject in objectPlacer.placedGameObjects)
        {
            if (placedObject == null)
            {
                continue;
            }

            string placedObjectName = placedObject.name.Replace("(Clone)", "").Trim();
            ObjectData objectData = database.objectsData.Find(obj => obj.Prefab.name == placedObjectName);
            if (objectData != null && objectData.Type == ObjectType.SpawnPoint)
            {
               // Debug.Log($"Existing player spawn point found: {placedObject.name}");
                return true;
            }
        }
      //  Debug.Log("No existing player spawn point found.");
        return false;
    }

    public bool AreEnemyPatrolPointsPlaced()
    {
        int patrolPointCount = 0;
        foreach (var placedObject in objectPlacer.placedGameObjects)
        {
            if (placedObject == null)
            {
                continue;
            }

            string placedObjectName = placedObject.name.Replace("(Clone)", "").Trim();
            ObjectData objectData = database.objectsData.Find(obj => obj.Prefab.name == placedObjectName);
            if (objectData != null && objectData.Type == ObjectType.EnemySpawnPoint)
            {
                patrolPointCount++;
            }
        }
        return patrolPointCount >= 2;
    }

    private void SetPlayerSpawnPoint(Vector3 position)
    {
        ObjectData spawnPointData = database.objectsData.Find(obj => obj.Type == ObjectType.SpawnPoint);
        if (spawnPointData == null)
        {
          //  Debug.LogWarning("Spawn point object not found in database.");
            return;
        }

        PlayerPrefs.SetFloat("SpawnPointX", position.x);
        PlayerPrefs.SetFloat("SpawnPointY", position.y);
        PlayerPrefs.SetFloat("SpawnPointZ", position.z);
        PlayerPrefs.Save();
    }

    private void SetEnemyPatrolPointA(Vector3 position)
    {
        PlayerPrefs.SetFloat("EnemyPatrolPointAX", position.x);
        PlayerPrefs.SetFloat("EnemyPatrolPointAY", position.y);
        PlayerPrefs.SetFloat("EnemyPatrolPointAZ", position.z);
        PlayerPrefs.Save();
    }

    private void SetEnemyPatrolPointB(Vector3 position)
    {
        PlayerPrefs.SetFloat("EnemyPatrolPointBX", position.x);
        PlayerPrefs.SetFloat("EnemyPatrolPointBY", position.y);
        PlayerPrefs.SetFloat("EnemyPatrolPointBZ", position.z);
        PlayerPrefs.Save();
    }

    private Vector3 CalculateModelOffset(Vector2Int objectSize, Vector3 originalPosition)
    {
        Vector3 offset = GetRotationOffset(currentDirection, objectSize);
        return originalPosition + offset;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].Type == ObjectType.Floor ? floorData : furnitureData;
        Vector2Int size = database.objectsData[selectedObjectIndex].Size;

        if (Mathf.Abs(objectRotation.y % 180) > 0.01f)
        {
            size = new Vector2Int(size.y, size.x);
        }

        return selectedData.CanPlaceObjectAt(gridPosition, size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if ((database.objectsData[selectedObjectIndex].Type == ObjectType.SpawnPoint || database.objectsData[selectedObjectIndex].Type == ObjectType.EnemySpawnPoint)
            && !IsFloorBelow(gridPosition))
        {
            placementValidity = false;
        }

        Vector3 modelOffset = CalculateModelOffset(database.objectsData[selectedObjectIndex].Size, originalPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity, modelOffset);
    }

    public void RotateSelectedObject(Vector3 cursorPosition)
    {
        currentDirection = GetNextDirection(currentDirection);
        objectRotation = new Vector3(0, GetRotationAngle(currentDirection), 0);
        Vector3 modelOffset = CalculateModelOffset(database.objectsData[selectedObjectIndex].Size, originalPosition);
        previewSystem.UpdatePreviewRotation(objectRotation, cursorPosition, modelOffset);
    }

    public static Direction GetNextDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
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

    private bool IsFloorBelow(Vector3Int position)
    {
        return floorData.HasObjectAt(position);
    }
}
