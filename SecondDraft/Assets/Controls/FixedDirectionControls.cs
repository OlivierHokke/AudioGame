using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class FixedDirectionControls : BaseControls
{
    public bool flipHorizontal = true;
    public bool flipVertical = false;
    public float ForwardRotationAngle = 0;
    public Quaternion ForwardRotation { get { return Quaternion.AngleAxis(ForwardRotationAngle, up); } }
    public Vector3 up = new Vector3(0, 1, 0);
    public float turnSpeed = 1f;
    public float moveSpeed = 0.1f;

    private Vector3 lastRotDir;

    private Vector3[] PlaneDirections
    {
        get
        {
            var directions = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(1, 0, 0) };
            return directions.Select(v => up.cross(v).Positivize().normalized).Where(v => v.magnitude != 0).Distinct().ToArray();
        }
    }

    //private class SameDirectionComparer : IEqualityComparer<Vector3>
    //{

    //    public bool Equals(Vector3 x, Vector3 y)
    //    {
    //        return x.cross(y).magnitude == 0;
    //    }

    //    public int GetHashCode(Vector3 obj)
    //    {
    //        var res = obj.normalized;
    //        if (res.maxDirection() < 0)
    //            return res.flip().GetHashCode();
    //        return res.GetHashCode();
    //    }
    //}

    public override void OnEnable() { }

    public override Vector3 GetMove()
    {
        return GetMoveDir() * moveSpeed;
    }

    private Vector3 GetMoveDir()
    {

        Vector3 forward = CameraManager.GetCameraForwardVector();
        return forward * Time.deltaTime * moveSpeed * Input.GetAxis("Vertical");
        //var vert = PlaneDirections[0]; if (flipVertical) vert = vert.flip();
        //var horz = PlaneDirections[1]; if (flipHorizontal) horz = horz.flip();
        //var moveDir = vert * Input.GetAxis("Vertical") + horz * Input.GetAxis("Horizontal");
        //return moveDir;
    }

    private Vector3 GetLastRotateDir()
    {
        var vert = PlaneDirections[0]; if (flipVertical) vert = vert.flip();
        var horz = PlaneDirections[1]; if (flipHorizontal) horz = horz.flip();
        var moveDir = ForwardRotation * (vert * Input.GetAxis("RightV") + horz * Input.GetAxis("RightH"));
        moveDir.Normalize();
        if (moveDir.magnitude > 0)
            lastRotDir = moveDir;
        return lastRotDir;
    }
    
    public override Quaternion GetRotation()
    {
        var desiredDir = GetLastRotateDir();
        var currentDir = CameraManager.GetCameraForwardVector();
        var a = currentDir.cross(desiredDir);
        if (a.magnitude == 0)
            return Quaternion.identity;

        var q = new Quaternion(a.x, a.y, a.z, 
            Mathf.Sqrt((Mathf.Pow(currentDir.magnitude, 2) * Mathf.Pow(desiredDir.magnitude, 2)) + currentDir.dot(desiredDir)));
        return q;
        return Quaternion.RotateTowards(Quaternion.identity, q, turnSpeed * Time.deltaTime);
    }

    public override void OnDisable() { }
}
