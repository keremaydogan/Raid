using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Character.Scripts;


public enum InpMngEnum
{
    InpManual = 0,
    InpVI = 1,
}

public enum Tags
{
    Player = 0,
    Mob = 1,
}

public enum PlayerControls
{
    Player1Controls = 0,
    Player2Controls = 1
}

public class MovementBasics : MonoBehaviour
{
    Rigidbody2D rb;
    CircleCollider2D detectionCol;
    CapsuleCollider2D bodyCol;
    CapsuleCollider2D feetCol;

    public InpMngEnum inpMngSelection;

    GameManager GameMng => GameManager.Instance;

    public Tags enemyTag;
    Collider2D[] enemyCols = new Collider2D[0];

    public PlayerControls playerControls;

    InputManager inpMng;

    [Header("Enemy Sights")]
    public int absSightLen;
    public LayerMask absSightLayers;
    public int norSightLen;
    public LayerMask norSightLayers;
    public int memSightLen;
    [Space(1)]

    [Header("Walk")]
    public float walkSpeed;
    public float crouchEffector;
    [Space(1)]
    float crouchCoef = 1;
    float speedCoef;

    //InputIV
    LayerMask obstacleLays;
    int horCheckCnst;
    int verCheckCnst;
    float horOffsetCnst = 0;
    float verOffsetCnst = 0;
    RaycastHit2D wallHorCheck;
    RaycastHit2D wallVerCheck;

    float movementX;
    float movementY;
    Vector2 movementDir;

    //DELEGATES
    private delegate void voidDel();
    private voidDel Inputs;

    // GUI
    string guiStr;

    //Gizmos
    public bool showGizmos;

    private InputManager[] inpManagers = new InputManager[]
    {
        new InputManual(),
        new InputVI(),
    };

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        detectionCol = transform.Find("DetectionArea").GetComponent<CircleCollider2D>();
        bodyCol = transform.Find("Body").GetComponent<CapsuleCollider2D>();
        feetCol = transform.Find("Feet").GetComponent<CapsuleCollider2D>();

        obstacleLays = LayerMask.GetMask("Room");

        inpMng = inpManagers[(int)inpMngSelection];
        int[] sightLens = { absSightLen, norSightLen, memSightLen };
        LayerMask[] sightLayMasks = { absSightLayers, norSightLayers};

        if (inpMng.GetType() == typeof(InputVI)){
            detectionCol.radius = norSightLen;
        }

        inpMng.InpMngAwake(enemyTag.ToString(), sightLens, sightLayMasks);

        if((int)inpMngSelection == 0)
        {
            Inputs = new voidDel(InputsIM);
        }else if((int)inpMngSelection == 1)
        {
            Inputs = new voidDel(InputsIV);
        }

        Physics2D.IgnoreCollision(detectionCol, bodyCol);
        Physics2D.IgnoreCollision(detectionCol, feetCol);
    }

    private void Start()
    {
        inpMng.InpMngStart(playerControls);
    }

    private void Update()
    {
        inpMng.InpMngUpdate();

        Inputs();
    }

    private void FixedUpdate()
    {
        inpMng.InpMngFixedUpdate(transform.position, enemyCols);

        Walk();
        // GUI
        //eColsStr = "";
        //for(int i = 0; i < enemyCols.Length; i++)
        //{
        //    eColsStr = eColsStr + enemyCols[i] + "\n";
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == enemyTag.ToString())
        {
            Collider2D[] collTemp = new Collider2D[enemyCols.Length + 1];
            for (int i = 0; i < enemyCols.Length; i++){
                collTemp[i] = enemyCols[i];
            }
            collTemp[collTemp.Length - 1] = collision;
            enemyCols = collTemp;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == enemyTag.ToString())
        {
            Collider2D[] collTemp = new Collider2D[enemyCols.Length - 1];
            int cTI = 0; // collTemp Index
            for (int i = 0; i < enemyCols.Length; i++){
                if (collision != enemyCols[i]){
                    try
                    {
                        collTemp[cTI] = enemyCols[i];
                        cTI++;
                    }
                    catch
                    {
                        Debug.LogError(transform.gameObject.name);
                    }
                }
            }
            enemyCols = collTemp;
        }
    }

    private void InputsIM()
    {
        if (!(inpMng.upInput || inpMng.downInput) || (inpMng.upInput && inpMng.downInput)){
            movementY = 0;
        }else if (inpMng.upInput){
            movementY = 1;
        }else if (inpMng.downInput){
            movementY = -1;
        }

        if (!(inpMng.rightInput || inpMng.leftInput) || (inpMng.rightInput && inpMng.leftInput)){
            movementX = 0;
        }else if (inpMng.rightInput){
            movementX = 1;
        }else if (inpMng.leftInput){
            movementX = -1;
        }

        movementDir = new Vector2(movementX, movementY).normalized;

        if (inpMng.crouchInput && crouchCoef == 1){
            crouchCoef = crouchEffector;
        }else if (inpMng.crouchInput){
            crouchCoef = 1;
        }
    }

    private void InputsIV()
    {
        horCheckCnst = 1;
        verCheckCnst = 1;
        if ((inpMng.dest.x - transform.position.x) < 0) { horCheckCnst = -1; } else if ((inpMng.dest.x - transform.position.x) == 0) { horCheckCnst = 0; }
        if ((inpMng.dest.y - transform.position.y) < 0) { verCheckCnst = -1; } else if ((inpMng.dest.y - transform.position.y) == 0) { verCheckCnst = 0; }
        wallHorCheck = Physics2D.Raycast(transform.position + (horOffsetCnst * Vector3.up), horCheckCnst * Vector3.right, feetCol.size.x + 0.2f, obstacleLays);
        wallVerCheck = Physics2D.Raycast(transform.position + (verOffsetCnst * Vector3.right), verCheckCnst * Vector3.up, feetCol.size.x + 0.2f, obstacleLays);

        Debug.DrawRay(transform.position + (horOffsetCnst * Vector3.up), horCheckCnst * Vector3.right, Color.red);
        Debug.DrawRay(transform.position + (verOffsetCnst * Vector3.right), verCheckCnst * Vector3.up, Color.red);

        if((inpMng.dest - transform.position).magnitude < 0.4f)
        {
            movementDir = Vector3.zero;
        }
        else
        {

            if (wallHorCheck.collider == null && wallVerCheck.collider == null)
            {
                horOffsetCnst = 0;
                verOffsetCnst = 0;

                movementDir = (inpMng.dest - transform.position).normalized;
            }
            else
            {
                if (wallHorCheck.collider != null)
                {
                    movementDir = verCheckCnst * Vector3.up;
                    horOffsetCnst = -verCheckCnst * feetCol.size.y;
                }
                if (wallVerCheck.collider != null)
                {
                    movementDir = horCheckCnst * Vector3.right;
                    verOffsetCnst = -horCheckCnst * feetCol.size.x;
                }
            }

        }

    }

    private void Walk()
    {
        speedCoef = walkSpeed * crouchCoef;
        rb.velocity = movementDir * speedCoef;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 150, 500), guiStr);
        GUI.Label(new Rect(25, 25, 150, 500), inpMng.guiStr);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, absSightLen);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, norSightLen);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, memSightLen);
        }
    }

}
