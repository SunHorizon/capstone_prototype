using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DataInputKeyStick : DataInput
{
    [SerializeField] private string name;
    public string Name { get { return name; } set { name = value; } }
    [SerializeField] private KeyCode positiveX;
    [SerializeField] private KeyCode negativeX;
    [SerializeField] private KeyCode positiveY;
    [SerializeField] private KeyCode negativeY;
    [SerializeField] private KeyCode positiveZ;
    [SerializeField] private KeyCode negativeZ;
    [Header("Data")]
    [SerializeField] private DataVector3 dataNode;
    [Header("Events")]
    [SerializeField] private UnityEvent onInputStart;
    [SerializeField] private UnityEvent onInput;
    [SerializeField] private UnityEvent onInputStop;

    public override void Start(Data data)
    {
        if (data.Has(dataNode))
            dataNode = data.GetVector3(dataNode.Name);
        else
            data.Add(dataNode);
    }

    public override void Update()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(positiveX))
            input.x += 1;
        if (Input.GetKey(negativeX))
            input.x -= 1;
        if (Input.GetKey(positiveY))
            input.y += 1;
        if (Input.GetKey(negativeY))
            input.y -= 1;
        if (Input.GetKey(positiveZ))
            input.z += 1;
        if (Input.GetKey(negativeZ))
            input.z -= 1;


        float inputMag = input.sqrMagnitude;
        float dataMag = dataNode.Value.sqrMagnitude;

        if (inputMag != 0 && dataMag == 0)
            onInputStart.Invoke();
        if (inputMag != 0 && dataMag != 0)
            onInput.Invoke();
        if (inputMag == 0 && dataMag != 0)
            onInputStop.Invoke();

        if (inputMag > 0)
            input.Normalize();

        dataNode.Value = input;
    }
}
