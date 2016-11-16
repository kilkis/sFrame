
CtrlNames = {
	Prompt = "PromptCtrl",
	Message = "MessageCtrl",
	Login = "LoginCtrl",
	LoginInfo = "LoginInfoCtrl",
	LoginCreate = "LoginCreateCtrl",
	Notice = "NoticeCtrl",
	ChooseCharacter = "ChooseCharacterCtrl",
	CreateCharacter = "CreateCharacterCtrl",
}

PanelNames = {
	"PromptPanel",	
	"MessagePanel",
	"LoginPanel",
	"LoginInfoPanel",
	"LoginCreateInfoPanel",
	"NoticePanel",
	"ChooseCharacterPanel",
	"CreateCharacterPanel",
}

--协议类型--
ProtocalType = {
	BINARY = 0,
	PB_LUA = 1,
	PBC = 2,
	SPROTO = 3,
}
--当前使用的协议类型--
TestProtoType = ProtocalType.BINARY;

Util = LuaFramework.Util;
AppConst = LuaFramework.AppConst;
LuaHelper = LuaFramework.LuaHelper;
ByteBuffer = LuaFramework.ByteBuffer;
Loading = sFrame.sULoading;
Flow = sFrame.sGameFlow;
Network = sFrame.sNetworkInterface;

resMgr = LuaHelper.GetResManager();
panelMgr = LuaHelper.GetPanelManager();
soundMgr = LuaHelper.GetSoundManager();
networkMgr = LuaHelper.GetNetManager();

WWW = UnityEngine.WWW;
GameObject = UnityEngine.GameObject;