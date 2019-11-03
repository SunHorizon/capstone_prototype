using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    // all the items that a slot has
    private Stack<Items> items = new Stack<Items>();

    // text for stacked items
    [SerializeField] private Text stackText;

    // sprites if the slot is empty
    [SerializeField] private Sprite slotEmpty;
    [SerializeField] private Sprite slotHighlight;

    private Inventory inv;

    // checking if the slot is empty
    public bool isEmpty
    {
        get { return items.Count == 0; }
    }

    // return the current item in the slot
    public Items CurrentItem
    {
        get { return items.Peek(); }
    }

    // checking if item can stack
    public bool isAvailable
    {
        get { return CurrentItem.maxSize > items.Count; }
    }

    // getter and setting for items
    public Stack<Items> Items
    {
        get { return items; }
        set { items = value; }
    }

    void Awake()
    {
        // making the list of items
        items = new Stack<Items>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // getting the rect of this slot
        RectTransform slotRect = GetComponent<RectTransform>();

        // getting the rect of the text
        RectTransform textRect = stackText.GetComponent<RectTransform>();

        // text factor 60% size of the slot 
        int textScaleFactor = (int)(slotRect.sizeDelta.x * 0.60);

        // setting the max and min size of text
        stackText.resizeTextMaxSize = textScaleFactor;
        stackText.resizeTextMinSize = textScaleFactor;

        // setting the position of the text
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);

        //// get the parent inventory of slot
        //inv = transform.parent.GetComponent<Inventory>();
    }
    
    // add items
    public void AddItem(Items item)
    {
        // adding the item to the slot
        items.Push(item);

        // checking if there is more the one item
        if (items.Count > 1)
        {
            // adding the number of items to the text
            stackText.text = items.Count.ToString();
        }
        
        // change the sprite
        ChangeSprite(item.GetSpriteNeutral(), item.GetSpriteHighlighted());

    }

    // adding stack of items for slot movement
    public void AddItems(Stack<Items> Items)
    {
        this.items = new Stack<Items>(Items);

        // update the string 
        if (items.Count > 1)
        {
            stackText.text = items.Count.ToString();
        }
        else
        {
            stackText.text = string.Empty;
        }

        ChangeSprite(CurrentItem.GetSpriteNeutral(), CurrentItem.GetSpriteHighlighted());
    }

    // changing the sprite of slot
    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        // changing the sprite of the image component
        GetComponent<Image>().sprite = neutral;

        // making new sprite state
        SpriteState st = new  SpriteState();

        // setting the highlighted Sprite
        st.highlightedSprite = highlight;

        // setting the sprite if pressed
        st.pressedSprite = neutral;

        // Adding the new sprite st to the button component
        GetComponent<Button>().spriteState = st;
    }

    // use item
    private void UseItem()
    {
        // if not empty pop item for stack and update text
        if (!isEmpty)
        {
            if (items.Peek().tag != "Weapon")
            {
                items.Pop().Use();

                // update the string 
                if (items.Count > 1)
                {
                    stackText.text = items.Count.ToString();
                }
                else
                {
                    stackText.text = string.Empty;
                }

                // if the slot is empty change the sprite to the default one
                if (isEmpty)
                {
                    ChangeSprite(slotEmpty, slotHighlight);
                    Inventory.EmptySlot++;
                }
            }
        }
    }

    // items to remove from slot
    public Stack<Items> RemoveItems(int amount)
    {
        Stack<Items> tmp = new Stack<Items>();

        // remove the items and out it into temp
        for (int i = 0; i < amount; i++)
        {
            tmp.Push(items.Pop());
        }

        // update the stack text count if item count is larger then 1
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        return tmp;
    }

    public Items RemoveItem()
    {  
        Items temp;

        // pop one item
        temp = items.Pop();

        // update the stack text count if item count is larger then 1
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        return temp;
    }

    // using item if clicked
    public void OnPointerClick(PointerEventData eventData)
    {

        // right click to use item
        if (eventData.button == PointerEventData.InputButton.Right && !GameObject.Find("Hover") && CloseOpenInventory.CanvasGroup.alpha > 0 && !StackOpenClose.OpenCloseStack)
        {
            // using the item 
            UseItem();
        }
        // shift right click to split item
        else if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift) && !isEmpty && !GameObject.Find("Hover"))
        {
            // get the parent inventory of slot
            if (!inv)
            {
                inv = transform.parent.GetComponent<Inventory>();
            }

            Vector2 pos;

            // get the position of the click slot
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inv.canvas.transform as RectTransform,
                Input.mousePosition, inv.canvas.worldCamera, out pos);

            // set the stack object to active
            inv.selectStackedSize.SetActive(true);

            // put the stack object to where we clicked
            inv.selectStackedSize.transform.position = inv.canvas.transform.TransformPoint(pos);

            // set the stack info
            inv.setStackInfo(items.Count);
        }
    }

    // clear slot
    public void ClearSlot()
    {
        // clear the slot change the sprite and text
        items.Clear();
        ChangeSprite(slotEmpty, slotHighlight);
        stackText.text = string.Empty;
    }

}
