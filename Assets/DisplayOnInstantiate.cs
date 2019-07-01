using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayOnInstantiate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float xMod = GetComponent<BoxCollider>().bounds.size.x;
        float zMod = GetComponent<BoxCollider>().bounds.size.z;

        transform.localScale = new Vector3(transform.localScale.x / xMod, 1, transform.localScale.z / zMod);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
