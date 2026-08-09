namespace AmiuLittle.DialogInjector;

public class BasicDialogInjection
{
    public delegate bool CustomDialogSearch(int branchIdx, int dialogIdx, NetNPC npc, ScriptableDialogData dialogData, DialogBranch dialogBranch, Dialog dialog);

    public DialogInjectionSearchMethod searchMethod;
    public string searchSubStr;
    public int branchIdx;
    public int dialogIdx;
    public CustomDialog customDialogInstance;
    public CustomDialogSearch customSearch;

    public DialogInjectionMethod injectionMethod;
    public CustomDialog replacement;
    public CustomDialogSelection[] newDialogSelections;
    public IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector;

    public BasicDialogInjection(string substr, CustomDialog replacement)
    {
        searchMethod = DialogInjectionSearchMethod.SUBSTR;
        searchSubStr = substr;
        injectionMethod = DialogInjectionMethod.REPLACE;
        this.replacement = replacement;
    }
    public BasicDialogInjection(string substr, CustomDialogSelection[] newDialogSelections)
    {
        searchMethod = DialogInjectionSearchMethod.SUBSTR;
        searchSubStr = substr;
        injectionMethod = DialogInjectionMethod.ADD_SELECTIONS;
        this.newDialogSelections = newDialogSelections;
    }
    public BasicDialogInjection(string substr, IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector)
    {
        searchMethod = DialogInjectionSearchMethod.SUBSTR;
        searchSubStr = substr;
        injectionMethod = DialogInjectionMethod.DYNAMIC;
        this.dynamicDialogInjector = dynamicDialogInjector;
    }
    public BasicDialogInjection(int branchIdx, int dialogIdx, CustomDialog replacement)
    {
        searchMethod = DialogInjectionSearchMethod.INDEX;
        this.branchIdx = branchIdx;
        this.dialogIdx = dialogIdx;
        injectionMethod = DialogInjectionMethod.REPLACE;
        this.replacement = replacement;
    }
    public BasicDialogInjection(int branchIdx, int dialogIdx, CustomDialogSelection[] newDialogSelections)
    {
        searchMethod = DialogInjectionSearchMethod.INDEX;
        this.branchIdx = branchIdx;
        this.dialogIdx = dialogIdx;
        injectionMethod = DialogInjectionMethod.ADD_SELECTIONS;
        this.newDialogSelections = newDialogSelections;
    }
    public BasicDialogInjection(int branchIdx, int dialogIdx, IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector)
    {
        searchMethod = DialogInjectionSearchMethod.INDEX;
        this.branchIdx = branchIdx;
        this.dialogIdx = dialogIdx;
        injectionMethod = DialogInjectionMethod.DYNAMIC;
        this.dynamicDialogInjector = dynamicDialogInjector;
    }
    public BasicDialogInjection(CustomDialog targetDialog, CustomDialog replacement)
    {
        searchMethod = DialogInjectionSearchMethod.INSTANCE;
        customDialogInstance = targetDialog;
        injectionMethod = DialogInjectionMethod.REPLACE;
        this.replacement = replacement;
    }
    public BasicDialogInjection(CustomDialog targetDialog, CustomDialogSelection[] newDialogSelections)
    {
        searchMethod = DialogInjectionSearchMethod.INSTANCE;
        customDialogInstance = targetDialog;
        injectionMethod = DialogInjectionMethod.ADD_SELECTIONS;
        this.newDialogSelections = newDialogSelections;
    }
    public BasicDialogInjection(CustomDialog targetDialog, IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector)
    {
        searchMethod = DialogInjectionSearchMethod.INSTANCE;
        customDialogInstance = targetDialog;
        injectionMethod = DialogInjectionMethod.DYNAMIC;
        this.dynamicDialogInjector = dynamicDialogInjector;
    }
    public BasicDialogInjection(CustomDialogSearch customSearch, CustomDialog replacement)
    {
        searchMethod = DialogInjectionSearchMethod.CUSTOM;
        this.customSearch = customSearch;
        injectionMethod = DialogInjectionMethod.REPLACE;
        this.replacement = replacement;
    }
    public BasicDialogInjection(CustomDialogSearch customSearch, CustomDialogSelection[] newDialogSelections)
    {
        searchMethod = DialogInjectionSearchMethod.CUSTOM;
        this.customSearch = customSearch;
        injectionMethod = DialogInjectionMethod.ADD_SELECTIONS;
        this.newDialogSelections = newDialogSelections;
    }
    public BasicDialogInjection(CustomDialogSearch customSearch, IDialogInjectionParams.DynamicDialogInjector dynamicDialogInjector)
    {
        searchMethod = DialogInjectionSearchMethod.CUSTOM;
        this.customSearch = customSearch;
        injectionMethod = DialogInjectionMethod.DYNAMIC;
        this.dynamicDialogInjector = dynamicDialogInjector;
    }
}