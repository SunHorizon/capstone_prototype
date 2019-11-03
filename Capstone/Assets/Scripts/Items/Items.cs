using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTpye {Mana, Health, Sword}

public enum Quality { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, ARTIFACT}

public class Items : MonoBehaviour
{
    // Iteam type
    public ItemTpye type;

    // quality of the item
    public Quality quality;

    // Non-selected sprite
    [SerializeField] private Sprite spriteNeutral;

    // Selected sprite
    [SerializeField] private Sprite spriteHighlighted;

    // Max number items can stack
    public int maxSize;

    // stats could change. is this only temp
    public float Strength, intellect, agility, stamina;

    // item name
    public string itemName;

    // description of the item
    public string description;


    // use the item method
    public void Use()
    {
        switch (type)
        {
            case ItemTpye.Mana:
                Debug.Log("Mana Used"); // for testing
                break;
            case ItemTpye.Health:
                Debug.Log("Health Used"); // for testing
                break;
            case ItemTpye.Sword:
                Debug.Log("Sword Pick Up"); // for testing
                break;
        }
    }
    
    // getters for the sprite
    public Sprite GetSpriteNeutral()
    {
        return spriteNeutral;
    }

    public Sprite GetSpriteHighlighted()
    {
        return spriteHighlighted;
    }

    // Tool tip text
    public string GetToolTip()
    {
        string stats = string.Empty;
        string color = string.Empty;
        string newLine = string.Empty;

        if(description != string.Empty)
        {
            newLine = "\n";
        }

        // set the color of the name based on the quality
        switch (quality)
        {
            case Quality.COMMON:
                color = "white";
                break;
            case Quality.UNCOMMON:
                color = "lime";
                break;
            case Quality.RARE:
                color = "navy";
                break;
            case Quality.EPIC:
                color = "magenta";
                break;
            case Quality.LEGENDARY:
                color = "orange";
                break;
            case Quality.ARTIFACT:
                color = "red";
                break;
        }

        // set the sats 
        if (Strength > 0)
        {
            stats += "\n+" + Strength.ToString() + " Strength";
        }
        if (intellect > 0)
        {
            stats += "\n+" + intellect.ToString() + " Intellect";
        }
        if (agility > 0)
        {
            stats += "\n+" + agility.ToString() + " Agility";
        }
        if (stamina > 0)
        {
            stats += "\n+" + stamina.ToString() + " Stamina";
        }

        // the format for the tool tip
        return string.Format("<color=" + color + "><size=14>{0}</size></color><size=12><i><color=lime>" + newLine + "{1}</color></i>{2}</size>", itemName, description, stats);
    }
}
