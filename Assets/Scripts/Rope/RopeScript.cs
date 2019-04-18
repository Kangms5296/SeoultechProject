using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HingeJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
    }
}
