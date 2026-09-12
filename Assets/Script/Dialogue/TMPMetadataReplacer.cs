using System.Collections.Generic;
using TMPEffects.Databases;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "TMPMetadataReplacer", menuName = "ShopCrafter/TMPCommand/TMPMetadataReplacer")]
    public class TMPMetadataReplacer : TMPCommand
    {
        [SerializeField]
        private ManagerRefs managerRefs;

        private class Data
        {
            public Value ValueName;
            public enum Value
            {
                DayTime,
                Reputation
            }
        }

        public override TagType TagType => TagType.Block;

        public override bool ExecuteInstantly => true;

        public override bool ExecuteOnSkip => false;

        public override void ExecuteCommand(ICommandContext context)
        {
            Data data = (Data)context.CustomData;
            switch (data.ValueName)
            {
                case Data.Value.DayTime:
                    context.Writer.SetText(ReplaceTextAtIndices(context, managerRefs.PNJManager.CurrentDayPeriod.ToString()));
                    break;

                case Data.Value.Reputation:
                    context.Writer.SetText(ReplaceTextAtIndices(context, managerRefs.MilestoneManager.CurrentMilestoneReputation.ToString()));
                    break;

                default:
                    break;
            }
        }

        private string ReplaceTextAtIndices(ICommandContext context, string newText)
        {
            string textBase = context.Writer.TextComponent.text;
            textBase = textBase.Remove(context.Indices.StartIndex, context.Indices.Length + 3);
            textBase = textBase.Insert(context.Indices.StartIndex, newText);

            return textBase;
        }

        public override object GetNewCustomData()
        {
            return new Data();
        }

        public override void SetParameters(object obj, IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            Data data = (Data)obj;

            foreach (KeyValuePair<string, string> parameterValue in parameters)
            {
                switch (parameterValue.Value.ToLower())
                {
                    case "daytime":
                        data.ValueName = Data.Value.DayTime;
                        break;

                    case "reputation":
                        data.ValueName = Data.Value.Reputation;
                        break;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
            {
                return false;
            }

            foreach (KeyValuePair<string, string> parameterValue in parameters)
            {
                switch (parameterValue.Value)
                {
                    case "daytime":
                        break;

                    case "reputation":
                        break;

                    default:
                        return false;
                }
            }

            return true;
        }
    }
}
