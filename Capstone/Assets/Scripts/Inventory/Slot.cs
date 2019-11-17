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
    // all the itemScript that a slot has
    private Stack<ItemScript> items = new Stack<ItemScript>();

    // text for stacked itemScript
    [SerializeField] private Text stackText;

    // sprites if the slot is empty
    [SerializeField] private Sprite slotEmpty;
    [SerializeField] private Sprite slotHighlight;


    // checking if the slot is empty
    public bool isEmpty
    {
        get { return items.Count == 0; }
    }

    // return the current itemScript in the slot
    public ItemScript CurrentItemScript
    {
        get { return items.Peek(); }
    }

    // checking if itemScript can stack
    public bool isAvailable
    {
        get { return CurrentItemScript.maxSize > items.Count; }
    }

    // getter and setting for itemScript
    public Stack<ItemScript> Items
    {
        get { return items; }
        set { items = value; }
    }

    void Awake()
    {
        // making the list of itemScript
        items = new Stack<ItemScript>();
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
    
    // add itemScript
    public void AddItem(ItemScript itemScript)
    {
        // adding the itemScript to the slot
        items.Push(itemScript);

        // checking if there is more the one itemScript
        if (items.Count > 1)
        {
            // adding the number of itemScript to the text
            stackText.text = items.Count.ToString();
        }
        
        // change the sprite
        ChangeSprite(itemScript.GetSpriteNeutral(), itemScript.GetSpriteHighlighted());

    }

    // adding stack of itemScript for slot movement
    public void AddItems(Stack<ItemScript> Items)
    {
        this.items = new Stack<ItemScript>(Items);

        // update the string 
        if (items.Count > 1)
        {
            stackText.text = items.Count.ToString();
        }
        else
        {
            stackText.text = string.Empty;
        }

        ChangeSprite(CurrentItemScript.GetSpriteNeutral(), CurrentItemScript.GetSpriteHighlighted());
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

    // use itemScript
    private void UseItem()
    {
        // if not empty pop itemScript for stack and update text
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
                    transform.parent.GetComponent<Inventory>().EmptySlot++;
                }
            }
        }
    }

    // itemScript to remove from slot
    public Stack<ItemScript> RemoveItems(int amount)
    {
        Stack<ItemScript> tmp = new Stack<ItemScript>();

        // remove the itemScript and out it into temp
        for (int i = 0; i < amount; i++)
        {
            tmp.Push(items.Pop());
        }

        // update the stack text count if itemScript count is larger then 1
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        return tmp;
    }

    public ItemScript RemoveItem()
    {  
        ItemScript temp;

        // pop one itemScript
        temp = items.Pop();

        // update the stack text count if itemScript count is larger then 1
        stackText.text = items.Count > 1 ? items.Count.ToString() : string.Empty;

        return temp;
    }

    // using itemScript if clicked
    public void OnPointerClick(PointerEventData eventData)
    {

        // right click to use itemScript
        if (eventData.button == PointerEventData.InputButton.Right && !GameObject.Find("Hover") && CloseOpenInventory.CanvasGroup.alpha > 0 && !StackOpenClose.OpenCloseStack)
        {
            // using the itemScript 
            UseItem();
        }
        // shift right click to split itemScript
        else if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift) && !isEmpty && !GameObject.Find("Hover"))
        {
 
            Vector2 pos;

            // get the position of the click slot
            RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance.canvas.transform as RectTransform,
                Input.mousePosition, InventoryManager.Instance.canvas.worldCamera, out pos);

            // set the stack object to active
            InventoryManager.Instance.selectStackedSize.SetActive(true);

            // put the stack object to where we clicked
            InventoryManager.Instance.selectStackedSize.transform.position = InventoryManager.Instance.canvas.transform.TransformPoint(pos);

            // set the stack info
            InventoryManager.Instance.setStackInfo(items.Count);
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
