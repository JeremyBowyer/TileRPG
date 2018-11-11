using System;
using UnityEngine;

public class MathCurves
{

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }

    public static Vector3 Bezier(Vector3 start, Vector3 end, Vector3 cp1, Vector3 cp2, float t)
    {

        Vector3 mid = Mathf.Pow(1 - t, 3) * start + 3 * Mathf.Pow(1 - t, 2) * t * cp1 + 3 * (1 - t) * Mathf.Pow(t, 2) * cp2 + Mathf.Pow(t, 3) * end;

        return mid;
    }

    public static Vector3 Bezier(Vector3 start, Vector3 end, Vector3 cp1, float t)
    {

        Vector3 mid = Mathf.Pow(1 - t, 2) * start + 2 * (1 - t) * t * cp1 + Mathf.Pow(t, 2) * end;

        return mid;
    }

}