using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataVector3 : DataNode
{
    [SerializeField] private Vector3 value;
    public Vector3 Value { get { return value; } set { this.value = value; } }

    public static explicit operator Vector3(DataVector3 data)
    {
        return data.value;
    }
}
