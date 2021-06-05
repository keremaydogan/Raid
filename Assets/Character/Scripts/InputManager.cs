using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Assets.Character.Scripts;


namespace Assets.Character.Scripts
{
    public abstract class InputManager
    {
        public bool isManual;

        public string enemyTag;

        public string guiStr;

        //Input Manual
        public bool upInput, downInput, rightInput, leftInput, attackInput, crouchInput;

        //Input VI
        protected Vector3 destination;
        public Vector3 dest => destination;

        public abstract void InpMngAwake(string enemyTag, int[] sightLens, LayerMask[] sightLayMasks);

        public abstract void InpMngStart(PlayerControls playerControl);

        public abstract void InpMngUpdate();

        public abstract void InpMngFixedUpdate(Vector3 selfPos, Collider2D[] targets);

        protected abstract void InputDetector();
    }
}
