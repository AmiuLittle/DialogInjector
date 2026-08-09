using UnityEngine;

namespace AmiuLittle.DialogInjector;

public class CustomDialog
{
    /// <summary>
    /// Represents the all the different randomly selected texts that an
    /// NPC can have for this dialog, put only one in if you want the same text
    /// every time for this dialog.
    /// </summary>
    public string[] dialogs;
    /// <summary>
    /// Picture that will show as you are speaking to an NPC.
    /// </summary>
    /// <remarks>
    /// All vanilla facepics can be found in <see cref="FacePics"/>.
    /// </remarks>
    public Sprite facepic;
    public string npcAnimation;

    /// <summary>
    /// Represents every dialog selection for this dialog.
    /// <br/>
    /// If it is null or empty it means the dialog should continue to the next one or end the dialog interaction.
    /// </summary>
    public CustomDialogSelection[] dialogSelections;

    public CustomDialog(string dialog, Sprite facepic, string npcAnimation = "", CustomDialogSelection[] dialogSelections = null)
    {
        dialogs = [dialog];
        this.facepic = facepic;
        this.npcAnimation = npcAnimation;
        this.dialogSelections = dialogSelections;
    }

    public CustomDialog(string[] dialogs, Sprite facepic, string npcAnimation = "", CustomDialogSelection[] dialogSelections = null)
    {
        this.dialogs = dialogs;
        this.facepic = facepic;
        this.npcAnimation = npcAnimation;
        this.dialogSelections = dialogSelections;
    }
}