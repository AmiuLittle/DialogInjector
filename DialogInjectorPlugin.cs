using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AmiuLittle.DialogInjector;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("org.sallys-workshop.localyssation")]
public class DialogInjectorPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Logger
        Logger = base.Logger;

        FacePics.Init();

        // Harmony
        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        harmony.CreateClassProcessor(typeof(Patches.DialogInjectionProcessor)).Patch();
        harmony.CreateClassProcessor(typeof(Patches.DynamicDialogInjectionProcessor)).Patch();
        harmony.CreateClassProcessor(typeof(Patches.ExtraDialogManagerInfoStartDialog)).Patch();
        harmony.CreateClassProcessor(typeof(Patches.ExtraDialogManagerInfoEndDialog)).Patch();
    }
}
