using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{

    

    

    public void SetMap(Vector3 startPoint, Vector3 endPoint, Vector3[] map)
    {

    }

    private class Nodes
    {
        public Vector3 location;
        public float G;
        public float H;
        public float F { get { return G + H; } }
    }

    private void FixedUpdate()
    {

    }
}
