using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DVelocityFromData : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DataFloat dataSpeed;
    [SerializeField] private DataFloat dataAcceleration;
    [SerializeField] private DataVector3 dataInput;
    [Header("References")]
    [SerializeField] private Data data;
    [SerializeField] private Rigidbody rigidbody;

    private float rotX;
    private CharacterController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<CharacterController>();
        dataSpeed = data.Float(dataSpeed);
        dataAcceleration = data.Float(dataAcceleration);
        dataInput = data.Vector3(dataInput);
    }

    // Update is called once per frame
    void Update()
    {

        rotX = Input.GetAxis("Mouse X") * 3;
        transform.Rotate(0, rotX, 0);
        player.Move(transform.TransformDirection((Vector3) dataInput * (float) dataSpeed) * Time.deltaTime);
       
        //rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, (Vector3)dataInput * (float)dataSpeed, (float)dataAcceleration);
    }
}
