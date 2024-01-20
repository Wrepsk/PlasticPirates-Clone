using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Equipment")]
public class EquipmentInfo : ScriptableObject
{
    public string equipmentName;
    public int damage;
    public bool isAutomatic;
    public float cooldown;
}
