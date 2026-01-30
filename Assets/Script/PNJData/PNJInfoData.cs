using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopCrafter/PNJInfo")]
public class PNJInfoData : ScriptableObject
{
    public IdentityData IdentityInfo;
    public List<PNJTraitData> TraitDatas;

    public float ShopStayDuration;
    public ManagerRefs ManagerRefs;

    public PNJRuntimeData GetRuntimeData()
    {
        PNJRuntimeData runtimeData = new PNJRuntimeData();
        runtimeData.Identity = IdentityInfo.GetIdentity();
        foreach (PNJTraitData traitData in TraitDatas)
        {
            runtimeData.ActiveTraits.Add(traitData.GetRuntimeLogic());
        }

        runtimeData.ShopStayDuration = ShopStayDuration;
        runtimeData.ManagerRefs = ManagerRefs;
        return runtimeData;
    }
}

[Serializable]
public class PNJRuntimeData
{
    public Identity Identity;
    public List<IPNJTraitRuntime> ActiveTraits = new List<IPNJTraitRuntime>();

    public float ShopStayDuration;
    public ManagerRefs ManagerRefs;
}
