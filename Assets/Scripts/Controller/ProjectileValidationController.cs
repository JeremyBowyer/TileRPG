using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileValidationController
{

    public BattleController bc;
    public float sphereRadius = 0.1f;

    public ProjectileValidationController(BattleController _bc)
    {
        bc = _bc;
    }

    public bool ValidateProjectile(Vector3[] path, GameObject target)
    {
        List<Vector3> validPath = new List<Vector3>();
        int layerMask = (1 << LayerMask.NameToLayer("Character"));
        layerMask = ~layerMask;


        foreach (Vector3 point in path)
        {
            Collider[] cols = Physics.OverlapSphere(point, sphereRadius, layerMask);
            if(cols.Length == 0)
            {
                validPath.Add(point);
                continue;
            }

            foreach(Collider col in cols)
            {
                if (col.gameObject != target)
                {
                    DisplayTrajectory(validPath, Color.gray);
                    return false;
                }
            }
        }

        DisplayTrajectory(validPath, CustomColors.Hostile);
        return true;
    }

    public void DisplayTrajectory(Vector3[] trajectory, Color color)
    {
        bc.lineRenderer.material.color = color;
        bc.lineRenderer.positionCount = trajectory.Length;
        bc.lineRenderer.SetPositions(trajectory);
    }

    public void DisplayTrajectory(List<Vector3> trajectory, Color color)
    {
        Vector3[] trajectoryArray = trajectory.ToArray();
        bc.lineRenderer.material.color = color;
        bc.lineRenderer.positionCount = trajectoryArray.Length;
        bc.lineRenderer.SetPositions(trajectoryArray);
    }

}
