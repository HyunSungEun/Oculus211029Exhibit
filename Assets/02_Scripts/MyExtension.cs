using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtension 
{
    public static string DebugStr(this Vector3 v)
    {
        return string.Format("({0:N2},{1:N2},{2:N2})", v.x, v.y, v.z);
    }
}
