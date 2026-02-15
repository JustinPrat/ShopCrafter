using System;
using UnityEngine;

public class DayEvents
{
    public event Action OnPauseDay;
    public void PauseDay()
    {
        if (OnPauseDay != null)
        {
            OnPauseDay.Invoke();
        }
    }

    public event Action OnResumeDay;
    public void ResumeDay()
    {
        if (OnResumeDay != null)
        {
            OnResumeDay.Invoke();
        }
    }
    
    public event Action OnStartDay;
    public void StartDay()
    {
        if (OnStartDay != null)
        {
            OnStartDay.Invoke();
        }
    }

    public event Action OnEndDay;
    public void EndDay()
    {
        if (OnEndDay != null)
        {
            OnEndDay.Invoke();
        }
    }

    public event Action OnNearEndDay;
    public void NearEndDay()
    {
        if (OnNearEndDay != null)
        {
            OnNearEndDay.Invoke();
        }
    }
}
