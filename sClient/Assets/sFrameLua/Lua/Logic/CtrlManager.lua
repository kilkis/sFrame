require "Common/define"
require "Controller/PromptCtrl"
require "Controller/MessageCtrl"
require "Controller/LoginCtrl"
require "Controller/LoginInfoCtrl"
require "Controller/LoginCreateInfoCtrl"
require "Controller/NoticeCtrl"
require "Controller/CreateCharacterCtrl"


CtrlManager = {};
local this = CtrlManager;
local ctrlList = {};	--控制器列表--

function CtrlManager.Init()
	logWarn("CtrlManager.Init----->>>");
	ctrlList[CtrlNames.Prompt] = PromptCtrl.New();
	ctrlList[CtrlNames.Message] = MessageCtrl.New();
	ctrlList[CtrlNames.Login] = LoginCtrl.New();
	ctrlList[CtrlNames.LoginInfo] = LoginInfoCtrl.New();
	ctrlList[CtrlNames.LoginCreate] = LoginCreateInfoCtrl.New();
	ctrlList[CtrlNames.Notice] = NoticeCtrl.New();
	ctrlList[CtrlNames.CreateCharacter] = CreateCharacterCtrl.New();
	return this;
end

--添加控制器--
function CtrlManager.AddCtrl(ctrlName, ctrlObj)
	ctrlList[ctrlName] = ctrlObj;
end

--获取控制器--
function CtrlManager.GetCtrl(ctrlName)
	return ctrlList[ctrlName];
end

--移除控制器--
function CtrlManager.RemoveCtrl(ctrlName)
	ctrlList[ctrlName] = nil;
end

--关闭控制器--
function CtrlManager.Close()
	logWarn('CtrlManager.Close---->>>');
end