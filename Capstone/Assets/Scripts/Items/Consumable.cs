using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{

    // the consumable items
    public int Health { get; set; }
    public int Mana { get; set; }

    public Consumable() {}
    
    // call the constructor of base class(Item) and set the variables
    public Consumable(string itemName_, string description_, ItemTpye itemType_, Quality quality_, string spriteNeutral_, string spriteHighlighted_, int maxSize_, int health_, int mana_) : base(itemName_, description_, itemType_, quality_, spriteNeutral_, spriteHighlighted_, maxSize_)
    {
        
        Health = health_;
        Mana = mana_;
    }

    public override void Use()
    {

    }

    public override string GetToolTip()
    {
        return base.GetToolTip();
    }
}
