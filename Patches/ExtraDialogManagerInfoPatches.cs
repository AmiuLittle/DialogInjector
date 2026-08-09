using System;
using HarmonyLib;

namespace AmiuLittle.DialogInjector.Patches;

[HarmonyPatch(typeof(DialogManager), nameof(DialogManager.Start_Dialog))]
public static class ExtraDialogManagerInfoStartDialog
{
    [HarmonyPostfix]
    static void Posfix(DialogManager __instance, DialogBranch _dialogBranch, DialogTrigger _dialogTrigger)
    {
        if (_dialogTrigger._scriptDialogData != null)
        {
            for (int i = 0; i < _dialogTrigger._scriptDialogData._dialogBranches.Length; i++)
            {
                if (Object.ReferenceEquals(_dialogBranch, _dialogTrigger._scriptDialogData._dialogBranches[i]))
                {
                    DialogInjectionManager.ExtraDialogManagerInfo.GetOrCreateValue(__instance).currentDialogBranchIdx = i;
                    break;
                }
            }
        }
    }
}

[HarmonyPatch(typeof(DialogManager), nameof(DialogManager.End_Dialog))]
public static class ExtraDialogManagerInfoEndDialog
{
    [HarmonyPostfix]
    static void Posfix(DialogManager __instance)
    {
        DialogInjectionManager.ExtraDialogManagerInfo.GetOrCreateValue(__instance).currentDialogBranchIdx = -1; 
    }
}