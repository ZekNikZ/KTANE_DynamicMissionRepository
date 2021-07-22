using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMG.Parser
{
    abstract class DMGAction
    {
        public abstract override string ToString();
    }

    class NoopAction : DMGAction
    {
        public override string ToString()
        {
            return "Action[Noop]";
        }
    }

    class SetSettingAction : DMGAction
    {
        public DMGSetting Setting { get; }

        public object Value { get; }

        public SetSettingAction(DMGSetting setting, object value)
        {
            Setting = setting;
            Value = value;
        }

        public override string ToString()
        {
            return $"Action[SetSetting]({Setting.Name}={Value})";
        }
    }

    class AddModulePoolAction : DMGAction
    {
        public bool ReducedDuplicates { get; }

        public int Count { get; }

        public List<string> ModuleIDs { get; }

        public AddModulePoolAction(bool reducedDuplicates, int count, List<string> moduleIDs)
        {
            ReducedDuplicates = reducedDuplicates;
            Count = count;
            ModuleIDs = moduleIDs;
        }

        public AddModulePoolAction(bool reducedDuplicates, int count, params string[] moduleIDs)
        {
            ReducedDuplicates = reducedDuplicates;
            Count = count;
            ModuleIDs = moduleIDs.ToList();
        }

        public override string ToString()
        {
            return $"Action[ModulePool]({(ReducedDuplicates ? "!" : "")}{Count}*[{string.Join(",", ModuleIDs.ToArray())}])";
        }
    }

    class BombDefinitionAction: DMGAction
    {
        public bool ReducedDuplicates { get; }

        public int Count { get; }

        public List<DMGAction> Actions { get; }

        public BombDefinitionAction(bool reducedDuplicates, int count, List<DMGAction> actions)
        {
            ReducedDuplicates = reducedDuplicates;
            Count = count;
            Actions = actions;
        }

        public override string ToString()
        {
            return $"Action[BombDefinition]({(ReducedDuplicates ? "!" : "")}{Count}*{{{string.Join(",", Actions.Select(action => action.ToString()).ToArray())}}})";
        }
    }
}
