using System.Collections.Generic;
using UnityEngine;

public class PNJManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject PnjPrefab;

    [SerializeField] 
    private Transform PnjSpawnOutside;

    [SerializeField]
    private int targetNumberPnj;

    [SerializeField]
    private float waitBeforeSpawnPNJ;
    
    [SerializeField]
    private PNJPool basePool;

    private List<PNJBrain> PNJList;

    private float waitPNJCounter;

    public bool HasEnoughtPNJ => PNJList.Count >= targetNumberPnj;

    private void Awake()
    {
        managerRefs.PNJManager = this;
        PNJList = new List<PNJBrain>();
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
        PNJ.transform.position = PnjSpawnOutside.position;
        PNJ.Setup(basePool.PNJPoolList.GetRandomElement());
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
