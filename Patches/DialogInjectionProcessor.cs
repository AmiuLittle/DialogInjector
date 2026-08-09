using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;

namespace AmiuLittle.DialogInjector.Patches;

[HarmonyPatch(typeof(NetNPC), "Awake")]
public static class DialogInjectionProcessor {
    private static ManualLogSource logger = Logger.CreateLogSource("DialogInjection");

    private static AccessTools.FieldRef<NetNPC, DialogTrigger> GetDialogTrigger =
        AccessTools.FieldRefAccess<NetNPC, DialogTrigger>("_dialogTrigger");

    [HarmonyPostfix]
    static void Postfix(NetNPC __instance)
    {
        logger.LogInfo($"Processing NPC: {__instance.name}");
        DialogTrigger dialogTrigger = GetDialogTrigger(__instance);
        if (dialogTrigger == null)
        {
            logger.LogInfo($"{__instance.name} does not have a DialogTrigger, skipping...");
            return;
        }

        logger.LogInfo($"Creating extra info for {__instance.name}");
        DialogInjectionManager.ExtraScriptableDialogDataInfo extraInfo = DialogInjectionManager.ExtraDialogDataInfo.GetOrCreateValue(dialogTrigger._scriptDialogData);
        extraInfo.vanillaBranchesLength = dialogTrigger._scriptDialogData._dialogBranches.Length;

        for (int i = DialogInjectionManager.InjectionParams.Count - 1; i >= 0; i--)
        {
            IDialogInjectionParams dParams = DialogInjectionManager.InjectionParams[i];
            if (dParams.IsTargetNPC(__instance))
            {
                logger.LogInfo($"Injecting dialog for {__instance.name}");
                InjectParams(dParams, __instance, dialogTrigger);
                DialogInjectionManager.InjectionParams.RemoveAt(i); // This stops the params from being re-injected every time the NPC loads in.
            }
        }
    }

    static void InjectParams(IDialogInjectionParams dParams, NetNPC npc, DialogTrigger dialogTrigger)
    {
        foreach (CustomDialogBranch customBranch in dParams.GetCustomDialogBranches(npc, dialogTrigger._scriptDialogData))
        {
            dialogTrigger._scriptDialogData._dialogBranches = dialogTrigger._scriptDialogData._dialogBranches.AddToArray(DialogInjectionManager.FlattenCustomDialogBranch(dialogTrigger._scriptDialogData, customBranch));
        }

        DialogInjectionManager.ExtraScriptableDialogDataInfo extraInfo = DialogInjectionManager.ExtraDialogDataInfo.GetOrCreateValue(dialogTrigger._scriptDialogData);
        for (int i = 0; i < dialogTrigger._scriptDialogData._dialogBranches.Length; i++)
        {
            DialogBranch dialogBranch = dialogTrigger._scriptDialogData._dialogBranches[i];
            for (int j = 0; j < dialogBranch.dialogs.Length; j++)
            {
                ref Dialog dialog = ref dialogBranch.dialogs[j];
                if (dParams.IsTargetDialog(i, j, npc, dialogTrigger._scriptDialogData, dialogBranch, dialog, out IDialogInjectionParams.DialogInjector injector))
                {
                    logger.LogInfo($"Modifying dialog \"{dialog._dialogKey}\"");

                    if (injector == null)
                    {
                        logger.LogError($"Dialog branch \"{dialogBranch._dialogIndexTag}\" for {npc.name} was targeted but no injector was provided, skipping...");
                        continue;
                    }

                    injector.Invoke(i, j, npc, dialogTrigger._scriptDialogData, dialogBranch, ref dialog, out IDialogInjectionParams.DynamicDialogInjector dynamicInjector);

                    dialog._dialogKey = dialog._dialogInput;
                    if (dialog._dialogKey.Length > 30)
                    {
                        dialog._dialogKey = dialog._dialogKey[..30] + "...";
                    }

                    if (dynamicInjector != null)
                    {
                        logger.LogInfo($"Dynamic injector provided on \"{dialogBranch._dialogIndexTag}\"");
                        Dictionary<int, List<IDialogInjectionParams.DynamicDialogInjector>> dict = extraInfo.dynamicInjections.GetValueOrDefault(i, null);
                        if (dict == null)
                        {
                            dict = new Dictionary<int, List<IDialogInjectionParams.DynamicDialogInjector>>();
                            extraInfo.dynamicInjections.Add(i, dict);
                        }
                        List<IDialogInjectionParams.DynamicDialogInjector> list = dict.GetValueOrDefault(j, null);
                        if (list == null)
                        {
                            list = new List<IDialogInjectionParams.DynamicDialogInjector>();
                            dict.Add(j, list);
                        }
                        list.Add(dynamicInjector);
                    }
                }
            }
        }
    }
}