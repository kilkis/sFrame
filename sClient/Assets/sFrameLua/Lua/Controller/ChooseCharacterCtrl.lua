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

	this.selAvatarDBID = ffi.new("uint64_t");
	
	Loading.instance:hideLoading();
end

--µ¥»÷ÊÂ¼þ--
function ChooseCharacterCtrl.OnClick(go)
	print("click");
	Loading.instance:showLoading();
	--Network.instance:sendMsg2Server("selectAvatarGame", this.selAvatarDBID);
	Network.instance:selAvatar(this.selAvatarDBID, "ChooseCharacterCtrl", "OnSelOK");
end

function ChooseCharacterCtrl.OnSelOK(go)
	destroy(gameObject);
	Flow.instance:changeNextState();
end


function ChooseCharacterCtrl.OnChoose1(go)
	if ChooseCharacterPanel.count > 0 then
		log(ChooseCharacterPanel.avatarInfo[0].name.." : "..ChooseCharacterPanel.avatarInfo[0].dbid);
		this.selAvatarDBID = ChooseCharacterPanel.avatarInfo[0].dbid;
	end
end

function ChooseCharacterCtrl.OnChoose2(go)
	if ChooseCharacterPanel.count > 1 then
		log(ChooseCharacterPanel.avatarInfo[1].name);
	end
end

function ChooseCharacterCtrl.OnChoose3(go)
	if ChooseCharacterPanel.count > 2 then
		log(ChooseCharacterPanel.avatarInfo[2].name);
	end
end

function ChooseCharacterCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.CreateCharacter);
end