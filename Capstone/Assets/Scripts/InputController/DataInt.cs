using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataInt : DataNode
{
    [SerializeField] private int value;

    public static explicit operator int(DataInt data)
    {
        return data.value;
    }
}
