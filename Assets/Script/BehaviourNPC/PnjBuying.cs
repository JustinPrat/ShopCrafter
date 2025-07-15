using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/PNJBuying")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "PNJBuying", message: "PNJ has arrived at buying position", category: "Events", id: "597e322f0567a9422bbf93eadf7b90d3")]
public sealed partial class PnjBuying : EventChannel { }

