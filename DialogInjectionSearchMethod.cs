namespace AmiuLittle.DialogInjector;

public enum DialogInjectionSearchMethod
{
    /// <summary>
    /// Find a dialog by searching for a sub string.
    /// </summary>
    SUBSTR,
    /// <summary>
    /// Find a dialog by the index of the branch it's in and the index of the dialog in the branch.
    /// </summary>
    INDEX,
    /// <summary>
    /// Find a custom dialog using its <see cref="CustomDialog"/> instance.
    /// </summary>
    INSTANCE,
    /// <summary>
    /// Use a custom function to find a dialog.
    /// </summary>
    CUSTOM
}