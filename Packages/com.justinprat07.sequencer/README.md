# Unity Sequencer

A lightweight and extensible action sequencing system for Unity. This tool allows you to chain behaviors together using ScriptableObjects, enabling complex sequences with minimal coding.

Package Manager :
Add with Git - https://github.com/JustinPrat/Unity-Sequencer.git

## Setup

1. Attach the `Sequencer.cs` script to any GameObject in your scene.
2. Create Action Assets:
   - Right-click in the **Project Window**.
   - Navigate to `Create` -> `Sequencer` -> `[YourActionName]Data`.
3. Configure the Sequencer:
   - Select the GameObject containing the `Sequencer` component.
   - Drag and drop your new `SequenceActionData` assets into the list in the Inspector.

## Usage

### From Code
To trigger a sequence via script, call the `Play()` method on your `Sequencer` reference:

```csharp
public Sequencer mySequencer;

void Start()
{
    mySequencer.Play();
}
```

### In Editor
You can test sequences directly in the Unity Editor by clicking the **Play** button at runtime (Editor-only support).

## How to Extend

The system is designed to be modular. To create a custom action, follow these steps.

### 1. Create the Data Asset (SequenceActionData)
Create a script that inherits from `SequenceActionData`. This allows you to create new configuration assets in the Project window and links the data to its behavior.

```csharp
using System.Collections;
using UnityEngine;

namespace Sequencer.Actions
{
    [CreateAssetMenu(fileName = "MovePositionActionData", menuName = "Sequencer/MovePositionActionData")]
    public class MovePositionActionData : SequenceActionData
    {
        public Vector3 MoveOffset;
        public float Duration;

        public override SequenceActionBehavior CreateBehavior(GameObject owner)
        {
            return new MovePositionActionBehavior(owner, this);
        }
    }
}
```

### 2. Implement the Behavior (SequenceActionBehavior)
Create a class that inherits from `SequenceActionBehavior`. This is where the actual logic runs.

- `Setup()` is called during the Sequencer's `Awake()`.
- `Execute()` is a coroutine and must `yield` until the action is complete.

```csharp
public class MovePositionActionBehavior : SequenceActionBehavior
{
    private MovePositionActionData data;
    private float timer;
    private Vector3 basePos;

    // Constructor: pass the data to access parameters from the ScriptableObject
    public MovePositionActionBehavior(GameObject owner, MovePositionActionData data) : base(owner)
    {
        this.data = data;
    }

    public override void Setup()
    {
        timer = 0;
    }

    public override IEnumerator Execute()
    {
        timer = 0;
        basePos = owner.transform.position;

        while (timer < data.Duration)
        {
            timer += Time.deltaTime;
            owner.transform.position = Vector3.LerpUnclamped(basePos, basePos + data.MoveOffset, timer / data.Duration);
            yield return null;
        }

        owner.transform.position = basePos + data.MoveOffset;
    }
}
```

## Action Chaining

The execution flow is handled inside `Sequencer.cs` using the `ActionType`:

| Type  | Description |
| :---- | :---------- |
| After | Plays the action, then plays all `Join` actions after it. |
| Join  | Plays alongside the `After` before it and does not hold execution. |
