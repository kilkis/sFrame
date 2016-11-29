require "Common/define"

ChooseCharacterCtrl = {}
local this = ChooseCharacterCtrl;

local panel;
local login;
local transform;
local gameObject;

local ffi = require("ffi")

function ChooseCharacterCtrl.New()
	return this;
end

function ChooseCharacterCtrl.Awake()
	panelMgr:CreatePanel('ugui/ChooseCharacterPanel', this.OnCreate);
end

function ChooseCharacterCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	lb = transform:GetComponent("LuaBehaviour");

	lb:AddClick(ChooseCharacterPanel.btnEnter, this.OnClick);
	lb:AddClick(ChooseCharacterPanel.avatarBtn[0], this.OnChoose1);
	lb:AddClick(ChooseCharacterPanel.avatarBtn[1], this.OnChoose2);
	lb:AddClick(ChooseCharacterPanel.avatarBtn[2], this.OnChoose3);
	lb:AddClick(ChooseCharacterPanel.delBtn[0], this.OnDel1);
	lb:AddClick(ChooseCharacterPanel.delBtn[1], this.OnDel2);
	lb:AddClick(ChooseCharacterPanel.delBtn[2], this.OnDel3);
	

	this.selAvatarDBID = ffi.new("uint64_t");
	this.selAvatarDBID = 0;
	
	Loading.instance:hideLoading();
end

--单击事件--
function ChooseCharacterCtrl.OnClick(go)
	if this.selAvatarDBID == 0 then
		log("sel avatarDBID is zero!");
		--这里应该显示错误提示框
	else
		Loading.instance:showLoading();
		--Network.instance:sendMsg2Server("selectAvatarGame", this.selAvatarDBID);
		Network.instance:selAvatar(this.selAvatarDBID, "ChooseCharacterCtrl", "OnSelOK");
	end
end

function ChooseCharacterCtrl.CloseTogether()
	destroy(gameObject);
end

function ChooseCharacterCtrl.OnSelOK(go)
	destroy(gameObject);
	local ctrl = CtrlManager.GetCtrl(CtrlNames.BattleButton);
	if ctrl ~= nil then
		ctrl:Awake();
	end
	Flow.instance:changeNextState();
end

function ChooseCharacterCtrl.OnDelOK(go)
	ChooseCharacterPanel:Reset();
end


function ChooseCharacterCtrl.OnChoose1(go)
	if ChooseCharacterPanel.count > 0 then
		log(ChooseCharacterPanel.avatarInfo[0].name.." : "..ChooseCharacterPanel.avatarInfo[0].dbid);
		this.selAvatarDBID = ChooseCharacterPanel.avatarInfo[0].dbid;
	else
		log("there is no character1");
		ChooseCharacterPanel:Hide();
		local ctrl = CtrlManager.GetCtrl(CtrlNames.CreateCharacter);
		if ctrl ~= nil then
			ctrl:Awake();
		end
	end
end

function ChooseCharacterCtrl.OnChoose2(go)
	if ChooseCharacterPanel.count > 1 then
		log(ChooseCharacterPanel.avatarInfo[1].name);
		this.selAvatarDBID = ChooseCharacterPanel.avatarInfo[1].dbid;
	else
		log("there is no character2");
	end
end

function ChooseCharacterCtrl.OnChoose3(go)
	if ChooseCharacterPanel.count > 2 then
		log(ChooseCharacterPanel.avatarInfo[2].name);
		this.selAvatarDBID = ChooseCharacterPanel.avatarInfo[2].dbid;
	else
		log("there is no character3");
	end
end

function ChooseCharacterCtrl.OnDel1(go)
	if ChooseCharacterPanel.count > 0 then
		Network.instance:delAvatar(ChooseCharacterPanel.avatarNameList[0].transform:GetComponent("Text").text, "ChooseCharacterCtrl", "OnDelOK");
	end
end

function ChooseCharacterCtrl.OnDel2(go)
	if ChooseCharacterPanel.count > 1 then
		Network.instance:delAvatar(ChooseCharacterPanel.avatarNameList[1].transform:GetComponent("Text").text, "ChooseCharacterCtrl", "OnDelOK");
	end
end

function ChooseCharacterCtrl.OnDel3(go)
	if ChooseCharacterPanel.count > 2 then
		Network.instance:delAvatar(ChooseCharacterPanel.avatarNameList[2].transform:GetComponent("Text").text, "ChooseCharacterCtrl", "OnDelOK");
	end
end

function ChooseCharacterCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.CreateCharacter);
end