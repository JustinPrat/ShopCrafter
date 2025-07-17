using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/PNJEvents")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "PNJEvents", message: "PNJ has an Event", category: "Events", id: "597e322f0567a9422bbf93eadf7b90d3")]
public sealed partial class PNJEvents : EventChannel { }

