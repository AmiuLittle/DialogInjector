# Atlyss Dialog Injector
A mod for injecting your own custom dialog into ATLYSS NPCs.

## Basic Usage
Most dialog injection can be handled by the `BasicDialogInjectionParams` class.
<br/>
Say if you wanted Angela to have an extra dialog branch, your code would look something like this:
```C#
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using AmiuLittle.DialogInjector;

namespace Example.MyMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Logger
        Logger = base.Logger;

        CustomDialogBranch myNewDialogBranch = new CustomDialogBranch([]);
        myNewDialogBranch.dialogs = [
            new CustomDialog(
                "Here's a new dialog in the new dialog branch.",
                FacePics.ANGELA_NEUTRAL,
                ""
            ),
            new CustomDialog(
                [
                    "Look, this one has multiple dialogs.",
                    "These dialogs are picked at random between each other."
                ],
                FacePics.ANGELA_NEUTRAL,
                ""
            ),
            new CustomDialog(
                "This dialog will make angela nod.",
                FacePics.ANGELA_NEUTRAL,
                "nod_loop"
            ),
            new CustomDialog(
                "This dialog has multiple branches.",
                FacePics.ANGELA_NEUTRAL,
                "",
                [
                    new CustomDialogSelection(
                        "Repeat that again?",
                        null, // no icon
                        new BranchReference(myNewDialogBranch)
                    ),
                    new CustomDialogSelection(
                        "Ah, Okay.",
                        null,
                        new BranchReference(0) // returns to the first dialog branch (the one that the NPC starts talking to you with)
                    )
                ]
            ),
        ];
        DialogInjectionManager.RegisterDialogInjectionParams(new BasicDialogInjectionParams(
            "Angela" // This matches by substring to the name of the root GameObject of the NPC
            [
                myNewDialogBranch
            ],
            [
                new BasicDialogInjection(
                    // This matches an NPCs dialog by substring, I recommend using Runtime Unity Editor to find these, more about it in the upcoming docs
                    "My knowledge is yours, what can I help you with?",
                    [ // This will add new dialog selections to this part of angela's dialog.
                        new CustomDialogSelection(
                            "Dialog Injection?",
                            null,
                            new BranchReference(myNewDialogBranch)
                        )
                    ]
                )
            ]
        ));
    }
}
```
It's kinda complicated, but definitely beats out manually writing out 15 patches to achieve the same effect.

## Building
### Requirements
- .NET SDK version 6.0 or newer
- An installed copy of ATLYSS
### Directions
Add a file called `Directory.Build.props` to the root of this project and fill it out as so.
```xml
<Project>
  <PropertyGroup>
    <ATLYSS_PATH>/your/path/to/steamapps/common/ATLYSS</ATLYSS_PATH>
  </PropertyGroup>
</Project>
```
Replace `/your/path/to` with the file path to your steam folder.
<br/>
Then simply run this command to build.
```bash
dotnet build -c Release
```
The finished .dll should be in the bin folder.

## License
[MIT License, Copyright (c) 2026 AmiuLittle](./LICENSE)
