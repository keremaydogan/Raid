using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VertexPath))]
public class VertexPathEditor : Editor
{
    VertexPath vp;
    Vector3[] points;
    Transform handleTransform;
    Quaternion handleRotation;
    LayerMask roomLayerMask;

    private void OnEnable()
    {
        roomLayerMask = LayerMask.GetMask("Room");
    }
    void OnSceneGUI()
    {
        Transform transform = ((VertexPath)target).transform;
        vp = target as VertexPath;
        handleTransform = vp.transform;
        handleRotation = handleTransform.rotation;
        points = new Vector3[vp.points.Length];
        
        for(int i = 0; i < points.Length; i++)
        {
            points[i] = handleTransform.TransformPoint(vp.points[i]);
        }

        Handles.color = Color.green;
        for(int i = 0; i < points.Length - 1; i++)
        {
            for(int j = i; j < points.Length; j++)
            {
                if (!Physics2D.Raycast(points[i], points[j] - points[i], (points[j] - points[i]).magnitude, roomLayerMask))
                {
                    Handles.DrawLine(points[i], points[j]);
                }
            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            Handles.SphereHandleCap(
            0,
            points[i],
            transform.rotation,
            0.5f,
            EventType.Repaint
            );
        }

        for(int i = 0; i < points.Length; i++)
        {
            HandlePosition(i);
        }

        
        
    }


    void HandlePosition(int ind)
    {
        EditorGUI.BeginChangeCheck();
        points[ind] = Handles.DoPositionHandle(points[ind], handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            vp.points[ind] = handleTransform.InverseTransformPoint(points[ind]);
            vp.points[ind].x = Mathf.RoundToInt(vp.points[ind].x);
            vp.points[ind].y = Mathf.RoundToInt(vp.points[ind].y);
            vp.points[ind].z = 0;
        }
    }

}
