using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyCarPosition : MonoBehaviour
{

    public Transform m_carPos;


    void Update()
    {
        this.transform.position = m_carPos.position;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 5f, this.transform.position.z);
        //this.transform.position = new Vector3(this.transform.position.x + 0.5f, this.transform.position.y + 10f, this.transform.position.z + 15f);
        this.transform.rotation = m_carPos.rotation;
    }
}
