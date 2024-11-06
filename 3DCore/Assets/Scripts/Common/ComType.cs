public static class ComType
{
    public const string HPRATIO = "fHPRatio";
    public const string RUNVALUE = "isRun";

    public const int FILE_NAME_LEN_MAX = 255;

    public const string UI_ROOT_NAME = "UIsRoot";
    public const string UI_ROOT_PAGE = "Page";
    public const string UI_ROOT_ABOVE = "Above";
    public const string UI_ROOT_POPUP = "Popup";

    public const string UI_PATH = "Prefabs/UI/Common/";
    public const string UI_PATH_PAGE = "Prefabs/UI/Page/";
    public const string UI_PATH_POPUP = "Prefabs/UI/Popup/";
    public const string UI_PATH_COMPO = "Prefabs/UI/Slot/";
    public const string UI_PATH_BATTLE = "Prefabs/UI/Battle/";
    public const string UI_PATH_BUTTON = "Prefabs/UI/Button/";
    public const string UI_PATH_ETC = "Prefabs/UI/ETCComponent/";

    public const string ATLAS_PATH = "SpriteAtlas/";
    public const string AUDIO_PATH = "Audios/";
    public const string MATERIAL_PATH = "Materials/";
    public const string TEXTURE_PATH = "Textures/";

    public static string[] AudioMixPaths = new string[]
    {
        "Master/SFX/ETC",
        "Master/SFX/UI",
        "Master/SFX/Notice",
        "Master/SFX/Voice",
        "Master/SFX/Battle",
        "Master/Music/BGM",
    };

    public static readonly string BGM_MIX = "Master/Music/BGM";
    public static readonly string UI_MIX = "Master/SFX/UI";
    public static readonly string BATTLE_MIX = "Master/SFX/Battle";
}
