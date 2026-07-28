using Sequencer.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using static Sequencer.Actions.SequenceActionData;

namespace Sequencer
{
    public class Sequencer : MonoBehaviour
    {
        [SerializeField]
        private List<Action> actions;

        private void Awake()
        {
            foreach (Action action in actions)
            {
                action.Behavior = action.ActionData.CreateBehavior(gameObject);
                action.Behavior.Setup();
            }
        }

        [Button, HideInEditMode]
        public void StartSequence()
        {
            StartCoroutine(ExecuteSequence());
        }

        private IEnumerator ExecuteSequence()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i];
                if (action.type == Action.ActionType.After || i == 0)
                {
                    action.IsExecuting = true;

                    for (int j = i + 1; j < actions.Count; j++)
                    {
                        if (action.type == Action.ActionType.After)
                            break;

                        if (action.type == Action.ActionType.Join)
                            StartCoroutine(action.Behavior.Execute());
                    }

                    yield return StartCoroutine(action.Behavior.Execute());
                    action.IsExecuting = false;
                }
            }
        }

        [Serializable]
        public class Action
        {
            public ActionType type;
            [InlineEditor]
            public SequenceActionData ActionData;

            [HideInInspector]
            public SequenceActionBehavior Behavior;

            [ReadOnly, HideInEditMode]
            public bool IsExecuting;

            public enum ActionType
            {
                After,
                Join
            }
        }
    }
}
