using PrimeTween;
using UnityEngine;

public static class CoreBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        PrimeTweenConfig.ManualInitialize();
        GameObject prefab = Resources.Load<GameObject>("Managers");
        GameObject instance = Object.Instantiate(prefab);
        instance.name = "[CORE MANAGERS]";
        Object.DontDestroyOnLoad(instance);
    }
}