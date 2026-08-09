using UnityEngine;

namespace AmiuLittle.DialogInjector;

public class CustomDialogSelection(string _text, Sprite _icon, BranchReference _nextBranch)
{
    public string text = _text;
    public Sprite icon = _icon;
    public BranchReference nextBranch = _nextBranch;
}