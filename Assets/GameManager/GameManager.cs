using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public KeyCode p1UpKey;
    public KeyCode p1DownKey;
    public KeyCode p1RightKey;
    public KeyCode p1LeftKey;
    public KeyCode p1AttackKey;
    public KeyCode p1CrouchKey;
    public KeyCode[] p1Keys;

    public KeyCode p2UpKey;
    public KeyCode p2DownKey;
    public KeyCode p2RightKey;
    public KeyCode p2LeftKey;
    public KeyCode p2AttackKey;
    public KeyCode p2CrouchKey;
    public KeyCode[] p2Keys;


    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);

        p1Keys = new KeyCode[] { p1UpKey, p1DownKey, p1RightKey, p1LeftKey, p1AttackKey, p1CrouchKey };
        p2Keys = new KeyCode[] { p2UpKey, p2DownKey, p2RightKey, p2LeftKey, p2AttackKey, p2CrouchKey };
    }


}
