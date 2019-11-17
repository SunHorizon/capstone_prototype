using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using UnityEngine;

public enum Category { EQUIPMENT, WEAPON, CONSUMABLE}

public class ItemManager : MonoBehaviour
{
    // all the enums   
    public ItemTpye itemtype;
    public Quality quality;
    public Category category;

    // item variables
    public string spriteNeutral;
    public string spriteHighlighted;
    public string itemName;
    public string description;

    public int maxSize;

    // Equipment variables
    public int intellect;
    public int agility;
    public int stamina;
    public int strength;

    // Weapon variables
    public int attackSpeed;

    // consumable variables
    public int health;
    public int mana;


    public void CreateItem()
    {
        // create the item 
        ItemContainer itemContainer = new ItemContainer();

        // make the item type
        Type[] itemTypes = {typeof(Equipment), typeof(Weapon), typeof(Consumable)};

        // make the file stream
        FileStream fs = new FileStream(Path.Combine(Application.streamingAssetsPath, "Items.xml"), FileMode.Open);

        // make the serializer 
        XmlSerializer serializer = new XmlSerializer(typeof(ItemContainer), itemTypes);

        //deserialize the xml file into the container
        itemContainer = (ItemContainer) serializer.Deserialize(fs);

        // Serialize the serializer
        serializer.Serialize(fs, itemContainer);

        // close the stream
        fs.Close();

        // add the item 
        switch (category)
        {
            case Category.EQUIPMENT:
                itemContainer.Equipment.Add(new Equipment(itemName, description, itemtype, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, stamina, strength));
                break;
            case Category.WEAPON:
                itemContainer.Weapons.Add(new Weapon(itemName, description, itemtype, quality, spriteNeutral, spriteHighlighted, maxSize, intellect, agility, stamina, strength, attackSpeed));
                break;
            case Category.CONSUMABLE:
                itemContainer.Weapons.Add(new Consumable(itemName, description, itemtype, quality, spriteNeutral, spriteHighlighted, maxSize, health, mana));
                break;
        }

        // put the item container back into the file
        fs = new FileStream(Path.Combine(Application.streamingAssetsPath, "Items.xml"), FileMode.Create);
        serializer.Serialize(fs, itemContainer);
        fs.Close();
    }

}
