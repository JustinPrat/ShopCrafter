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
    private PNJPool basePool;

    private List<PNJBrain> PNJList;

    private float waitPNJCounter;

    private List<PNJData> PNJDataPoolList;
    private List<PNJData> PNJDataUsedList = new List<PNJData>();

    public Vector3 PnjSpawnOutside => pnjSpawnOutside.position;

    public bool HasEnoughtPNJ => PNJList.Count >= targetNumberPnj;

    private void Awake()
    {
        managerRefs.PNJManager = this;
        PNJList = new List<PNJBrain>();
        PNJDataPoolList = new List<PNJData>(basePool.PNJPoolList);
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
