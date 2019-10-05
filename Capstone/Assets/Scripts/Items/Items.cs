using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTpye {Mana, Health}

public class Items : MonoBehaviour
{
    // Iteam type
    public ItemTpye type;

    // Non-selected sprite
    [SerializeField] private Sprite spriteNeutral;

    // Selected sprite
    [SerializeField] private Sprite spriteHighlighted;

    // Max number items can stack
    public int maxSize;

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

}
