using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    [SerializeField] 
    private ManagerRefs managerRefs;

    public List<SellSlot> SellSlots = new List<SellSlot>();

    private void Awake()
    {
        managerRefs.SellManager = this;
    }
}
