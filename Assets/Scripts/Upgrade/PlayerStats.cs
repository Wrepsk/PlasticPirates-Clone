using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int health = 100;
    public int armor = 100;
    public int speed = 5;


    private void Awake()
    {
        instance = this;
    }
}
