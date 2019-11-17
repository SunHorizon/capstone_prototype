using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // instance static
    private static InventoryManager instance;

    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
            }
            return instance; 
        }
    }


    // image of the slot
    [SerializeField] public GameObject slotPrefab;
 

    // hover prefab and object 
    [SerializeField] public GameObject IconPrefab;
    [SerializeField] private GameObject hoverObject;
    public GameObject HoverObject
    {
        get { return hoverObject; }
        set { hoverObject = value; }
    }

    // prototype of the itemScript
    // inventory itemScript
    public GameObject Mana;
    public GameObject Health;
    public GameObject Sword;

    // tool tip object
    public GameObject toolTipObject;

    // text for the size of the tool tip
    public Text SizeTextObject;

    // text to show
    public Text visualTextObject;

    public Text VisualTextObject
    {
        get { return visualTextObject; }
        set { visualTextObject = value; }
    }

    // get the canvas
    public Canvas canvas;

    // slots moving from and to 
    private Slot from, to;

    public Slot From
    {
        get { return from;}
        set { from = value; }
    }

    public Slot To
    {
        get { return to; }
        set { to = value; }
    }

    // clicked object
    private GameObject clicked;

    public GameObject Clicked
    {
        get { return clicked; }
        set { clicked = value; }
    }

    // object for the stack size
    public GameObject selectStackedSize;

    // text for the stack size
    public Text stackText;

    // the amount to split
    private int splitAmount;

    public int SplitAmount
    {
        get { return splitAmount; }
        set { splitAmount = value; }
    }

    // the max stack the itemScript can move
    private int maxSizeCount;
    public int MaxSizeCount
    {
        get { return maxSizeCount; }
        set { maxSizeCount = value; }
    }


    // temp variable to put into when moving itemScript 
    private Slot movingSlot;
    public Slot MovingSlot
    {
        get { return movingSlot; }
        set { movingSlot = value; }
    }


    // event system to destory itemScript in inventory
    public EventSystem eventSystem;


    // info of stack 
    public void setStackInfo(int maxStackCount)
    {
        // set the stack size object to true
        selectStackedSize.SetActive(true);

        toolTipObject.SetActive(false);

        // stack is open 
        StackOpenClose.OpenCloseStack = true;

        // split amount is 0 at the start
        splitAmount = 0;
        // max size count of the object
        MaxSizeCount = maxStackCount;
        // change the text to split amount
        stackText.text = splitAmount.ToString();
    }
}
