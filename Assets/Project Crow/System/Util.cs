using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{ 
    public static float distance(Vector3 a, Vector3 b)
    {
        float x_distance = a.x - b.x;
        x_distance *= x_distance;

        float y_distance = a.y - b.y;
        y_distance *= y_distance;

        float z_distance = a.z - b.z;
        z_distance *= z_distance;

        return Mathf.Sqrt(x_distance + y_distance + z_distance);
    }
}
