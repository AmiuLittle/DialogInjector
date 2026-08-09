namespace AmiuLittle.DialogInjector;

public class BranchReference
{
    internal BranchReferenceType referenceType;
    internal int index;
    internal string searchSubstr;
    internal CustomDialogBranch refDialogBranch;

    public BranchReference(int index)
    {
        referenceType = BranchReferenceType.INDEX;
        this.index = index;
    }
    public BranchReference(string searchSubstr)
    {
        referenceType = BranchReferenceType.SUBSTR;
        this.searchSubstr = searchSubstr;
    }
    public BranchReference(CustomDialogBranch refDialogBranch)
    {
        referenceType = BranchReferenceType.CUSTOM_DIALOG_BRANCH;
        this.refDialogBranch = refDialogBranch;
    }

    public static readonly BranchReference END_DIALOG = new BranchReference(-1);
}