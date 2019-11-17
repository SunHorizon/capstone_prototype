using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer
{
    // list of diffrenweapons, consumable, and equipment
    private List<Item> weapons = new List<Item>();
    private List<Item> consumable = new List<Item>();
    private List<Item> equipment = new List<Item>();

    // getter and setter for Weapons list
    public List<Item> Weapons
    {
        get { return weapons; }
        set { weapons = value; }
    }

    // getter and setter for Consumable list
    public List<Item> Consumable
    {
        get { return consumable; }
        set { consumable = value; }
    }

    // getter and setter for Equipment list
    public List<Item> Equipment
    {
        get { return equipment; }
        set { equipment = value; }
    }

    public ItemContainer() {}

}
