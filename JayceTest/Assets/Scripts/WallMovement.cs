using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public Vector3 ReStartPos;
    // Start is called before the first frame update
    void Start()
    {
        ReStartPos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime*2);
        if(transform.position.z >= 5.53)
        {
            transform.position = ReStartPos;
        }
    }
}
