using System.Collections.Generic;

namespace AmiuLittle.DialogInjector;

/// <summary>
/// Tells the dialog injector how to inject dialog and who to target.
/// <br/>
/// Implement this class if you need to use specific behaviours to inject dialog into an NPC, otherwise use <see cref="BasicDialogInjectionParams"/>
/// </summary>
public interface IDialogInjectionParams
{
    /// <summary>
    /// This is run on the loading screen to modify a targeted dialog branch, static dialog modifications should be made here.
    /// </summary>
    /// <param name="branchIdx">The dialog branch's idx.</param>
    /// <param name="dialogIdx">The dialog's idx</param>
    /// <param name="npc">The target NPC.</param>
    /// <param name="dialogData">The NPC's dialog data.</param>
    /// <param name="dialogBranch">The dialog branch associated with the target dialog.</param>
    /// <param name="dialog">The target dialog.</param>
    /// <param name="dynamicDialogInjector">Leave null if you do not wish for this dialog branch to have a dynamic dialog function.</param>
    public delegate void DialogInjector(int branchIdx, int dialogIdx, NetNPC npc, ScriptableDialogData dialogData, DialogBranch dialogBranch, ref Dialog dialog, out DynamicDialogInjector dynamicDialogInjector);
    /// <summary>
    /// This is run when the target dialog is about to be displayed to the screen, dynamic dialog modifications should be made here.
    /// </summary>
    /// <param name="branchIdx">The target dialog branch's id.</param>
    /// <param name="dialogIdx">The dialog's idx</param>
    /// <param name="npc">The target NPC.</param>
    /// <param name="player">The player that is being talked to.</param>
    /// <param name="dialogData">The NPC's dialog data.</param>
    /// <param name="dialogBranch">The target dialog branch</param>
    /// <param name="dialog">The target dialog.</param>
    /// <returns>What the character should say. If null, will leave whatever the NPC is about to say alone.</returns>
    /// <remarks>
    /// The reason that you have to return a string is because just editing Dialog._dialogInput
    /// will not get around the text queue. This also makes it possible for DialogInjector to override the 
    /// text of dialogs with _altInputs. The returned string does not edit Dialog._dialogInput, if you want 
    /// to make the new dialog persist you will have to edit Dialog._dialogInput or Dialog._altInputs yourself.
    /// </remarks>
    public delegate string DynamicDialogInjector(int branchIdx, int dialogIdx, NetNPC npc, Player player, ScriptableDialogData dialogData, DialogBranch dialogBranch, Dialog dialog);

    /// <summary>
    /// This is run to see if the NPC that is currently being checked
    /// is the NPC that should have their dialog changed.
    /// </summary>
    /// <param name="npc">The NetNPC instance that is currently being checked.</param>
    /// <returns>True if the NPC is the one that should have their dialog changed.</returns>
    bool IsTargetNPC(NetNPC npc);

    /// <summary>
    /// This is run to add custom dialog branches to the target NPC.
    /// </summary>
    /// <param name="npc">The target NPC.</param>
    /// <param name="dialogData"></param>
    /// <returns>The new dialog branches to be added.</returns>
    IEnumerable<CustomDialogBranch> GetCustomDialogBranches(NetNPC npc, ScriptableDialogData dialogData);

    /// <summary>
    /// This run to see if the dialog that is currently being checked
    /// is the dialog that should be hooked into.
    /// </summary>
    /// <param name="branchIdx">The dialog branch's idx</param>
    /// <param name="dialogIdx">The dialog's idx</param>
    /// <param name="npc">The targeted NPC.</param>
    /// <param name="dialogData">The NPC's dialog data.</param>
    /// <param name="dialogBranch">The dialog branch associated with the current dialog that's being checked.</param>
    /// <param name="dialog">The dialog that is being checked.</param>
    /// <param name="injector">The dialog injector function to use if dialog is the target branch.</param>
    /// <returns>True if the dialog currently being checked is the target dialog</returns>
    public bool IsTargetDialog(int branchIdx, int dialogIdx, NetNPC npc, ScriptableDialogData dialogData, DialogBranch dialogBranch, Dialog dialog, out IDialogInjectionParams.DialogInjector injector);
}