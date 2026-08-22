using Sequencer.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Sequencer
{
    public class Sequencer : MonoBehaviour
    {
        [SerializeReference]
        private List<Action> actions;

        [SerializeField]
        private PlayMode playMode;

        public enum PlayMode
        {
            Manual,
            Start,
            Enable
        }

        private void Awake()
        {
            foreach (Action action in actions)
            {
                action.Behavior = action.Setup(action.ChangeTarget && action.Target != null ? action.Target : gameObject);
            }
        }

        private void OnEnable()
        {
            if (playMode == PlayMode.Enable)
            {
                StartSequence();
            }
        }

        private void Start()
        {
            if (playMode == PlayMode.Start)
            {
                StartSequence();
            }
        }

        [Button, HideInEditMode]
        public void StartSequence()
        {
            if (!isActiveAndEnabled)
                return;

            StartCoroutine(ExecuteSequence());
        }

        public void StopSequence()
        {
            bool hasReachedCurrent = false;
            for (int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i];
                if (action.IsExecuting || hasReachedCurrent) 
                {
                    hasReachedCurrent = true;
                    action.Behavior.Stop();
                }
            }
            
            StopAllCoroutines();
        }

        public IEnumerator ExecuteSequence()
        {
            foreach (Action action in actions)
            {
                action.Behavior.SetExecuteBaseValue();
            }

            for (int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i];
                if (action.type == Action.ActionType.After || i == 0)
                {
                    action.IsExecuting = true;

                    PlayRelatedJoin(i);

                    yield return StartCoroutine(action.Behavior.Execute());
                    action.IsExecuting = false;
                }
            }
        }

        private void PlayRelatedJoin(int i)
        {
            for (int j = i + 1; j < actions.Count; j++)
            {
                Action action = actions[j];

                if (action.type == Action.ActionType.After)
                    break;

                if (action.type == Action.ActionType.Join)
                    StartCoroutine(action.Behavior.Execute());
            }
        }

        [Serializable]
        public abstract class Action
        {
            public bool ChangeTarget;

            [ShowIf(nameof(ChangeTarget))]
            public GameObject Target;

            public ActionType type;

            [HideInInspector]
            public SequenceActionBehavior Behavior;

            [ReadOnly, HideInEditMode]
            public bool IsExecuting;

            public enum ActionType
            {
                After,
                Join
            }

            public abstract SequenceActionBehavior Setup(GameObject owner);
        }

        [Serializable]
        public class ActionScriptable : Action
        {
            [InlineEditor]
            public SequenceActionData ActionData;

            public override SequenceActionBehavior Setup(GameObject owner)
            {
                return ActionData.CreateBehavior(owner);
            }
        }

        [Serializable]
        public class ActionReference : Action
        {
            [SerializeReference]
            public SequenceRefActionData ActionData;

            public override SequenceActionBehavior Setup(GameObject owner)
            {
                ActionData.CreateBehavior(owner);
                return ActionData;
            }
        }
    }
}
