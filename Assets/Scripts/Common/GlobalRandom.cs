using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalRandom
{
    private static System.Random rnd = new System.Random();
    public static int GetRandom()
    {
        return rnd.Next();
    }

    public static int GetRandom(int from, int to)
    {
        return rnd.Next(from, to);
    }

}
