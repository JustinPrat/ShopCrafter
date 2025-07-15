using UnityEngine;

[CreateAssetMenu(fileName = "ManagerRefs", menuName = "ShopCrafter/ManagerRefs")]
public class ManagerRefs : ScriptableObject
{
    public SellManager SellManager { get; set; }
    public PNJManager PNJManager { get; set; }
}
