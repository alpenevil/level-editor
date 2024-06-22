using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private void Start()
    {
        if (pointA != null && pointB != null)
        {
            StartCoroutine(StartPatrol());
        }
    }

    private IEnumerator StartPatrol()
    {
        while (true)
        {
            yield return StartCoroutine(FollowPath(pointA.position, pointB.position));
            yield return StartCoroutine(FollowPath(pointB.position, pointA.position));
        }
    }

    private IEnumerator FollowPath(Vector3 startPoint, Vector3 endPoint)
    {
        List<Node> path = AStarPathfinding.Instance.FindPath(startPoint, endPoint);
        if (path == null)
        {
            Debug.LogError("Path not found");
            yield break;
        }

        foreach (Node node in path)
        {
            Vector3 targetPosition = node.worldPosition;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
