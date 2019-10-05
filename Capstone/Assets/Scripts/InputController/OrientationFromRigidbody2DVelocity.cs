using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationFromRigidbody2DVelocity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody;

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.sqrMagnitude > 0)
            transform.up = rigidbody.velocity;
    }
}
