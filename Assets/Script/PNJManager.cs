using System;
using System.Collections.Generic;
using UnityEngine;

public class PNJManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject PnjPrefab;

    [SerializeField] 
    private Transform pnjSpawnOutside;

    [SerializeField]
    private int targetNumberPnj;

    [SerializeField]
    private float waitBeforeSpawnPNJ;
    
    [SerializeField]
    private List<PNJPoolElement> pnjPoolList;

    private List<PNJBrain> PNJList;
    private int currentPoolIndex;
    private float waitPNJCounter;

    private List<PNJData> PNJDataPoolList;
    private List<PNJData> PNJDataUsedList = new List<PNJData>();

    public Vector3 PnjSpawnOutside => pnjSpawnOutside.position;

    public bool HasEnoughtPNJ => PNJList.Count >= targetNumberPnj;

    [Serializable]
    private struct PNJPoolElement
    {
        public int minCraft;
        public PNJPool pnjPool;
    }

    private int PoolListCompare (PNJPoolElement a, PNJPoolElement b)
    {
        if (a.minCraft < b.minCraft)
        {
            return -1;
        }
        else if (a.minCraft == b.minCraft)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        pnjPoolList.Sort(PoolListCompare);
    }
#endif

    private void Awake()
    {
        managerRefs.PNJManager = this;
        PNJList = new List<PNJBrain>();
        PNJDataPoolList = new List<PNJData>(pnjPoolList[currentPoolIndex].pnjPool.PNJPoolList);
    }

    private void Start()
    {
        managerRefs.CraftingManager.OnItemCraft += OnItemCraft;
    }

    private void OnItemCraft(int number)
    {
        if (pnjPoolList[currentPoolIndex].minCraft < number && currentPoolIndex < pnjPoolList.Count -1)
        {
            currentPoolIndex += 1;
            PNJDataPoolList = new List<PNJData>(pnjPoolList[currentPoolIndex].pnjPool.PNJPoolList);
        }
    }

    private void Update()
    {
        if (!HasEnoughtPNJ)
        {
            waitPNJCounter += Time.deltaTime;

            if (waitPNJCounter >= waitBeforeSpawnPNJ)
            {
                waitPNJCounter = 0;
                SpawnPNJ();
            }
        }
    }

    private void SpawnPNJ ()
    {
        PNJBrain PNJ = Instantiate(PnjPrefab).GetComponent<PNJBrain>();
        PNJ.transform.position = PnjSpawnOutside;

        PNJData data = PNJDataPoolList.GetRandomElement();
        PNJDataPoolList.Remove(data);
        PNJDataUsedList.Add(data);

        if (PNJDataPoolList.Count <= 0)
        {
            PNJDataPoolList.AddRange(PNJDataUsedList);
        }

        PNJ.Setup(data);
    }

    public void AddPnj (PNJBrain pnj)
    {
        PNJList.Add(pnj);
    }

    public void RemovePnj (PNJBrain pnj)
    {
        PNJList.Remove(pnj);
    }
}
