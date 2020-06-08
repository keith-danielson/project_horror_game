using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 180)]
    public float viewAngle;

    public LayerMask obstacleMask;
    public Transform seekedObject;
    public bool canSeeTarget = false;
    private Vector3 pathDirection;
    private Vector3 lastPosition;



    void Update()

    {
        
       FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        Vector3 dirToTarget = (seekedObject.position - transform.position).normalized;
        if (Vector3.Angle(transform.right, dirToTarget) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(transform.position, seekedObject.position);

            if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                canSeeTarget = true;
            }
            else
            {
                canSeeTarget = false;
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {

        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        Vector3 temp = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), (Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)), 0);

  
        return temp;
    }
}

