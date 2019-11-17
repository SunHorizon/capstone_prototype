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


    // name of the inventory to see what type of itemScript to store
    public string name;

    // list of all the slots in the inventory
    public List<GameObject> allSlots;

  
    // offset for hover icon
    private float HoverYffSet;

    // checking how many empty slots are left
    private int emptySlot;


    // making getter and setter for empty slot 
    public int EmptySlot
    {
        get { return emptySlot; }
        set { emptySlot = value; }
    }


    // Start is called before the first frame update
    void Start()
    {
        // make the inventory
        CreateLayout();

        InventoryManager.Instance.MovingSlot = GameObject.Find("MovingSlot").GetComponent<Slot>();

    }

    // Update is called once per frame
    void Update()
    {
        // delete itemScript when drag out of inventory
        if (Input.GetMouseButton(0))
        {
            if (!InventoryManager.Instance.eventSystem.IsPointerOverGameObject(-1) && InventoryManager.Instance.From != null)
            {
                InventoryManager.Instance.From.GetComponent<Image>().color = Color.white; // reset the color
                InventoryManager.Instance.From.ClearSlot(); // clear all the itemScript from the slot 
                Destroy(GameObject.Find("Hover")); // Destroy all the object

                // reset all the objects
                InventoryManager.Instance.To = null;
                InventoryManager.Instance.From = null;
                emptySlot++;
            }
            else if (!InventoryManager.Instance.eventSystem.IsPointerOverGameObject(-1) && !InventoryManager.Instance.MovingSlot.isEmpty)
            {
                InventoryManager.Instance.MovingSlot.ClearSlot();
                Destroy(GameObject.Find("Hover"));
            }
        }

        // move object 
        if (InventoryManager.Instance.HoverObject != null)
        {
            // follows the mouse around
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform, Input.mousePosition,
                InventoryManager.Instance.canvas.worldCamera, out position);
            position.Set(position.x, position.y - HoverYffSet);
            InventoryManager.Instance.HoverObject.transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(position);
        }

        if (CloseOpenInventory.CheckFade)
        {
            PutItemBack(); // put itemScript back if it is selected 
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
                GameObject newSlot = (GameObject)Instantiate(InventoryManager.Instance.slotPrefab);

                // get the rect transform of the new slot
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                // name the slot
                newSlot.name = "Slot";

                // making the canvas the parent of slot
                newSlot.transform.SetParent(this.transform.parent);

                // placing the slot in the inventory
                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPaddingLeft + (x * slotPaddingLeft) + (slotSize * x), -slotPaddingTop * (y + 1) - (slotSize *  y));
               

                // setting the size
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize * InventoryManager.Instance.canvas.scaleFactor);
                newSlot.transform.SetParent(this.transform);

                // adding the new slot to the list
                allSlots.Add(newSlot);

            }
        }
    }

    // add itemScript to slot
    public bool AddItem(ItemScript itemScript)
    {
        // add itemScript if itemScript is stackable
        if (itemScript.maxSize == 1)
        {
            PlaceEmpty(itemScript);
            return true;
        }
        else
        {
            // if itemScript can stack 
            foreach (GameObject slot in allSlots)
            {
                Slot temp = slot.GetComponent<Slot>();
                // is the slot is not empty. stack it
                if (!temp.isEmpty)
                {
                    // if current itemScript is the same type and it can stack
                    if (temp.CurrentItemScript.type == itemScript.type && temp.isAvailable)
                    {
                        if (!InventoryManager.Instance.MovingSlot.isEmpty && InventoryManager.Instance.Clicked.GetComponent<Slot>() == temp.GetComponent<Slot>())
                        {
                            continue;
                        }
                        else
                        {
                            temp.AddItem(itemScript);
                            return true;
                        }       
                    }
                }
            }

            // if can't find the slot to stack
            if (emptySlot > 0)
            {
                PlaceEmpty(itemScript);
            }
        }

        return false;
    }

    // place the itemScript in the empty slot
    private bool PlaceEmpty(ItemScript itemScript)
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
                    temp.AddItem(itemScript);

                    // subtract empty Slot
                    emptySlot--;
                    return true;
                }
            }
        }
        return false;
    }

    // move itemScript from slot to slot
    public void MoveItem(GameObject clicked)
    {
        InventoryManager.Instance.Clicked = clicked;
       
        // if the are trying to split the itemScript 
        if (!InventoryManager.Instance.MovingSlot.isEmpty)
        {
            Slot temp = clicked.GetComponent<Slot>();

            // if the slot is empty then add all the itemScript in to slot
            if (temp.isEmpty)
            {
                // add the itemScript from moving slot to temp
                temp.AddItems(InventoryManager.Instance.MovingSlot.Items);
                // clear the itemScript in the moving slot
                InventoryManager.Instance.MovingSlot.Items.Clear();
                // destroy the hover object
                Destroy(GameObject.Find("Hover"));
            }
            // if the slot is not empty 
            else if(!temp.isEmpty && InventoryManager.Instance.MovingSlot.CurrentItemScript.type == temp.CurrentItemScript.type && temp.isAvailable)
            {
                MergeStacks(InventoryManager.Instance.MovingSlot, temp);
            }
        }
        // checking if nothing is in from
        else if (InventoryManager.Instance.From == null && CloseOpenInventory.CanvasGroup.alpha == 1 && !Input.GetKey(KeyCode.LeftShift) && !StackOpenClose.OpenCloseStack)
        {
            //if the clicked slot is not empty 
            if (!clicked.GetComponent<Slot>().isEmpty && !GameObject.Find("Hover"))
            {
                // place the clicked itemScript in from 
                InventoryManager.Instance.From = clicked.GetComponent<Slot>();
                InventoryManager.Instance.From.GetComponent<Image>().color = Color.gray;

                CreateHoverIcon();
            }
        }
        else if (InventoryManager.Instance.To == null && !Input.GetKey(KeyCode.LeftShift) && !StackOpenClose.OpenCloseStack)
        {
            InventoryManager.Instance.To = clicked.GetComponent<Slot>();
            // destroy the hover object ones its placed
            Destroy(GameObject.Find("Hover"));

        }
        if (InventoryManager.Instance.To != null && InventoryManager.Instance.From != null)
        {
            // swap itemScript 
            Stack<ItemScript> tempTo = new Stack<ItemScript>(InventoryManager.Instance.To.Items);
            // add the selected itemScript to to
            InventoryManager.Instance.To.AddItems(InventoryManager.Instance.From.Items);

            // if slot is empty the clear it 
            if (tempTo.Count == 0)
            {
                InventoryManager.Instance.From.ClearSlot();
            }
            else
            {
                // add slot if not empty
                InventoryManager.Instance.From.AddItems(tempTo);
            }
            // change the color back to white
            InventoryManager.Instance.From.GetComponent<Image>().color = Color.white;

            InventoryManager.Instance.To = null;
            InventoryManager.Instance.From = null;
            Destroy(GameObject.Find("Hover"));
        }
    }

    // hover icon funcation 
    private void CreateHoverIcon()
    {
        // place the icon prefab for hover
        InventoryManager.Instance.HoverObject = (GameObject)Instantiate(InventoryManager.Instance.IconPrefab);
        InventoryManager.Instance.HoverObject.GetComponent<Image>().sprite = InventoryManager.Instance.Clicked.GetComponent<Image>().sprite;
        InventoryManager.Instance.HoverObject.name = "Hover";

        // get hover and clicked rect 
        RectTransform hoverTransform = InventoryManager.Instance.HoverObject.GetComponent<RectTransform>();
        RectTransform ClickedTransform = InventoryManager.Instance.Clicked.GetComponent<RectTransform>();

        // set the hover size of the clicked object
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ClickedTransform.sizeDelta.x);
        hoverTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ClickedTransform.sizeDelta.y);

        // make hover object child of canvas 
        InventoryManager.Instance.HoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);

        // size the normal size of clicked object
        InventoryManager.Instance.HoverObject.transform.localScale = InventoryManager.Instance.Clicked.gameObject.transform.localScale;

        InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text =
            InventoryManager.Instance.MovingSlot.Items.Count > 1 ? InventoryManager.Instance.MovingSlot.Items.Count.ToString() : string.Empty;
    }

    // to put the itemScript back in inventory when it is closed
    private void PutItemBack()
    {
        if (InventoryManager.Instance.From != null)
        {
            Destroy(GameObject.Find("Hover")); // destroy the hover object
            InventoryManager.Instance.From.GetComponent<Image>().color = Color.white; // change the color back to white in the slot
            InventoryManager.Instance.From = null;
        }
        else if (!InventoryManager.Instance.MovingSlot.isEmpty)
        {
            Destroy(GameObject.Find("Hover"));
            foreach (ItemScript item in InventoryManager.Instance.MovingSlot.Items)
            {
                InventoryManager.Instance.Clicked.GetComponent<Slot>().AddItem(item);
            }
            InventoryManager.Instance.MovingSlot.ClearSlot(); 
        }
        InventoryManager.Instance.selectStackedSize.SetActive(false);
    }
  
    // function to slit stacks
    public void splitStack()
    {
        // set it to false 
        InventoryManager.Instance.selectStackedSize.SetActive(false);

        // stack is closed
        StackOpenClose.OpenCloseStack = false;

        if (InventoryManager.Instance.SplitAmount == InventoryManager.Instance.MaxSizeCount)
        {
            MoveItem(InventoryManager.Instance.Clicked);
        }
        else if(InventoryManager.Instance.SplitAmount > 0)
        {
            // move the slot 
            InventoryManager.Instance.MovingSlot.Items = InventoryManager.Instance.Clicked.GetComponent<Slot>().RemoveItems(InventoryManager.Instance.SplitAmount);

            // make the hover icon
            CreateHoverIcon();
        }
    }

    // change the text for the stack object
    public void ChangeStackText(int i)
    {
        InventoryManager.Instance.SplitAmount += i;

        // if the split amount is 0 then set it to 0
        if (InventoryManager.Instance.SplitAmount < 0)
        {
            InventoryManager.Instance.SplitAmount = 0;
        }

        // if the split amount lager then MaxSizeCount then set it to MaxSizeCount
        if (InventoryManager.Instance.SplitAmount > InventoryManager.Instance.MaxSizeCount)
        {
            InventoryManager.Instance.SplitAmount = InventoryManager.Instance.MaxSizeCount;
        }
        // update the text 
        InventoryManager.Instance.stackText.text = InventoryManager.Instance.SplitAmount.ToString();

    }

    // function to merge stacks
    public void MergeStacks(Slot source, Slot destination)
    {
        // the max size at the destination
        int max = destination.CurrentItemScript.maxSize - destination.Items.Count;
        
        // amount of itemScript to move
        int count = source.Items.Count < max ? source.Items.Count : max;

        // add the split itemScript to the destination
        for (int i = 0; i < count; i++)
        {
            destination.AddItem(source.RemoveItem());
            InventoryManager.Instance.HoverObject.transform.GetChild(0).GetComponent<Text>().text = InventoryManager.Instance.MovingSlot.Items.Count.ToString();
        }
        // if the source is 0 clear it
        if (source.Items.Count == 0)
        {
            source.ClearSlot();
            InventoryManager.Instance.To = null;
            InventoryManager.Instance.From = null;
            Destroy(GameObject.Find("Hover"));
        }
    }

    // show the tool tip function
    public void ShowToolTip(GameObject slot)
    {
        // get the Component of slot
        Slot tempSlot = slot.GetComponent<Slot>();
        
        // if the slot is not empty
        if (!tempSlot.isEmpty && InventoryManager.Instance.HoverObject == null && !InventoryManager.Instance.selectStackedSize.activeSelf)
        {
            // set the visual Text
            InventoryManager.Instance.VisualTextObject.text = tempSlot.CurrentItemScript.GetToolTip();
            //set the size of the text
            InventoryManager.Instance.SizeTextObject.text = InventoryManager.Instance.VisualTextObject.text;

            // set the tool tip to active
            InventoryManager.Instance.toolTipObject.SetActive(true);

            // set the x pos for the tool tip
            float xPos = slot.transform.position.x + slotPaddingLeft;

            // set the y pos for the tool tip
            float yPos = slot.transform.position.y - slot.GetComponent<RectTransform>().sizeDelta.y - slotPaddingTop;

            // set the pos of the tool tip
            InventoryManager.Instance.toolTipObject.transform.position = new Vector2(xPos, yPos);
        }

    }

    // hide the tool tip function
    public void HideToolTip()
    {
        // set the tool tip to false
        InventoryManager.Instance.toolTipObject.SetActive(false);
    }
}
