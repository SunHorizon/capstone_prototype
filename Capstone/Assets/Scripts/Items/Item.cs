using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    // get the Item type
    public ItemTpye itemType { get; set; }

    // get the quality of item
    public Quality quality { get; set; }

    // get the string of sprite in neutral state
    public string SpriteNeutral { get; set; }

    // get the string of sprite in Highlighted state
    public string SpriteHighlighted { get; set; }

    // get the max size of the item
    public int MaxSize { get; set; }

    // get the Item name
    public string ItemName { get; set; }

    // get the description for the items 
    public string Description { get; set; }

    public Item() { }


    public Item(string itemName_, string description_, ItemTpye itemType_, Quality quality_, string spriteNeutral_, string spriteHighlighted_, int maxSize_)
    {
        // set the all the values from the parameter
        ItemName = itemName_;
        Description = description_;
        itemType = itemType_;
        quality = quality_;
        SpriteNeutral = spriteNeutral_;
        SpriteHighlighted = spriteHighlighted_;
        MaxSize = maxSize_;
    }

    public abstract void Use();

    public virtual string GetToolTip()
    {
        return null;
    }
}

