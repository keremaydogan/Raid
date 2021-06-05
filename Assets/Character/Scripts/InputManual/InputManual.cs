using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Character.Scripts;



public class InputManual : InputManager
{
    private KeyCode upKey, downKey, rightKey, leftKey, attackKey, crouchKey;

    public InputManual()
    {
        isManual = true;
    }

    public override void InpMngAwake(string enemyTag, int[] sightLens, LayerMask[] sightLayMasks)
    {
    }

    public override void InpMngStart(PlayerControls playerControl)
    {
        SetKeys(playerControl);
    }

    public override void InpMngUpdate()
    {
        InputDetector();
    }

    public override void InpMngFixedUpdate(Vector3 selfPos, Collider2D[] targets)
    {

    }

    public void SetKeys(PlayerControls pc)
    {
        KeyCode[] keys = null;
        if((int)pc == 0)
        {
            keys = GameManager.Instance.p1Keys;
        }else if((int)pc == 1)
        {
            keys = GameManager.Instance.p2Keys;
        }
        upKey = keys[0];
        downKey = keys[1];
        rightKey = keys[2];
        leftKey = keys[3];
        attackKey = keys[4];
        crouchKey = keys[5];
    }

    protected override void InputDetector()
    {
        if (Input.GetKey(upKey)){
            upInput = true;
        }else{
            upInput = false;
        }

        if (Input.GetKey(downKey)){
            downInput = true;
        }
        else
        {
            downInput = false;
        }

        if (Input.GetKey(rightKey)){
            rightInput = true;
        }else{
            rightInput = false;
        }

        if (Input.GetKey(leftKey)){
            leftInput = true;
        }else{
            leftInput = false;
        }

        if (Input.GetKeyDown(attackKey)){
            attackInput = true;
        }else{
            attackInput = false;
        }

        if (Input.GetKeyDown(crouchKey)){
            crouchInput = true;
        }else{
            crouchInput = false;
        }
    }
}
