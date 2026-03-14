using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PNJManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

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

    [SerializeField]
    private int maxSpecialPerDay = 1;

    private List<PNJBrain> PNJList;
    private List<PNJBrain> NeedRespawnPNJList = new List<PNJBrain>();
    private int currentPoolIndex;
    private float waitPNJCounter;

    private List<PNJInfoData> PNJDataPoolList;
    private List<PNJInfoData> PNJDataUsedList = new List<PNJInfoData>();

    private List<PNJInfoData> SpecialPNJDataPoolList;
    private List<PNJInfoData> SpecialPNJDataUsedList = new List<PNJInfoData>();

    private float currentDayTime;
    private DateTime currentHourDayTime;
    private int dayIndex;
    private bool isTimePaused;
    private bool isNearDayEndEventTriggered;
    private DayTime dayTime;
    private int numberSpecialSpawnedToday;
    private float nearEndDayDuration => (nearDayEndTime / dayEndTime) * dayDuration;

    private enum DayTime
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

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
        SpecialPNJDataPoolList = new List<PNJInfoData>(pnjPoolList[currentPoolIndex].pnjPool.SpecialPNJPool);
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
            SpecialPNJDataPoolList = new List<PNJInfoData>(pnjPoolList[currentPoolIndex].pnjPool.SpecialPNJPool);
        }
    }

    private void Update()
    {
        if (!HasEnoughtPNJ && !isNearDayEndEventTriggered)
        {
            waitPNJCounter += Time.deltaTime;

            if (waitPNJCounter >= waitBeforeSpawnPNJ)
            {
                waitPNJCounter = 0;
                SpawnPNJ();
            }
        }
       
        if (!isTimePaused && dayTime != DayTime.Night)
        {
            currentDayTime += Time.deltaTime;
            if (currentDayTime >= dayDuration)
            {
                EndDay();
            }

            if (currentDayTime >= nearEndDayDuration && !isNearDayEndEventTriggered)
            {
                NearDayEnd();
            }

            currentHourDayTime = currentHourDayTime.AddSeconds((Time.deltaTime / dayDuration) * ((dayEndTime - dayStartTime) * 3600f));
            daytime.text = "date : " + currentHourDayTime.Day + " - " + currentHourDayTime.Hour + "h" + currentHourDayTime.Minute;
        }
    }

    public void StartDay ()
    {
        dayTime = DayTime.Morning;
        currentDayTime = 0;
        dayIndex++;
        isNearDayEndEventTriggered = false;
        numberSpecialSpawnedToday = 0;

        currentHourDayTime = new DateTime(1, 1, dayIndex, dayStartTime, 0, 0);
        managerRefs.GameEventsManager.dayEvents.StartDay();
    }

    private void EndDay ()
    {
        dayTime = DayTime.Night;
        managerRefs.GameEventsManager.dayEvents.EndDay();
    }

    private void NearDayEnd()
    {
        dayTime = DayTime.Evening;
        isNearDayEndEventTriggered = true;
        managerRefs.GameEventsManager.dayEvents.NearEndDay();
    }

    private void SpawnPNJ ()
    {
        PNJBrain PNJ = null;
        if (NeedRespawnPNJList.Count > 0)
        {
            PNJ = NeedRespawnPNJList[0];
            if (PNJ.Data.IsSpecial)
            {
                numberSpecialSpawnedToday++;
            }

            PNJ.gameObject.SetActive(true);
            PNJ.ChangeState(State.RoamingAround);
            NeedRespawnPNJList.Remove(PNJ);
        }
        else
        {
            bool spawnSpecial = numberSpecialSpawnedToday < maxSpecialPerDay && SpecialPNJDataPoolList.Count > 0;

            List<PNJInfoData> PNJList = PNJDataPoolList;
            List<PNJInfoData> PNJUsedList = PNJDataUsedList;

            if (spawnSpecial)
            {
                PNJList = SpecialPNJDataPoolList;
                PNJUsedList = SpecialPNJDataUsedList;
            }

            PNJInfoData PNJData = PNJList.GetRandomElement();
            PNJList.Remove(PNJData);
            PNJUsedList.Add(PNJData);

            PNJ = Instantiate(PNJData.PnjPrefab).GetComponent<PNJBrain>();
            PNJ.Setup(PNJData);

            if (PNJList.Count <= 0 && !spawnSpecial)
            {
                PNJList.AddRange(PNJUsedList);
            }
        }
        
        if (PNJ == null)
            return;

        PNJList.Add(PNJ);
        PNJ.transform.position = PnjSpawnOutside;
    }

    public void RemovePnj (PNJBrain pnj)
    {
        PNJList.Remove(pnj);

        if (pnj.ShouldReturn && !NeedRespawnPNJList.Contains(pnj))
        {
            NeedRespawnPNJList.Add(pnj);
            pnj.gameObject.SetActive(false);
        }
        else
        {
            Destroy(pnj.gameObject);
        }
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
