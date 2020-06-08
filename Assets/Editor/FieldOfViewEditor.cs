
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (EnemyFieldOfView))]
public class FieldOfViewEditor : Editor
{
   void OnSceneGUI()
    {
        EnemyFieldOfView fov = (EnemyFieldOfView)target;
        Handles.color = Color.white;

        Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.right, 360, fov.viewRadius);


        Vector3 viewAngleA = fov.DirFromAngle(- fov.viewAngle + 180 / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle + 180/ 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        if (fov.canSeeTarget)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.seekedObject.position);
        }
        
    }
}
