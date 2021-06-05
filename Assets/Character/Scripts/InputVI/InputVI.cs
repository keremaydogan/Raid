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
    Vector3[] map = new Vector3[0];
    bool destIsReachable = false;

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
            destination = selfPos;
            SetMap();
        }
        else
        {
            destination = selfPos;
        }
    }

    private void SetMap()
    {
        RaycastHit2D checkWayNor = Physics2D.Raycast(selfPos, targetPos - selfPos, (targetPos - selfPos).magnitude, norSightLays);

        map = new Vector3[0];
        Vector3[] path = new Vector3[0];

        while (!IsEnemyCol(checkWayNor.collider) && checkWayNor.collider.gameObject.transform.parent != null)
        {
            path = checkWayNor.collider.gameObject.transform.parent.Find("Path").GetComponent<VertexPath>().GetPath();

            if(path == null)
            {
                break;
            }

            Transform pathParent = checkWayNor.collider.gameObject.transform.parent;
            AddToMap(path);

            int closestPointInd = -1;
            float minDist = float.MaxValue;

            for (int i = 0; i < path.Length; i++)
            {
                Debug.DrawRay(path[i], Vector3.up, Color.green);

                checkWayNor = Physics2D.Raycast(path[i], targetPos - path[i], (targetPos - path[i]).magnitude, norSightLays);

                if (IsEnemyCol(checkWayNor.collider) && (checkWayNor.collider.transform.position - path[i]).magnitude < minDist)
                {
                    minDist = (checkWayNor.collider.transform.position - path[i]).magnitude;
                    closestPointInd = i;
                }
            }

            if(closestPointInd == -1)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    RaycastHit2D checkWayDir = Physics2D.Raycast(path[i], targetPos - path[i], (targetPos - path[i]).magnitude, absSightLays);

                    if (IsEnemyCol(checkWayDir.collider) && (checkWayDir.collider.transform.position - path[i]).magnitude < minDist)
                    {
                        checkWayNor = Physics2D.Raycast(path[i], targetPos - path[i], (targetPos - path[i]).magnitude, norSightLays);
                        if (checkWayNor.collider.gameObject.transform.parent == pathParent)
                        {
                            continue;
                        }

                        minDist = (checkWayDir.collider.transform.position - path[i]).magnitude;
                        closestPointInd = i;
                    }
                }
            }
            
            if(closestPointInd == -1)
            {
                Debug.Log("There is no way");
                break;
            }

            checkWayNor = Physics2D.Raycast(path[closestPointInd], targetPos - path[closestPointInd], (targetPos - path[closestPointInd]).magnitude, norSightLays);

            if (targetEnemy.GetCol().Equals(checkWayNor.collider))
            {
                Debug.DrawRay(path[closestPointInd], Vector3.up, Color.red);
                Debug.DrawRay(path[closestPointInd], checkWayNor.collider.transform.position - path[closestPointInd]);
            }

        }
    }

    private bool IsEnemyCol(Collider2D col)
    {
        return targetEnemy.GetCol().Equals(col);
    }


    private void AddToMap(Vector3[] path)
    {
        Vector3[] newMap = new Vector3[map.Length + path.Length];
        for(int i = 0; i < map.Length; i++)
        {
            newMap[i] = map[i];
        }
        for (int i = 0; i < path.Length; i++)
        {
            newMap[map.Length + i] = path[i];
        }
        map = newMap;
    }
}

