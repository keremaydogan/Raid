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

    

    public Vector3[] ShortestPath(Vector3 startPoint, Vector3 endPoint, Vector3[] pointMap)
    {
        SetMap(startPoint, endPoint, pointMap);

        RoughWay(endPoint);

        return CompileWay();
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
        }
    }


    private void RoughWay(Vector3 endPoint)
    {
        RaycastHit2D checkWayNor;

        int currentNodeInd = 0;
        int nextNodeInd = -1;
        HashSet<int> walkableNodeInds = new HashSet<int>();

        float GTemp;
        float FMin;

        int repeat = 0;

        while (currentNodeInd != map.Length - 1)
        {
            Debug.DrawRay(map[currentNodeInd].position, Vector3.up, Color.magenta);

            map[currentNodeInd].nodeState = NodeState.Closed;
            walkableNodeInds.Remove(currentNodeInd);

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

                    map[i].H = (endPoint - map[i].position).magnitude;
                    walkableNodeInds.Add(i);

                }
            }

            if (walkableNodeInds.Count == 0) { break; }

            FMin = float.MaxValue;

            foreach (int ind in walkableNodeInds)
            {
                if (map[ind].Equals(map[map.Length - 1]))
                {
                    nextNodeInd = ind;
                    break;
                }
                if (FMin > map[ind].F || (FMin > map[ind].F && map[ind].H < map[nextNodeInd].H))
                {
                    FMin = map[ind].F;
                    nextNodeInd = ind;
                }
            }

            currentNodeInd = nextNodeInd;


            repeat++;
            if (repeat == map.Length) { break; }
        }
    }


    private Vector3[] CompileWay()
    {
        RaycastHit2D checkWayNor;

        List<Node> parents = new List<Node>();
        List<Vector3> way = new List<Vector3>();

        Node currentNode = map[map.Length - 1];
        Node parentNode;

        int repeat = 0;

        while (currentNode.parentNodeInd != -1)
        {
            parentNode = map[currentNode.parentNodeInd];
            parents.Add(parentNode);
            currentNode = parentNode;


            repeat++;
            if (repeat == map.Length) { break; }
        }

        currentNode = map[map.Length - 1];
        while (!currentNode.Equals(map[0]))
        {
            way.Add(currentNode.position);
            for (int i = parents.Count - 1; i > -1; i--)
            {
                checkWayNor = Physics2D.Raycast(currentNode.position, parents[i].position - currentNode.position, (parents[i].position - currentNode.position).magnitude, obstacleLays);
                if (checkWayNor.collider == null)
                {
                    currentNode = parents[i];
                    break;
                }
            }


            repeat++;
            if (repeat == map.Length) { break; }

        }

        way.Reverse();

        return way.ToArray();
    }
}
