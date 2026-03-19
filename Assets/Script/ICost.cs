using UnityEngine;

public interface ICost
{
    public struct UIDisplayData
    {
        public string DisplayName;
        public int Amount;
        public Sprite Icon;
    }

    UIDisplayData GetDisplayData();
    public bool CanPay(ManagerRefs managerRefs);
    public void ResolveCost(ManagerRefs managerRefs);
}
