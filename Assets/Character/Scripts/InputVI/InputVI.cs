using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Character.Scripts;
public enum States
{
    Sleep,
    Guard,
    Offensive,
}

public class InputVI : InputManager
{
    private class Enemy
    {
        Collider2D col;

        public Enemy(Collider2D col)
        {
            this.col = col;
        }

        public static bool HashSetContains(ref HashSet<Enemy> enemiesSet, Enemy enemy) 
        {
            foreach(Enemy e in enemiesSet)
            {
                if(e.GetCol().GetHashCode() == enemy.GetCol().GetHashCode())
                {
                    return true;
                }
            }
            return false;
        }
        
        public Collider2D GetCol() { return col; }
    }

    AStarPathfinder aStar = new AStarPathfinder();

    HashSet<Enemy> enemiesInSight = new HashSet<Enemy>{};

    Enemy targetEnemy = null;

    Vector3 selfPos;
    Vector3 targetPos;

    //ENEMY DETECTION
    RaycastHit2D absoluteSight;
    int absSightLen;
    LayerMask absSightLays;
    RaycastHit2D normalSight;
    int norSightLen;
    LayerMask norSightLays;
    RaycastHit2D memorySight;
    int memSightLen;

    //SET DESTINATION
    List<Vector3> map;
    Vector3[] path = new Vector3[0];
    LayerMask pathMask;

    //Vector3[] map = new Vector3[0];

    public InputVI()
    {
        isManual = false;
    }
    
    public override void InpMngAwake(string enemyTag, int[] sightLens, LayerMask[] sightLayMasks)
    {

        this.enemyTag = enemyTag;

        absSightLen = sightLens[0];
        norSightLen = sightLens[1];
        memSightLen = sightLens[2];
        absSightLays = sightLayMasks[0];
        norSightLays = sightLayMasks[1];

        aStar.AStarAwake();

        pathMask = LayerMask.GetMask("Path");
    }

    public override void InpMngStart(PlayerControls playerControl)
    {
    }

    public override void InpMngUpdate()
    {
    }

    public override void InpMngFixedUpdate(Vector3 selfPos, Collider2D[] targets)
    {
        this.selfPos = selfPos;

        if(targetEnemy != null)
        {
            targetPos = targetEnemy.GetCol().transform.position;
        }

        EnemyDetection(selfPos, targets);

        SetDestination();

    }

    protected override void InputDetector()
    {
    }

    private void EnemyDetection(Vector3 selfPos, Collider2D[] targets)
    {
        if (targets.Length > 0)
        {
            AddEnemiesInSight(targets);
        }
        if (enemiesInSight.Count > 0)
        {
            RemoveEnemiesInSight();
            ChooseEnemy();
        }
    }

    private void AddEnemiesInSight(Collider2D[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Debug.DrawRay(selfPos, targets[i].transform.position - selfPos, Color.blue);

            bool inAbsSight = false;
            bool inNorSight = false;
            bool inSight = false;

            absoluteSight = Physics2D.Raycast(selfPos, targets[i].transform.position - selfPos, absSightLen, absSightLays);
            inAbsSight = absoluteSight.collider != null && absoluteSight.collider.tag == enemyTag;

            if (!inAbsSight)
            {
                normalSight = Physics2D.Raycast(selfPos, targets[i].transform.position - selfPos, norSightLen, norSightLays);
                inNorSight = normalSight.collider != null && normalSight.collider.tag == enemyTag;
            }

            inSight = inAbsSight || inNorSight;

            Enemy e = new Enemy(targets[i]);
            if (!Enemy.HashSetContains(ref enemiesInSight, e) && inSight)
            {
                enemiesInSight.Add(new Enemy(targets[i]));
            }
        }
    }

    private void RemoveEnemiesInSight()
    {
        foreach(Enemy e in enemiesInSight)
        {
            bool inMemSight = false;

            memorySight = Physics2D.Raycast(selfPos, e.GetCol().transform.position - selfPos, memSightLen, absSightLays);
            inMemSight = memorySight.collider != null && memorySight.collider.tag == enemyTag;

            if (!inMemSight)
            {
                enemiesInSight.Remove(e);
                break;
            }
            Debug.DrawRay(selfPos, e.GetCol().transform.position - selfPos, Color.green);
        }
    }

    private void ChooseEnemy()
    {
        Enemy enemy = null;
        float minDistance = float.MaxValue;
        foreach (Enemy e in enemiesInSight)
        {
            float distance = (e.GetCol().transform.position - selfPos).magnitude;
            if (distance < minDistance)
            {
                enemy = e;
                minDistance = distance;
            }
        }
        targetEnemy = enemy;
        if(targetEnemy != null)
        {
            Debug.DrawRay(selfPos, enemy.GetCol().transform.position - selfPos, Color.red);
        }
    }

    private void SetDestination()
    {
        if(targetEnemy != null)
        {
            RaycastHit2D checkWay = Physics2D.Raycast(selfPos, targetPos - selfPos, (targetPos - selfPos).magnitude, norSightLays);
            if (checkWay.collider.Equals(targetEnemy.GetCol()))
            {
                destination = selfPos;
            }
            else
            {
                SetMap();
                destination = selfPos;
            }
        }
        else
        {
            destination = selfPos;
        }
    }


    private void SetMap()
    {
        Collider2D[] pathCols;
        VertexPath[] pathObjects;
        map = new List<Vector3>();
        Vector3[] pathPart = new Vector3[0];
        Vector3[] path = new Vector3[0];

        pathCols = Physics2D.OverlapCircleAll(selfPos + ((targetPos - selfPos) / 2), Mathf.Abs((selfPos - targetPos).magnitude/2) + 10, pathMask);

        pathObjects = new VertexPath[pathCols.Length];

        for (int i = 0; i < pathObjects.Length; i++)
        {
            pathObjects[i] = pathCols[i].transform.parent.Find("Path").GetComponent<VertexPath>();
        }

        for(int i = 0; i < pathObjects.Length; i++)
        {
            pathPart = pathObjects[i].GetPath();
            for(int j = 0; j < pathPart.Length; j++)
            {
                map.Add(pathPart[j]);
                Debug.DrawRay(pathPart[j], Vector3.up, Color.green);
            }
        }

        path = aStar.ShortestPath(selfPos, targetPos, map.ToArray());


        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawRay(path[i], Vector3.up, Color.red);
            Debug.DrawLine(path[i], path[i + 1], Color.white);
        }
        Debug.DrawRay(path[path.Length - 1], Vector3.up, Color.red);
        Debug.DrawLine(selfPos, path[0], Color.white);

    }


    private bool IsEnemyCol(Collider2D col)
    {
        return targetEnemy.GetCol().Equals(col);
    }

}

