using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathDrawer : MonoBehaviour
{
    public Color pathColor;

    private List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = pathColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        foreach (var pathTransform in pathTransforms)
        {
            if(pathTransform != transform)
            {
                nodes.Add(pathTransform);
            }
        }

        

        Vector3 currentNode;
        Vector3 previousNode = Vector3.zero;

        foreach (var node in nodes)
        {
            currentNode = node.position;

            if (previousNode != Vector3.zero)
                Gizmos.DrawLine(previousNode, currentNode);

            previousNode = node.position;
        }
    }
}
