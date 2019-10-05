using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleCanvas : MonoBehaviour
{
    // canvas scaler
    private CanvasScaler scaler;

    // Start is called before the first frame update
    void Start()
    {
        scaler = GetComponent<CanvasScaler>();

        // change the canvas scaler to screen size so the inventory is the correct size
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    }
   
}
