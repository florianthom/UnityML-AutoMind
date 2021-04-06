using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addVelocity : MonoBehaviour
{

    [SerializeField]
    Vector3 veclocityAddition;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    // called 50 times per sec
    void FixedUpdate()
    {
        // x,y,z
        GetComponent<Rigidbody>().velocity += veclocityAddition;
    }
}
