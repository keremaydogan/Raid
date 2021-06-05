using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VertexPath : MonoBehaviour
{
    public Vector3[] points;

    public Vector3[] GetPath()
    {
        Vector3[] path = new Vector3[points.Length];
        Vector3 goPos = gameObject.transform.position;
        for(int i = 0; i < path.Length; i++)
        {
            path[i] = goPos + points[i];
        }
        return path;
    }
}
