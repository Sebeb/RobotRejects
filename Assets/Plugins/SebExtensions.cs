using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class WUtils
{
    public static Vector2 screenSize { get { return new Vector2(Screen.width, Screen.height); } }
    #region Vectors

    public static Vector2 xz(this Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }

    public static Vector3 y2z(this Vector2 input, float newY)
    {
        return new Vector3(input.x, newY, input.y);
    }

    public static Vector2 ComplexMult(this Vector2 aVec, Vector2 aOther)
    {
        return new Vector2(aVec.x * aOther.x - aVec.y * aOther.y,
            aVec.x * aOther.y + aVec.y * aOther.x);
    }
    public static Vector2 Rotation(float aDegree)
    {
        var a = aDegree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }
    public static Vector2 Rotate(this Vector2 aVec, float aDegree) => ComplexMult(aVec, Rotation(aDegree));

    public static Vector3 SetX(this Vector3 input, float value) => new Vector3(value, input.y, input.z);
    public static Vector3 SetY(this Vector3 input, float value) => new Vector3(input.x, value, input.z);
    public static Vector3 SetZ(this Vector3 input, float value) => new Vector3(input.x, input.y, value);

    public static Vector2 SetX(this Vector2 input, float value) => new Vector2(value, input.y);
    public static Vector2 SetY(this Vector2 input, float value) => new Vector2(input.x, value);

    public static bool toBool(this float f) => f >= 1;
    public static bool toBool(this int i) => i >= 1;

    // public static Vector3 operator %(Vector3 a, Vector3 b) => new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);

    public static float WrapAngle(this float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle;
    }

    public static float UnwrapAngle(this float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    #endregion

    #region Transform
    public static void ResetLocal(this Transform input)
    {
        input.localPosition = Vector3.zero;
        input.localRotation = Quaternion.identity;
        input.localScale = Vector3.one;
    }
    public static void Reset(this Transform input)
    {
        input.position = Vector3.zero;
        input.rotation = Quaternion.identity;
        input.localScale = Vector3.one;
    }

    public static void CopyFrom(this Transform to, Transform from, bool includeParent = true)
    {
        if (includeParent)to.parent = from.parent;
        to.position = from.position;
        to.localScale = from.localScale;
        to.rotation = from.rotation;
    }

    #endregion

    #region String
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/791963c8-9e20-4e9e-b184-f0e592b943b0/split-a-camel-case-string?forum=csharpgeneral
    public static string NormalizeCamel(this string stringtosplit)
    {
        string words = string.Empty;
        if (!string.IsNullOrEmpty(stringtosplit))
        {
            foreach (char ch in stringtosplit)
            {
                if (char.IsLower(ch))
                {
                    words += ch.ToString();
                }
                else
                {
                    words += " " + ch.ToString();
                }

            }
            return words;
        }
        else
            return string.Empty;
    }
    #endregion

    #region Colour
    public static Color SetA(this Color input, float a)
    {
        input.a = a;
        return input;
    }
    #endregion

    #region Collections
    //List
    public static T LastElement<T>(this List<T> l)
    {
        return l == null || l.Count == 0 ? default(T) : l[l.Count - 1];
    }
    public static T FirstElement<T>(this List<T> l)
    {
        return l == null || l.Count == 0 ? default(T) : l[0];
    }
    #endregion

    #region GameObject
    public static GameObject AddChild(this GameObject input, string name, bool resetLocalScale = true)
    {
        var ret = new GameObject(name);
        ret.transform.parent = input.transform;
        if (resetLocalScale)ret.transform.ResetLocal();
        else
        {
            ret.transform.localPosition = Vector3.zero;
            ret.transform.localRotation = Quaternion.identity;
        }

        return ret;
    }

    public static void SetLayersRecursively(this GameObject go, int layer)
    {
        go.layer = layer;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            child.layer = layer;
            SetLayersRecursively(child, layer);
        }
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    #endregion

    //Math
    public static int idx(int x, int y, int z, int sX, int sY) => x + y * sX + z * sX * sY;
    public static int wrap(this int x, int m) => (x % m + m) % m;
}

// https://www.habrador.com/tutorials/math/5-line-line-intersection/
namespace LinearAlgebra
{
    //How to figure out if two lines are intersecting
    public static class Lines
    {
        //Line segment-line segment intersection in 2d space by using the dot product
        //p1 and p2 belongs to line 1, and p3 and p4 belongs to line 2 
        public static bool Do2DLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            return IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2);
        }

        //Are the points on different sides of a line?
        public static bool IsPointsOnDifferentSides(Vector2 l1, Vector2 l2, Vector2 p1, Vector2 p2)
        {
            //The direction of the line
            Vector2 lineDir = l2 - l1;

            //The normal to a line is just flipping x and y and making y negative
            Vector2 lineNormal = new Vector2(lineDir.y, -lineDir.x);

            //Now we need to take the dot product between the normal and the points on the other line
            float dot1 = Vector2.Dot(lineNormal, p1 - l1);
            float dot2 = Vector2.Dot(lineNormal, p2 - l1);

            //If you multiply them and get a negative value then p3 and p4 are on different sides of the line
            return dot1 * dot2 < 0f;
        }

        //Is the point on the left side of a line?
        public static bool IsPointOnLeftSide(this Vector2 p, Vector2 l1, Vector2 l2)
        {
            Vector2 lineDir = l2 - l1;

            Vector2 lineNormal = new Vector2(lineDir.y, -lineDir.x);

            float dot1 = Vector2.Dot(lineNormal, p - l1);

            return dot1 < 0;
        }

        public static bool IsInTriangle(this Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
            float t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

            return A < 0
                ? (s <= 0 && s + t >= A)
                : (s >= 0 && s + t <= A);
        }
        public static bool IsInTriangle(this Vector2 p, Vector2[] points)
        {
            return IsInTriangle(p, points[0], points[1], points[2]);
        }
        public static bool IsInTriangle(this Vector2 p, Transform[] points)
        {
            if (points?.Length < 3)return false;

            return IsInTriangle(p, points[0].position.xz(), points[1].position.xz(), points[2].position.xz());
        }
    }
}