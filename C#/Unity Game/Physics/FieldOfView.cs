using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadiusMin;
    [Range(0, 360)]
    public float viewAngleMin;

    public float viewRadiusMax;
    [Range(0, 360)]
    public float viewAngleMax;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargetsMin = new List<Transform>();
    [HideInInspector]
    public List<Transform> visibleTargetsMax = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .1f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargetsMin.Clear();
        visibleTargetsMax.Clear();
        Collider2D[] targetsInViewRadiusMin = Physics2D.OverlapCircleAll(transform.position, viewRadiusMin, targetMask);

        for (int i = 0; i < targetsInViewRadiusMin.Length; i++)
        {
            Transform target = targetsInViewRadiusMin[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(-transform.up, dirToTarget) < viewAngleMin / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargetsMin.Add(target);
                }
            }
        }

        Collider2D[] targetsInViewRadiusMax = Physics2D.OverlapCircleAll(transform.position, viewRadiusMax, targetMask);

        for (int i = 0; i < targetsInViewRadiusMax.Length; i++)
        {
            Transform target = targetsInViewRadiusMax[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(-transform.up, dirToTarget) < viewAngleMax / 2)
            {
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargetsMax.Add(target);
                }
            }
        }
    }

    public void OnEnable()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    public void OnDisable()
    {
        visibleTargetsMin.Clear();
        visibleTargetsMax.Clear();
    }

    public bool FoundTargetMin()
    {
        if(visibleTargetsMin.Count > 0)
        {
            return true;
        }
        return false;
    }

    public bool FoundTargetMax()
    {
        if (visibleTargetsMax.Count > 0)
        {
            return true;
        }
        return false;
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), -Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public Vector2 TargetMaxPosition()
    {
        if (visibleTargetsMax.Count > 0)
        {
            return new Vector2(visibleTargetsMax[0].transform.position.x, visibleTargetsMax[0].transform.position.y);
        }
        return new Vector2(0, 0);
    }

    public string TargetMaxTagName()
    {
        if (visibleTargetsMax.Count > 0)
        {
            return visibleTargetsMax[0].tag;
        }
        return "Untagged";
    }

    public string TargetMinTagName()
    {
        if (visibleTargetsMin.Count > 0)
        {
            return visibleTargetsMin[0].tag;
        }
        return "Untagged";
    }
}
