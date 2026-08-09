namespace AmiuLittle.DialogInjector;

public enum DialogInjectionMethod
{
    /// <summary>
    /// Replace a dialog with a new one when the NPC is loaded in.
    /// </summary>
    REPLACE,
    /// <summary>
    /// Add a new dialog selection option to a existing dialog when the NPC is loaded in.
    /// </summary>
    ADD_SELECTIONS,
    /// <summary>
    /// Inject dialog when the NPC is talked to
    /// </summary>
    DYNAMIC
}