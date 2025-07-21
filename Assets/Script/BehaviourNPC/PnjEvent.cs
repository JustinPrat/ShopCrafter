using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/PNJEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "PNJEvent", message: "[Agent] has an event", category: "Events", id: "63d084aaa5ff4ec95d9d7506edb1373a")]
public sealed partial class PnjEvent : EventChannel<GameObject> { }

