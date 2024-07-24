using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    public LayerMask walkableMask;
    public LayerMask unwalkableMask;
    public LayerMask floorMask; 
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public TerrainType[] walkableRegions;
    LayerMask walkableLayer;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }

        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                // Проверка на наличие пола
                bool hasFloor = Physics.CheckSphere(worldPoint, nodeRadius, floorMask);
                if (!hasFloor)
                {
                    walkable = false;
                }

                int movementPenalty = 0;

                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
               
            }
        }
    }

    public void MarkUnwalkable(Vector3Int position, Vector2Int size, Vector3 rotation)
    {
       
        Vector3Int offset = Vector3Int.FloorToInt(GetRotationOffset(rotation, size));
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int adjustedPosition = position + new Vector3Int(x, 0, y) - offset;
                Node node = NodeFromWorldPoint(new Vector3(adjustedPosition.x, 0, adjustedPosition.z));
                if (node != null)
                {
                    node.walkable = false;
                    
                }
            }
        }
    }

    public void MarkWalkable(Vector3Int position, Vector2Int size, Vector3 rotation)
    {
        
        Vector3Int offset = Vector3Int.FloorToInt(GetRotationOffset(rotation, size));
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int adjustedPosition = position + new Vector3Int(x, 0, y) - offset;
                Node node = NodeFromWorldPoint(new Vector3(adjustedPosition.x, 0, adjustedPosition.z));
                if (node != null)
                {
                    node.walkable = true;
                   
                }
            }
        }
    }

    private Vector3 GetRotationOffset(Vector3 rotation, Vector2Int objectSize)
    {
        float normalizedY = Mathf.Round(rotation.y % 360);
        normalizedY = (normalizedY < 0) ? normalizedY + 360 : normalizedY;

        switch (normalizedY)
        {
            case 0:
                return Vector3.zero;
            case 90:
                return new Vector3(0, 0, objectSize.y);
            case 180:
                return new Vector3(objectSize.x, 0, objectSize.y);
            case 270:
                return new Vector3(objectSize.x, 0, 0);
            default:
                return Vector3.zero;
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
