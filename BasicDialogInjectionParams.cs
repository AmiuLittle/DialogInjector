using System;
using System.Collections.Generic;
using HarmonyLib;

namespace AmiuLittle.DialogInjector;

public class BasicDialogInjectionParams(string targetNPCNameSubstr, CustomDialogBranch[] customDialogBranches, BasicDialogInjection[] injections) : IDialogInjectionParams
{
    public string targetNPCNameSubstr = targetNPCNameSubstr;
    public CustomDialogBranch[] customDialogBranches = customDialogBranches;
    public BasicDialogInjection[] injections = injections;
    private Dictionary<int, Dictionary<int, int>> IdsToInjectionIdx =
        new Dictionary<int, Dictionary<int, int>>();

    public bool IsTargetNPC(NetNPC npc)
    {
        return npc.name.Contains(targetNPCNameSubstr);
    }

    public IEnumerable<CustomDialogBranch> GetCustomDialogBranches(NetNPC _, ScriptableDialogData _1)
    {
        return customDialogBranches;
    }

    public bool IsTargetDialog(int branchIdx, int dialogIdx, NetNPC npc, ScriptableDialogData dialogData, DialogBranch dialogBranch, Dialog dialog, out IDialogInjectionParams.DialogInjector injector)
    {
        injector = BasicInjector;
        bool output = false;
        int i;
        for (i = 0; i < injections.Length; i++)
        {
            BasicDialogInjection injection = injections[i];
            if (injection.searchMethod == DialogInjectionSearchMethod.SUBSTR)
            {
                if (dialog._dialogInput.Contains(injection.searchSubStr))
                {
                    output = true;
                    break;
                }
                foreach (string altInput in dialog._altInputs)
                {
                    if (altInput.Contains(injection.searchSubStr))
                    {
                        output = true;
                        break;
                    }
                }
            }
            else if (injection.searchMethod == DialogInjectionSearchMethod.INDEX)
            {
                if (branchIdx == injection.branchIdx && dialogIdx == injection.dialogIdx)
                {
                    output = true;
                    break;
                }
            }
            else if (injection.searchMethod == DialogInjectionSearchMethod.INSTANCE)
            {
                DialogInjectionManager.ExtraVanillaDialogInfo extraInfo = DialogInjectionManager.ExtraDialogInfo.GetOrCreateValue(dialog);
                if (extraInfo.baseCustomDialogInstance != null && Object.ReferenceEquals(injection.customDialogInstance, extraInfo.baseCustomDialogInstance)) {
                    output = true;
                    break;
                }
            }
            else if (injection.searchMethod == DialogInjectionSearchMethod.CUSTOM)
            {
                if (injection.customSearch(branchIdx, dialogIdx, npc, dialogData, dialogBranch, dialog))
                {
                    output = true;
                    break;
                }
            }
        }

        if (output)
        {
            if (!IdsToInjectionIdx.ContainsKey(branchIdx)) IdsToInjectionIdx[branchIdx] = new Dictionary<int, int>();
            IdsToInjectionIdx[branchIdx][dialogIdx] = i;
        }
        return output;
    }

    void BasicInjector(int branchIdx, int dialogIdx, NetNPC npc, ScriptableDialogData dialogData, DialogBranch dialogBranch, ref Dialog dialog, out IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector)
    {
        dynamicDialogInjector = null;
        BasicDialogInjection injection = injections[IdsToInjectionIdx[branchIdx][dialogIdx]];

        if (injection.injectionMethod == DialogInjectionMethod.REPLACE)
        {
            dialog = DialogInjectionManager.FlattenCustomDialog(dialogData, injection.replacement);
        }
        else if (injection.injectionMethod == DialogInjectionMethod.ADD_SELECTIONS)
        {
            foreach (CustomDialogSelection customDialogSelection in injection.newDialogSelections)
            {
                dialog._dialogSelections = dialog._dialogSelections.AddToArray(DialogInjectionManager.FlattenCustomDialogSelection(dialogData, customDialogSelection));
            }
        }
        else if (injection.injectionMethod == DialogInjectionMethod.DYNAMIC)
        {
            dynamicDialogInjector = injection.dynamicDialogInjector;
        }
    }
}