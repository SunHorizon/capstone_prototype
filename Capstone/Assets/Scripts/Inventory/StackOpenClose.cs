using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackOpenClose : MonoBehaviour
{
    private static bool openClosStack;

    public static bool OpenCloseStack
    {
        get { return openClosStack; }
        set { openClosStack = value; }
    }
}
