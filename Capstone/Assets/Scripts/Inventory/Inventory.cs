using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // inventory background rect
    private RectTransform inventoryRect;

    // width and height of inventory
    private float inventoryWidth, inventoryHeight;

    // number of slots and rows for the inventory
    [SerializeField] public int slots;
    [SerializeField] public int rows;

    // spacing between each slot
    [SerializeField] public float slotPaddingLeft, slotPaddingTop;

    // size of slot which is also size of the inventory
    [SerializeField] public float slotSize;

    // image of the slot
    [SerializeField] private GameObject slotPrefab;

    // hover prefab and object 
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private static GameObject HoverObject;

    // name of the inventory to see what type of item to store
    public string name;

    // list of all the slots in the inventory
    public List<GameObject> allSlots;

    // get the canvas
    public Canvas canvas;

    // offset for hover icon
    private float HoverYffSet;

    // checking how many empty slots are left
    private static int emptySlot;

    // slots moving from and to 
    private static Slot from, to;

    // event system to destory items in inventory
    public EventSystem eventSystem;

    private static GameObject clicked;

    // object for the stack size
    public GameObject selectStackedSize;
    private static GameObject selectStackedSizeStatic;

    // text for the stack size
    public Text stackText;

    // the amount to split
    private int splitAmount;

    // the max stack the item can move
    private int MaxSizeCount;

    // temp variable to put into when moving items 
    private static Slot movingSlot;

    // object for tool tip static
    private static GameObject toolTip;
    // tool tip object
    public GameObject toolTipObject;
    // text for the size of the tool tip
    public Text SizeTextObject;
    private static Text SizeText;
    // text to show
    public Text visualTextObject;
    private static Text VisualText;


    // making getter and setter for empty slot 
    public static int EmptySlot
    {
        get { return emptySlot; }
        set { emptySlot = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // make the inventory
        CreateLayout();

        movingSlot = GameObject.Find("MovingSlot").GetComponent<Slot>();

        // set the tooltip objects
        toolTip = toolTipObject;
        SizeText = SizeTextObject;
        VisualText = visualTextObject;


        selectStackedSizeStatic = selectStackedSize;
    }

    // Update is called once per frame
    void Update()
    {
        // delete item when drag out of inventory
        if (Input.GetMouseButton(0))
        {
            if (!eventSystem.IsPointerOverGameObject(-1) && from != null)
            {
                from.GetComponent<Image>().color = Color.white; // reset the color
                from.ClearSlot(); // clear all the items from the slot 
                Destroy(GameObject.Find("Hover")); // Destroy all the object

                // reset all the objects
                to = null;
                from = null;
                emptySlot++;
            }
            else if (!eventSystem.IsPointerOverGameObject(-1) && !movingSlot.isEmpty)
            {
                movingSlot.ClearSlot();
                Destroy(GameObject.Find("Hover"));
            }
        }

        // move object 
        if (HoverObject != null)
        {
            // follows the mouse around
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition,
                canvas.worldCamera, out position);
            position.Set(position.x, position.y - HoverYffSet);
            HoverObject.transform.position = canvas.transform.TransformPoint(position);
        }

        if (CloseOpenInventory.CheckFade)
        {
            PutItemBack(); // put item back if it is selected 
        }
    }

    // create the inventory
    public void CreateLayout()
    {
        // if all slots exits destroy it
        if (allSlots != null)
        {
            // loop through all the slots and destroy it
            foreach (GameObject slo in allSlots)
            {
                Destroy(slo);
            }
        }

        // instantiate the slots
        allSlots = new List<GameObject>();

        // set hover offset
        HoverYffSet = slotSize * 0.01f;

        // all slots and empty at the beginning
        emptySlot = slots;

        // calculate the inventory width
        inventoryWidth = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;

        // calculate the inventory height
        inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;

        // get the rect component
        inventoryRect = GetComponent<RectTransform>();

        // set the width and height in the rect for the inventory
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);

        // calculate the columns
        int columns = slots / rows;
        for(int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // make the new slot
                GameObject newSlot = (GameObject)Instantiate(slotPrefab);

                // get the rect transform of the new slot
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                // name the slot
                newSlot.name = "Slot";

                // making the canvas the parent of slot
                newSlot.transform.SetParent(this.transform.parent);

                // placing the slot in the inventory
                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft + (x * slotPaddingLeft) + (slotSize * x), -slotPaddingTop * (y + 1) - (slotSize *  y));
               

                // setting the size
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * canvas.scaleFactor);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * canvas.scaleFactor);
                newSlot.transform.SetParent(this.transform);

                // adding the new slot to the list
                allSlots.Add(newSlot);

            }
        }
    }

    // add item to slot
    public bool AddItem(Items items)
    {
        // add item if item is stackable
        if (items.maxSize == 1)
        {
            PlaceEmpty(items);
            return true;
        }
        else
        {
            // if item can stack 
            foreach (GameObject slot in allSlots)
            {
                Slot temp = slot.GetComponent<Slot>();
                // is the slot is not empty. stack it
                if (!temp.isEmpty)
                {
                    // if current item is the same type and it can stack
                    if (temp.CurrentItem.type == items.type && temp.isAvailable)
                    {
                        if (!movingSlot.isEmpty && clicked.GetComponent<Slot>() == temp.GetComponent<Slot>())
                        {
                            continue;
                        }
                        else
                        {
                            temp.AddItem(items);
                            return true;
                        }       
                    }
                }
            }

            // if can't find the slot to stack
            if (emptySlot > 0)
            {
                PlaceEmpty(items);
            }
        }

        return false;
    }

    // place the item in the empty slot
    private bool PlaceEmpty(Items item)
    {
        // checking if there are any empty slots
        if (emptySlot > 0)
        {
            // looping through all slots
            foreach (GameObject slot in allSlots)
            {
                Slot temp = slot.GetComponent<Slot>();

                // check if slot is empty
                if (temp.isEmpty)
                {
                    // adding the slot
                    temp.AddItem(item);

                    // subtract empty Slot
                    emptySlot--;
                    return true;
                }
            }
        }
        return false;
    }

    // move items from slot to slot
    public void MoveItem(GameObject clicked)
    {
        Inventory.clicked = clicked;
       
        // if the are trying to split the item 
        if (!movingSlot.isEmpty)
        {
            Slot temp = clicked.GetComponent<Slot>();

            // if the slot is empty then add all the items in to slot
            if (temp.isEmpty)
            {
                // add the items from moving slot to temp
                temp.AddItems(movingSlot.Items);
                // clear the items in the moving slot
                movingSlot.Items.Clear();
                // destroy the hover object
                Destroy(GameObject.Find("Hover"));
            }
            // if the slot is not empty 
            else if(!temp.isEmpty && movingSlot.CurrentItem.type == temp.CurrentItem.type && temp.isAvailable)
            {
                MergeStacks(movingSlot, temp);
            }
        }
        // checking if nothing is in from
        else if (from == null && CloseOpenInventory.CanvasGroup.alpha == 1 && !Input.GetKey(KeyCode.LeftShift) && !StackOpenClose.OpenCloseStack)
        {
            //if the clicked slot is not empty 
            if (!clicked.GetComponent<Slot>().isEmpty && !GameObject.Find("Hover"))
            {
                // place the clicked item in from 
                from = clicked.GetComponent<Slot>();
                from.GetComponent<Image>().color = Color.gray;

                CreateHoverIcon();
            }
        }
        else if (to == null && !Input.GetKey(KeyCode.LeftShift) && !StackOpenClose.OpenCloseStack)
        {
            to = clicked.GetComponent<Slot>();
            // destroy the hover object ones its placed
            Destroy(GameObject.Find("Hover"));

        }
        if (to != null && from != null)
        {
            // swap items 
            Stack<Items> tempTo = new Stack<Items>(to.Items);
            // add the selected item to to
            to.AddItems(from.Items);

            // if slot is empty the clear it 
            if (tempTo.Count == 0)
            {
                from.ClearSlot();
            }
            else
            {
                // add slot if not empty
                from.AddItems(tempTo);
            }
            // change the color back to white
            from.GetComponent<Image>().color = Color.white;

            to = null;
            from = null;
            Destroy(GameObject.Find("Hover"));
        }
    }

    // hover icon funcation 
    private void CreateHoverIcon()
    {
        // place the icon prefab for hover
        HoverObject = (GameObject)Instantiate(IconPrefab);
        HoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
        HoverObject.name = "Hover";

        // get hover and clicked rect 
        RectTransform hoverTransform = HoverObject.GetComponent<RectTransform>();
        RectTransform ClickedTransform = clicked.GetComponent<RectTransform>();

        // set the hover size of the clicked object
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ClickedTransform.sizeDelta.x);
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ClickedTransform.sizeDelta.y);

        // make hover object child of canvas 
        HoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);

        // size the normal size of clicked object
        HoverObject.transform.localScale = clicked.gameObject.transform.localScale;

        HoverObject.transform.GetChild(0).GetComponent<Text>().text =
            movingSlot.Items.Count > 1 ? movingSlot.Items.Count.ToString() : string.Empty;
    }

    // to put the items back in inventory when it is closed
    private void PutItemBack()
    {
        if (from != null)
        {
            Destroy(GameObject.Find("Hover")); // destroy the hover object
            from.GetComponent<Image>().color = Color.white; // change the color back to white in the slot
            from = null;
        }
        else if (!movingSlot.isEmpty)
        {
            Destroy(GameObject.Find("Hover"));
            foreach (Items item in movingSlot.Items)
            {
                clicked.GetComponent<Slot>().AddItem(item);
            }
            movingSlot.ClearSlot(); 
        }
        selectStackedSize.SetActive(false);
    }

    // info of stack 
    public void setStackInfo(int maxStackCount)
    {
        // set the stack size object to true
        selectStackedSize.SetActive(true);

        toolTip.SetActive(false);

        // stack is open 
        StackOpenClose.OpenCloseStack = true;
  
        // split amount is 0 at the start
        splitAmount = 0;
        // max size count of the object
        MaxSizeCount = maxStackCount;
        // change the text to split amount
        stackText.text = splitAmount.ToString();
    }
    
    // function to slit stacks
    public void splitStack()
    {
        // set it to false 
        selectStackedSize.SetActive(false);

        // stack is closed
        StackOpenClose.OpenCloseStack = false;

        if (splitAmount == MaxSizeCount)
        {
            MoveItem(clicked);
        }
        else if(splitAmount > 0)
        {
            // move the slot 
            movingSlot.Items = clicked.GetComponent<Slot>().RemoveItems(splitAmount);

            // make the hover icon
            CreateHoverIcon();
        }
    }

    // change the text for the stack object
    public void ChangeStackText(int i)
    {
        splitAmount += i;

        // if the split amount is 0 then set it to 0
        if (splitAmount < 0)
        {
            splitAmount = 0;
        }

        // if the split amount lager then MaxSizeCount then set it to MaxSizeCount
        if (splitAmount > MaxSizeCount)
        {
            splitAmount = MaxSizeCount;
        }
        // update the text 
        stackText.text = splitAmount.ToString();

    }

    // function to merge stacks
    public void MergeStacks(Slot source, Slot destination)
    {
        // the max size at the destination
        int max = destination.CurrentItem.maxSize - destination.Items.Count;
        
        // amount of items to move
        int count = source.Items.Count < max ? source.Items.Count : max;

        // add the split items to the destination
        for (int i = 0; i < count; i++)
        {
            destination.AddItem(source.RemoveItem());
            HoverObject.transform.GetChild(0).GetComponent<Text>().text = movingSlot.Items.Count.ToString();
        }
        // if the source is 0 clear it
        if (source.Items.Count == 0)
        {
            source.ClearSlot();
            to = null;
            from = null;
            Destroy(GameObject.Find("Hover"));
        }
    }

    // show the tool tip function
    public void ShowToolTip(GameObject slot)
    {
        // get the Component of slot
        Slot tempSlot = slot.GetComponent<Slot>();
        
        // if the slot is not empty
        if (!tempSlot.isEmpty && HoverObject == null && !selectStackedSizeStatic.activeSelf)
        {
            // set the visual Text
            VisualText.text = tempSlot.CurrentItem.GetToolTip();
            //set the size of the text
            SizeText.text = VisualText.text;

            // set the tool tip to active
            toolTip.SetActive(true);

            // set the x pos for the tool tip
            float xPos = slot.transform.position.x + slotPaddingLeft;

            // set the y pos for the tool tip
            float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;

            // set the pos of the tool tip
            toolTip.transform.position = new Vector2(xPos, yPos);
        }

    }

    // hide the tool tip function
    public void HideToolTip()
    {
        // set the tool tip to false
        toolTip.SetActive(false);
    }
}
