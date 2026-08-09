using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace AmiuLittle.DialogInjector;

public static class DialogInjectionManager
{
    internal static List<IDialogInjectionParams> InjectionParams { get; private set; } = new List<IDialogInjectionParams>();
    internal static ConditionalWeakTable<DialogManager, ExtraVanillaDialogManagerInfo> ExtraDialogManagerInfo { get; private set; } =
        new ConditionalWeakTable<DialogManager, ExtraVanillaDialogManagerInfo>();
    internal static ConditionalWeakTable<ScriptableDialogData, ExtraScriptableDialogDataInfo> ExtraDialogDataInfo { get; private set; } =
        new ConditionalWeakTable<ScriptableDialogData, ExtraScriptableDialogDataInfo>();
    internal static ConditionalWeakTable<DialogBranch, ExtraVanillaDialogBranchInfo> ExtraDialogBranchInfo { get; private set; } =
        new ConditionalWeakTable<DialogBranch, ExtraVanillaDialogBranchInfo>();
    internal static ConditionalWeakTable<Dialog, ExtraVanillaDialogInfo> ExtraDialogInfo { get; private set; } =
        new ConditionalWeakTable<Dialog, ExtraVanillaDialogInfo>();

    /// <summary>
    /// Register an instance of a class that implements <see cref="IDialogInjectionParams"/>.
    /// </summary>
    /// <remarks>
    /// All injection params should be registered at Plugin.Awake().
    /// </remarks>
    /// <param name="injectionParams">Params to register.</param>
    public static void RegisterDialogInjectionParams(IDialogInjectionParams injectionParams)
    {
        InjectionParams.Add(injectionParams);
    }

    public class ExtraVanillaDialogManagerInfo
    {
        public int currentDialogBranchIdx = -1;
    }
    public class ExtraScriptableDialogDataInfo
    {
        public Dictionary<int, Dictionary<int, List<IDialogInjectionParams.DynamicDialogInjector>>> dynamicInjections =
            new Dictionary<int, Dictionary<int, List<IDialogInjectionParams.DynamicDialogInjector>>>();
        public int vanillaBranchesLength = -1;
    }
    public class ExtraVanillaDialogBranchInfo
    {
        public CustomDialogBranch baseCustomDialogBranchInstance;
    }
    public class ExtraVanillaDialogInfo
    {
        public CustomDialog baseCustomDialogInstance;
    }

    /// <summary>
    /// A utility to function to find a dialog branches by a substring.
    /// <br/>
    /// This will search both the _dialogInput and _altInputs of the first dialog in each DialogBranch.
    /// </summary>
    /// <param name="dialogData">The dialog data to search.</param>
    /// <param name="substring">Substring to search for.</param>
    /// <returns>The index of the dialog branch, -1 if none was found.</returns>
    public static int FindDialogBranchIdxByFirstDialog(ScriptableDialogData dialogData, string substring)
    {
        for (int i = 0; i < dialogData._dialogBranches.Length; i++)
        {
            DialogBranch branch = dialogData._dialogBranches[i];
            if (branch.dialogs.Length > 0)
            {
                if (branch.dialogs[0]._dialogInput.Contains(substring))
                {
                    return i;
                }
                if (branch.dialogs[0]._altInputs != null)
                {
                    foreach (string altInput in branch.dialogs[0]._altInputs)
                    {
                        if (altInput.Contains(substring))
                        {
                            return i;
                        }
                    }
                }
            }
        }
        return -1;
    }

    /// <summary>
    /// Turns a <see cref="CustomDialogBranch"/> into a vanilla DialogBranch.
    /// </summary>
    /// <param name="dialogData">The dialog data associated with the branch.</param>
    /// <param name="customBranch">The custom branch to flatten.</param>
    /// <returns>The flattened DialogBranch.</returns>
    public static DialogBranch FlattenCustomDialogBranch(ScriptableDialogData dialogData, CustomDialogBranch customBranch)
    {
        DialogBranch vanillaBranch = new DialogBranch();
        ExtraVanillaDialogBranchInfo extraInfo = ExtraDialogBranchInfo.GetOrCreateValue(vanillaBranch);
        extraInfo.baseCustomDialogBranchInstance = customBranch;

        vanillaBranch._dialogIndexTag = $"Dialog Branch Index #{dialogData._dialogBranches.Length}";
        vanillaBranch.dialogs = [];
        
        foreach (CustomDialog customDialog in customBranch.dialogs)
        {
            vanillaBranch.dialogs = vanillaBranch.dialogs.AddToArray(FlattenCustomDialog(dialogData, customDialog));
        }

        return vanillaBranch;
    }

