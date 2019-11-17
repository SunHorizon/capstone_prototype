using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{
    // attack speed of weapon
    // TODO: Can add more variables 
    public float AttackSpeed { get; set; }
  
    public Weapon() {}

    // call the constructor of base class(Item) and set the variables
    public Weapon(string itemName_, string description_, ItemTpye itemType_, Quality quality_, string spriteNeutral_, string spriteHighlighted_, int maxSize_, int intellect_, int agility_, int stamina_, int strength_, float attackSpeed_) : 
        base(itemName_, description_, itemType_, quality_, spriteNeutral_, spriteHighlighted_, maxSize_, intellect_, agility_, stamina_, strength_)
    {
        AttackSpeed = attackSpeed_;
    }

    public override void Use()
    {

    }

    public override string GetToolTip()
    {
        return base.GetToolTip();
    }
}
