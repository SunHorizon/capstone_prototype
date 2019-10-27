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
    [SerializeField] private int slots;
    [SerializeField] private int rows;

    // spacing between each slot
    [SerializeField] private float slotPaddingLeft, slotPaddingTop;

    // size of slot which is also size of the inventory
    [SerializeField] private float slotSize;

    // image of the slot
    [SerializeField] private GameObject slotPrefab;

    // hover prefab and object 
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private static GameObject HoverObject;

    // name of the inventory to see what type of item to store
    public string name;

    // list of all the slots in the inventory
    private List<GameObject> allSlots;

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
    private void CreateLayout()
    {
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
                        temp.AddItem(items);
                        return true;
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
        // checking if nothing is in from
        if (from == null && CloseOpenInventory.CanvasGroup.alpha == 1)
        {
            //if the clicked slot is not empty 
            if (!clicked.GetComponent<Slot>().isEmpty && !GameObject.Find("Hover"))
            {
                // place the clicked item in from 
                from = clicked.GetComponent<Slot>();
                from.GetComponent<Image>().color = Color.gray;

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
                HoverObject.transform.localScale = from.gameObject.transform.localScale;
            }
        }
        else if (to == null)
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

    // to put the items back in inventory when it is closed
    private void PutItemBack()
    {
        if (from != null)
        {
            Destroy(GameObject.Find("Hover")); // destroy the hover object
            from.GetComponent<Image>().color = Color.white; // change the color back to white in the slot
            from = null;
        }
    } 
}