    /// <summary>
    /// Turns a <see cref="CustomDialog"/> into a vanilla Dialog.
    /// </summary>
    /// <param name="dialogData">The dialog data associated with the dialog.</param>
    /// <param name="customDialog">The custom dialog to flatten.</param>
    /// <returns>The flattened Dialog.</returns>
    public static Dialog FlattenCustomDialog(ScriptableDialogData dialogData, CustomDialog customDialog)
    {
        Dialog vanillaDialog = new Dialog();
        ExtraVanillaDialogInfo extraInfo = ExtraDialogInfo.GetOrCreateValue(vanillaDialog);
        extraInfo.baseCustomDialogInstance = customDialog;

        vanillaDialog._dialogKey = customDialog.dialogs[0];
        vanillaDialog._dialogInput = customDialog.dialogs[0];
        if (vanillaDialog._dialogKey.Length > 30)
        {
            vanillaDialog._dialogKey = vanillaDialog._dialogKey[..30] + "...";
        }
        if (customDialog.dialogs.Length > 1)
        {
            vanillaDialog._altInputs = customDialog.dialogs;
        }
        vanillaDialog.facepic = customDialog.facepic;
        vanillaDialog._npcAnimationTag = customDialog.npcAnimation;
        vanillaDialog._dialogUI = DialogUIPrompt.NULL;
        vanillaDialog._dialogSelections = [];

        if (customDialog.dialogSelections != null)
        {
            // ATLYSS stores dialog selections in backwards order
            for (int i = customDialog.dialogSelections.Length - 1; i >= 0; i--)
            {
                CustomDialogSelection customDialogSelection = customDialog.dialogSelections[i];
                vanillaDialog._dialogSelections = vanillaDialog._dialogSelections.AddToArray(FlattenCustomDialogSelection(dialogData, customDialogSelection));
            }
        }

        return vanillaDialog;
    }

    /// <summary>
    /// Turns a <see cref="CustomDialogSelection"/> into a vanilla DialogSelection.
    /// </summary>
    /// <param name="dialogData">The dialog data associated with the dialog selection.</param>
    /// <param name="customDialogSelection">The custom dialog selection to flatten.</param>
    /// <returns>The flattened DialogSelection.</returns>
    public static DialogSelection FlattenCustomDialogSelection(ScriptableDialogData dialogData, CustomDialogSelection customDialogSelection)
    {
        ExtraScriptableDialogDataInfo extraInfo = ExtraDialogDataInfo.GetOrCreateValue(dialogData);

        DialogSelection vanillaDialogSelection = new DialogSelection();
        vanillaDialogSelection._selectionCaption = customDialogSelection.text;
        vanillaDialogSelection._selectionIcon = customDialogSelection.icon;
        vanillaDialogSelection._setDialogIndex = -1;
        if (customDialogSelection.nextBranch.referenceType == BranchReferenceType.INDEX)
        {
            vanillaDialogSelection._setDialogIndex = customDialogSelection.nextBranch.index;
        }
        else if (customDialogSelection.nextBranch.referenceType == BranchReferenceType.SUBSTR)
        {
            for (int i = 0; i < dialogData._dialogBranches.Length; i++)
            {
                DialogBranch dialogBranch = dialogData._dialogBranches[i];
                if (dialogBranch.dialogs == null || dialogBranch.dialogs.Length <= 0)
                {   
                    continue;
                }
                if (dialogBranch.dialogs[0]._dialogInput.Contains(customDialogSelection.nextBranch.searchSubstr))
                {
                    vanillaDialogSelection._setDialogIndex = i;
                    break;
                }
                foreach (string altInput in dialogBranch.dialogs[0]._altInputs)
                {
                    if (altInput.Contains(customDialogSelection.nextBranch.searchSubstr))
                    {
                        vanillaDialogSelection._setDialogIndex = i;
                        break;
                    }
                }
                if (vanillaDialogSelection._setDialogIndex > -1)
                {
                    break;
                }
            }
            if (vanillaDialogSelection._setDialogIndex == -1)
            { 
                DialogInjectorPlugin.Logger.LogError($"Dialog Selection \"{customDialogSelection.text}\": Substring \"{customDialogSelection.nextBranch.searchSubstr}\" was not found in the dialog for \"{dialogData._nameTag}\", selecting this dialog selection will end the dialog instead.");
            }
        }
        else if (customDialogSelection.nextBranch.referenceType == BranchReferenceType.CUSTOM_DIALOG_BRANCH)
        {
            for (int i = 0; i < dialogData._dialogBranches.Length; i++)
            {
                DialogBranch dialogBranch = dialogData._dialogBranches[i];
                ExtraVanillaDialogBranchInfo extraBranchInfo = ExtraDialogBranchInfo.GetOrCreateValue(dialogBranch);
                if (extraBranchInfo.baseCustomDialogBranchInstance == null)
                {
                    if (i >= extraInfo.vanillaBranchesLength)
                    {  
                        DialogInjectorPlugin.Logger.LogWarning($"Non-Vanilla Dialog Branch \"{dialogBranch._dialogIndexTag}\" does not have an associated CustomDialogBranch instance!");
                    }
                    continue;
                }

                if (Object.ReferenceEquals(extraBranchInfo.baseCustomDialogBranchInstance, customDialogSelection.nextBranch.refDialogBranch))
                {
                    vanillaDialogSelection._setDialogIndex = i;
                    break;
                }
            }
            if (vanillaDialogSelection._setDialogIndex == -1)
            { 
                DialogInjectorPlugin.Logger.LogError($"Dialog Selection \"{customDialogSelection.text}\": Provided CustomDialogBranch was not found in the dialog for \"{dialogData._nameTag}\", selecting this dialog selection will end the dialog instead.");
            }
        }

        return vanillaDialogSelection;
    }
}