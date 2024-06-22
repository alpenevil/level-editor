using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private ObjectsDatabaseSO database;
    [SerializeField]
    private GameObject gridVisualization;
    [SerializeField]
    private AudioClip correctPlacementClip, wrongPlacementClip;
    [SerializeField]
    private AudioSource source;

    public GridData floorData, furnitureData;
    [SerializeField]
    private PreviewSystem preview;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;
    RandomState currentRandomState;
    [SerializeField]
    private SoundFeedback soundFeedback;

    private void Awake()
    {
        gridVisualization.SetActive(false);
        floorData = new GridData();
        furnitureData = new GridData();
    }

    private void Start()
    {
        inputManager.OnRemovePressed += StartRemoving;
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        InputManager.Instance.OnRotatePressed += RotateSelectedObject;
    }

    private void HandleDoubleClick(Vector3Int position)
    {
        Debug.Log("Double click at position: " + position);
       
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3Int gridPosition = grid.WorldToCell(inputManager.GetSelectedMapPosition());
        buildingState.OnAction(gridPosition);
    }

    public void RotateSelectedObject()
    {
        if (buildingState != null && buildingState is PlacementState)
        {
            Vector3Int cursorPosition = grid.WorldToCell(inputManager.GetSelectedMapPosition());
            ((PlacementState)buildingState).RotateSelectedObject(cursorPosition);
        }
    }

    public void StopPlacement()
    {
        soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;

        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        InputManager.Instance.OnRotatePressed -= RotateSelectedObject;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;

    }

    private void Update()
    {
        if (buildingState == null)
            return;

        Vector3Int gridPosition = grid.WorldToCell(inputManager.GetSelectedMapPosition());
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    public void StartFilling(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new FillingState(ID, grid, preview, database, floorData, objectPlacer, soundFeedback);
        inputManager.OnStartSelection += StartSelection;
        inputManager.OnUpdateSelection += UpdateSelection;
        inputManager.OnEndSelection += EndSelection;
    }

    private void StartSelection(Vector3Int startPosition)
    {
        if (buildingState is FillingState fillingState)
        {
            fillingState.StartSelecting(startPosition);
        }
    }

    private void UpdateSelection(Vector3Int currentPosition)
    {
        if (buildingState is FillingState fillingState)
        {
            fillingState.UpdateSelection(currentPosition);
        }
    }

    private void EndSelection(Vector3Int endPosition)
    {
        if (buildingState is FillingState fillingState)
        {
            fillingState.EndSelection(endPosition);
        }
    }

    public void StartRandomPlacement()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        currentRandomState = new RandomState(database, grid, preview, floorData, furnitureData, objectPlacer, soundFeedback);
        buildingState = currentRandomState;
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void UpdateRandomSelections()
    {
        if (currentRandomState != null)
        {
            currentRandomState.UpdateRandomSelections(database.objectsData);
            currentRandomState.UpdatePreview(); 
        }
    }

    public void UpdatePreview()
    {
        if (buildingState is RandomState randomState)
        {
            randomState.UpdatePreview();
        }
    }

    private void OnDestroy()
    {
        inputManager.OnRemovePressed -= StartRemoving;
    }
}
