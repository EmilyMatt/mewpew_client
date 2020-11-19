using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSelect : MonoBehaviour
{
    // Start is called before the first frame update
    Transform mesh;
    void Start()
    {
        mesh = transform.Find("Mesh");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mesh.Rotate(Vector3.up, 1);
    }
}
