using System;
using System.Collections;
using System.Collections.Generic;

public enum EOSType
{
	Editor,
	Android,
	iOS,
}

public enum ESceneType
{
	NONE = -1,
	MemuScene,
	TestScene,

	MAX
}

public enum EAtlasType
{
	Icons = 0,
	Common,
	InGame,
	Outgame,

	END
}

public enum EResourceType
{
	None = -1,
	UI,
	UIPage,
	UIPopup,
	UIComponent,
	UIButton,
	UIETC,

    Custom,

    End
}

public enum EUIType
{
	Page = 0,
	Popup,
	Component,
	End
}

public enum EUIPage
{

	PageMenu = 1,
	PageTest,

	AboveTutorial,
	AboveQuestCard,

	End
}

public enum EUIPopup
{
	PopupLoading = 0,

    PopupOneButton,
    PopupTwoButton,

	PopupWait4Response,

	End
}

public enum EUIComponent
{
	SlotItem = 1,
	SlotWeapon,
	SlotMaterial,
	SlotGear,
	SlotBox,

	SlotMessage,

	SlotSimpleItem,
	SlotSimpleItemReward,
	SlotEffect,
	SlotStat,
	SlotBattlePass,

	SlotShopBox,
	SlotShopCrystal,
	SlotShopGameMoney,
	SlotShopADS,
	SlotShopVIP,
	SlotBuyVIP,

	SlotSkill,
	SlotAbyss,
	SlotAbyssGroup,
	SlotAbyssRankTable,
	SlotAbyssLeague,
	SlotAbyssLeagueGrade,
	SlotAbyssRewards,

	SlotDefenceRewardsGroup,

	SlotRecipeItem,
	SlotRecipeItemGroup,

	SlotBattlePassSet,
	SlotEpisode,
	SlotWorkshopItem,
	SlotWorkshopSymbol,
	SlotAlarm,

	SlotChapterBlock,

	SlotAttendance,

	SlotQuest,
	SlotQuestReward,

	ToastMessage,

	TooltipItem,
	TooltipItemList,
	TooltipMessage,
	TooltipToast,

	BattleSlotWeapon,
	End
}

