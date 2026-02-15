using System;
using System.Collections.Generic;
using TMPro;
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
    private float dayDuration;

    [SerializeField] 
    private int dayStartTime;

    [SerializeField] 
    private int dayEndTime;

    [SerializeField] 
    private float nearDayEndTime;

    [SerializeField]
    private int targetNumberPnj;

    [SerializeField]
    private float waitBeforeSpawnPNJ;
    
    [SerializeField]
    private List<PNJPoolElement> pnjPoolList;

    [SerializeField]
    private TextMeshProUGUI daytime;

    private List<PNJBrain> PNJList;
    private int currentPoolIndex;
    private float waitPNJCounter;

    private List<PNJInfoData> PNJDataPoolList;
    private List<PNJInfoData> PNJDataUsedList = new List<PNJInfoData>();

    private float currentDayTime;
    private DateTime currentHourDayTime;
    private int dayIndex;
    private bool isTimePaused;
    private bool isNearDayEndEventTriggered;

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
        PNJDataPoolList = new List<PNJInfoData>(pnjPoolList[currentPoolIndex].pnjPool.PNJPoolList);
    }

    private void Start()
    {
        StartDay();
        managerRefs.CraftingManager.OnItemCraft += OnItemCraft;
        managerRefs.GameEventsManager.dayEvents.OnPauseDay += PauseDayCounter;
        managerRefs.GameEventsManager.dayEvents.OnResumeDay += ResumeDayCounter;
    }

    private void OnItemCraft(int number)
    {
        if (pnjPoolList[currentPoolIndex].minCraft < number && currentPoolIndex < pnjPoolList.Count -1)
        {
            currentPoolIndex += 1;
            PNJDataPoolList = new List<PNJInfoData>(pnjPoolList[currentPoolIndex].pnjPool.PNJPoolList);
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
       
        if (!isTimePaused)
        {
            currentDayTime += Time.deltaTime;
            if (currentDayTime >= dayDuration)
            {
                EndDay();
            }

            if (currentDayTime >= nearDayEndTime && !isNearDayEndEventTriggered)
            {
                NearDayEnd();
            }

            currentHourDayTime = currentHourDayTime.AddSeconds((Time.deltaTime / dayDuration) * ((dayEndTime - dayStartTime) * 3600f));
            daytime.text = "date : " + currentHourDayTime.Day + " - " + currentHourDayTime.Hour + "h" + currentHourDayTime.Minute;
        }
    }

    private void StartDay ()
    {
        currentDayTime = 0;
        dayIndex++;

        currentHourDayTime = new DateTime(1, 1, dayIndex, dayStartTime, 0, 0);
        managerRefs.GameEventsManager.dayEvents.StartDay();
    }

    private void EndDay ()
    {
        managerRefs.GameEventsManager.dayEvents.EndDay();
        StartDay();
    }

    private void NearDayEnd()
    {
        isNearDayEndEventTriggered = true;
        managerRefs.GameEventsManager.dayEvents.NearEndDay();
    }

    private void SpawnPNJ ()
    {
        PNJBrain PNJ = Instantiate(PnjPrefab).GetComponent<PNJBrain>();
        PNJ.transform.position = PnjSpawnOutside;

        PNJInfoData PNJData = PNJDataPoolList.GetRandomElement();
        PNJDataPoolList.Remove(PNJData);
        PNJDataUsedList.Add(PNJData);

        if (PNJDataPoolList.Count <= 0)
        {
            PNJDataPoolList.AddRange(PNJDataUsedList);
        }

        PNJ.Setup(PNJData);
    }

    public void AddPnj (PNJBrain pnj)
    {
        PNJList.Add(pnj);
    }

    public void RemovePnj (PNJBrain pnj)
    {
        PNJList.Remove(pnj);
    }
    private void PauseDayCounter()
    {
        isTimePaused = true;
    }

    private void ResumeDayCounter()
    {
        isTimePaused = false;
    }

    private void OnDestroy()
    {
        if (managerRefs.CraftingManager != null)
        {
            managerRefs.CraftingManager.OnItemCraft -= OnItemCraft;
        }

        if (managerRefs.GameEventsManager != null)
        {
            managerRefs.GameEventsManager.dayEvents.OnPauseDay -= PauseDayCounter;
            managerRefs.GameEventsManager.dayEvents.OnResumeDay -= ResumeDayCounter;
        }
    }
}
