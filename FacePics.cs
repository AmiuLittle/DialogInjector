using UnityEngine;

namespace AmiuLittle.DialogInjector;

/// <summary>
/// All available vanilla facepics in ATLYSS 12026.a3.
/// </summary>
public static class FacePics {
    /// <summary>
    /// facepic_angela01
    /// </summary>
    public static Sprite ANGELA_NEUTRAL = null;
    /// <summary>
    /// facepic_angela02
    /// </summary>
    public static Sprite ANGELA_EXPLAINING = null;
    /// <summary>
    /// facepic_angela03
    /// </summary>
    public static Sprite ANGELA_SAD = null;

    /// <summary>
    /// facepic_enok01
    /// </summary>
    public static Sprite ENOK_NEUTRAL = null;

    /// <summary>
    /// facepic_sally01
    /// </summary>
    public static Sprite SALLY_NEUTRAL = null;
    /// <summary>
    /// facepic_sally02
    /// </summary>
    public static Sprite SALLY_HAPPY = null;
    /// <summary>
    /// facepic_sally03
    /// </summary>
    public static Sprite SALLY_CONCERNED = null;

    /// <summary>
    /// facepic_spike01
    /// </summary>
    public static Sprite SPIKE_NEUTRAL = null;

    /// <summary>
    /// facepic_vivian
    /// </summary>
    public static Sprite VIVIAN_NEUTRAL = null;


    internal static void Init()
    {
        ANGELA_NEUTRAL = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_angela01");
        ANGELA_EXPLAINING = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_angela02");
        ANGELA_SAD = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_angela03");
        ENOK_NEUTRAL = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_enok01");
        SALLY_NEUTRAL = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_sally01");
        SALLY_HAPPY = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_sally02");
        SALLY_CONCERNED = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_sally03");
        SPIKE_NEUTRAL = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_spike01");
        VIVIAN_NEUTRAL = Resources.Load<Sprite>("_graphic/_ui/_facepics/facepic_vivian");
    }
}