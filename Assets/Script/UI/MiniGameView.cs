using System.Collections.Generic;
using UnityEngine;

public class MiniGameView : UIView
{
    [SerializeField]
    private ManagerRefs managerRefs;

    private List<Item> items;

    public override void Toggle(bool isOn)
    {
        base.Toggle(isOn);
    }

    public void Setup (List<Item> itemConsumed)
    {
        items = itemConsumed;
    }
}
