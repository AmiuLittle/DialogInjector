namespace AmiuLittle.DialogInjector;

public enum BranchReferenceType
{
    /// <summary>
    /// Branch reference by index.
    /// </summary>
    INDEX,
    /// <summary>
    /// Branch reference by searching for a substring in it's first dialog.
    /// </summary>
    SUBSTR,
    /// <summary>
    /// Branch reference by <see cref="CustomDialogBranch"/>.
    /// </summary>
    CUSTOM_DIALOG_BRANCH
}