using System;
using System.Collections.Generic;
using UnityEngine;

public class FillingState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback;

    private Vector3Int startGridPosition;
    private bool isSelecting = false;

    public FillingState(int iD,
                        Grid grid,
                        PreviewSystem previewSystem,
                        ObjectsDatabaseSO database,
                        GridData floorData,
                        ObjectPlacer objectPlacer,
                        SoundFeedback soundFeedback)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, Vector2Int.one); 
        InputManager.Instance.OnCursorMovement += UpdateCursorMovement;
    }

    public void StartSelecting(Vector3Int startPosition)
    {
        startGridPosition = startPosition;
        isSelecting = true;
        previewSystem.ShowSelectionArea(startGridPosition, startGridPosition);
    }

    public void UpdateSelection(Vector3Int currentPosition)
    {
        if (isSelecting)
        {
            previewSystem.UpdateSelectionArea(startGridPosition, currentPosition);
        }
    }

    public void EndSelection(Vector3Int endPosition)
    {
        if (isSelecting)
        {
            previewSystem.UpdateSelectionArea(startGridPosition, endPosition);
            FillArea(startGridPosition, endPosition);
            isSelecting = false;
            previewSystem.HideSelectionArea(endPosition);
        }
    }

    private void FillArea(Vector3Int start, Vector3Int end)
    {
        Vector3Int min = Vector3Int.Min(start, end);
        Vector3Int max = Vector3Int.Max(start, end);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int z = min.z; z <= max.z; z++)
            {
                Vector3Int position = new Vector3Int(x, 0, z);
                if (floorData.HasObjectAt(position))
                {
                    int representationIndex = floorData.GetRepresentationIndex(position);
                    objectPlacer.RemoveObjectAt(representationIndex);
                    floorData.RemoveObjectAt(position);
                }
                int placedObjectIndex = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab,
                                     grid.CellToWorld(position),
                                     Vector3.zero);
                floorData.AddObjectAt(position, Vector2Int.one, database.objectsData[selectedObjectIndex].ID, placedObjectIndex);
            }
        }
    }

    public void OnAction(Vector3Int gridPosition) { }
    public void EndState()
    {
        previewSystem.StopShowingPreview();
        InputManager.Instance.OnCursorMovement -= UpdateCursorMovement;
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return true;
    }
    public void UpdateState(Vector3Int gridPosition) {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity, Vector3.zero);
    }

    private void UpdateCursorMovement(Vector3Int gridPosition)
    {
        UpdateSelection(gridPosition);
        
    }


}
