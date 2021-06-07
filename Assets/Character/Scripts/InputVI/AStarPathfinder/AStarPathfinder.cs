using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NodeState { Open, Closed }

public class AStarPathfinder
{
    Node[] map = new Node[0];

    LayerMask obstacleLays;

    public void AStarAwake()
    {
        obstacleLays = LayerMask.GetMask("Room");
    }

    private class Node
    {
        public Vector3 position;
        public float G;
        public float H;
        public float F { get { return G + H; } }

        public int parentNodeInd;
        public NodeState nodeState;

        public Node(Vector3 position)
        {
            this.position = position;
            nodeState = NodeState.Open;
            G = float.MaxValue;
            parentNodeInd = -1;
        }
    }

    private void SetMap(Vector3 startPoint, Vector3 endPoint, Vector3[] pointMap)
    {
        map = new Node[pointMap.Length + 2];

        map[0] = new Node(startPoint);
        map[0].G = 0;

        map[map.Length - 1] = new Node(endPoint);


        Debug.DrawRay(map[0].position, Vector3.up, Color.magenta);
        Debug.DrawRay(map[map.Length - 1].position, Vector3.up, Color.magenta);


        for (int i = 0; i < pointMap.Length; i++)
        {
            map[i + 1] = new Node(pointMap[i]);
            map[i + 1].H = (endPoint - pointMap[i]).magnitude;
        }
    }

    public Vector3[] ShortestPath(Vector3 startPoint, Vector3 endPoint, Vector3[] pointMap)
    {
        SetMap(startPoint, endPoint, pointMap);

        RaycastHit2D checkWayNor;

        int currentNodeInd = 0;
        int nextNodeInd = -1;
        HashSet<int> walkableNodeInds = new HashSet<int>();

        float GTemp;
        float FMax;

        int repeat = 0;

        while (currentNodeInd != map.Length - 1)
        {
            map[currentNodeInd].nodeState = NodeState.Closed;

            for (int i = 0; i < map.Length; i++)
            {
                if (map[i].nodeState == NodeState.Closed) { continue; }

                checkWayNor = Physics2D.Raycast(map[currentNodeInd].position, (map[i].position - map[currentNodeInd].position), (map[i].position - map[currentNodeInd].position).magnitude, obstacleLays);
                if (checkWayNor.collider == null)
                {
                    GTemp = map[currentNodeInd].G + (map[i].position - map[currentNodeInd].position).magnitude;

                    if (map[i].G > GTemp)
                    {
                        map[i].G = GTemp;
                        map[i].parentNodeInd = currentNodeInd;
                    }

                    walkableNodeInds.Add(i);
                }
            }

            FMax = float.MaxValue;

            Debug.Log("walkableNodeInds.Count " + walkableNodeInds.Count);

            foreach (int ind in walkableNodeInds)
            {
                if (map[ind].Equals(map[map.Length - 1]))
                {
                    map[ind].parentNodeInd = currentNodeInd;
                    currentNodeInd = map.Length - 1;
                    break;
                }
                if (map[ind].F < FMax || (map[ind].F == FMax && map[ind].H > map[currentNodeInd].H))
                {
                    Debug.Log("AAA " + ind);
                    FMax = map[ind].F;
                    nextNodeInd = ind;
                }
            }

            Debug.Log("nextNodeInd " + nextNodeInd);
            map[nextNodeInd].parentNodeInd = currentNodeInd;
            currentNodeInd = nextNodeInd;

            repeat++;
            if (repeat == 10) { break; }
        }

        for (int i = 0; i < map.Length; i++)
        {
            if (map[i].nodeState == NodeState.Closed)
            {
                Debug.DrawRay(map[i].position, Vector3.up, Color.magenta);
            }
        }

        //return CompileWay();

        return new Vector3[0];
    }

    private Vector3[] CompileWay()
    {
        Debug.Log("COMPILE");

        RaycastHit2D checkWayNor;

        List<Node> parents = new List<Node>();
        List<Vector3> way = new List<Vector3>();

        Node currentNode = map[map.Length - 1];
        Node parentNode = map[currentNode.parentNodeInd];

        while (!currentNode.Equals(map[0]))
        {
            while (!parentNode.Equals(null))
            {
                parents.Add(parentNode);
                currentNode = parentNode;
                parentNode = map[currentNode.parentNodeInd];
            }

            for(int i = 0; i < parents.Count; i++)
            {
                checkWayNor = Physics2D.Raycast(currentNode.position, parents[i].position - currentNode.position, (parents[i].position - currentNode.position).magnitude, obstacleLays);
                if (checkWayNor.collider == null)
                {
                    currentNode = parents[i];
                    way.Add(parents[i].position);
                    break;
                }
            }
        }

        way.Reverse();
        return way.ToArray();
    }

}
