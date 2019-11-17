using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{

    // stats of the equipment 
    public int Intellect { get; set; }
    public int Agility { get; set; }
    public int Stamina { get; set; }
    public int Strength { get; set; }


    public Equipment() {}
  

    // call the constructor of base class(Item) and set the variables
    public Equipment(string itemName_, string description_, ItemTpye itemType_, Quality quality_, string spriteNeutral_, string spriteHighlighted_, int maxSize_, int intellect_, int agility_, int stamina_, int strength_) : base(itemName_, description_, itemType_, quality_, spriteNeutral_, spriteHighlighted_, maxSize_)
    {
        Intellect = intellect_;
        Agility = agility_;
        Stamina = stamina_;
        Strength = strength_;
    }


    public override void Use()
    {

    }


    public override string GetToolTip()
    {
        return base.GetToolTip();
    }
}
