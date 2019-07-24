using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPFloor : MonoBehaviour
{
    public int x;
    public int y;
    public bool isDoorway = false;
    public bool isCorner = false;
    public List<GameObject> walls = new List<GameObject>();
    public GameObject prop;
}
