using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AmiuLittle.DialogInjector.Patches;

[HarmonyPatch(typeof(DialogManager), "Display_NextSentence")]
public static class DynamicDialogInjectionProcessor
{
    private static AccessTools.FieldRef<DialogManager, int> GetDialogIdx =
        AccessTools.FieldRefAccess<DialogManager, int>("_dialogIndex");
    private static AccessTools.FieldRef<DialogManager, Queue<string>> GetDialogSentences =
        AccessTools.FieldRefAccess<DialogManager, Queue<string>>("_dialogSentences");
    private static AccessTools.FieldRef<Queue<string>, string[]> GetQueueArray =
        AccessTools.FieldRefAccess<Queue<string>, string[]>("_array");
    private static AccessTools.FieldRef<Queue<string>, int> GetQueueHeadIndex =
        AccessTools.FieldRefAccess<Queue<string>, int>("_head");

    static void DynamicInject(DialogManager __instance)
    {
        ScriptableDialogData dialogData = __instance._scriptableDialog;
        DialogInjectionManager.ExtraScriptableDialogDataInfo extraDialogDataInfo = DialogInjectionManager.ExtraDialogDataInfo.GetOrCreateValue(dialogData);
        if (extraDialogDataInfo.dynamicInjections == null)
        {
            return;
        }
        DialogInjectionManager.ExtraVanillaDialogManagerInfo extraDialogManagerInfo = DialogInjectionManager.ExtraDialogManagerInfo.GetOrCreateValue(__instance);

        int branchIdx = extraDialogManagerInfo.currentDialogBranchIdx;
        Dictionary<int, List<IDialogInjectionParams.DynamicDialogInjector>> dict = extraDialogDataInfo.dynamicInjections.GetValueOrDefault(branchIdx, null);
        if (dict == null) return;
        int dialogIdx = GetDialogIdx(__instance);
        List<IDialogInjectionParams.DynamicDialogInjector> list = dict.GetValueOrDefault(dialogIdx, null);
        if (list == null) return;

        foreach (IDialogInjectionParams.DynamicDialogInjector injection in list)
        {
            string text = injection.Invoke(branchIdx, dialogIdx, __instance._cachedNpc, Player._mainPlayer, dialogData, dialogData._dialogBranches[branchIdx], dialogData._dialogBranches[branchIdx].dialogs[dialogIdx]);
            if (text != null)
            {
                Queue<string> dialogSentences = GetDialogSentences(__instance);
                if (dialogSentences.Count > 0)
                {
                    string[] array = GetQueueArray(dialogSentences);
                    int head = GetQueueHeadIndex(dialogSentences);
                    array[head] = text;
                }
            }
        }
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        MethodInfo dynamicInject = AccessTools.Method(typeof(DynamicDialogInjectionProcessor), nameof(DynamicDialogInjectionProcessor.DynamicInject));

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Stfld && codes[i].operand is FieldInfo field && field.Name == "_currentDialog")
            {
                codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                codes.Insert(i + 2, new CodeInstruction(OpCodes.Call, dynamicInject));
                break;
            }
        }

        return codes;
    }
}