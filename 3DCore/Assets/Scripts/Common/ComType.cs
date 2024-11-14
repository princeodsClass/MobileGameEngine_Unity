public static class ComType
{
    // public const string URL = "http://ec2-15-164-210-94.ap-northeast-2.compute.amazonaws.com:29661/";
    public const string URL = "http://localhost:29661/";

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

    public const string EXCEPTION_ACCESSTOKEN = "accessToken does not match";

    public const string SERVER_CONTROLLER_DATATABLE_VERSION = "checkversion";
    public const string SERVER_CONTROLLER_DATATABLE_DOWNLOAD = "datatable";
    public const string SERVER_CONTROLLER_MAP_DOWNLOAD = "mapdata";

    public const string SERVER_ACCOUNT_POST_PATH = "account";
    public const string SERVER_INVENTORY_POST_PATH = "inventory";
    public const string SERVER_SHOP_POST_PATH = "shop";
    public const string SERVER_MAP_POST_PATH = "map";
    public const string SERVER_ETC_POST_PATH = "etc";
    public const string SERVER_ABYSS_POST_PATH = "abyss";

    public const string API_URL_CREATE_ACCOUNT = "class_create_account";
    public const string API_URL_CREATE_ITEM = "class_create_item";
    public const string API_URL_GET_ACCOUNT_INFO = "class_get_account_info";
    public const string API_URL_GET_INVENTORY_iNFO = "class_get_inventory_info";
    public const string API_URL_UPDATE_ACCOUNT_INFO = "class_update_account_info";
    public const string API_URL_UPDATE_ITEM_INFO = "class_update_item_info";
    public const string API_URL_DELETE_ACCOUNT = "class_delete_account";
    public const string aPI_URL_DELETE_ITEM = "class_delete_item";

    public const string DEFAULT_NICKNAME = "GEUST";

    public const string STORAGE_UID = "auid";
}
