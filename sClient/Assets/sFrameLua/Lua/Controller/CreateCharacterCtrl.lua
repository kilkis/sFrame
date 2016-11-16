require "Common/define"

CreateCharacterCtrl = {}
local this = CreateCharacterCtrl;

local panel;
local login;
local transform;
local gameObject;

local ffi = require("ffi")

function CreateCharacterCtrl.New()
	return this;
end

function CreateCharacterCtrl.Awake()
	panelMgr:CreatePanel('ugui/CreateCharacterPanel', this.OnCreate);
end

function CreateCharacterCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	lb = transform:GetComponent("LuaBehaviour");

	lb:AddClick(CreateCharacterPanel.btnEnter, this.OnClick);
	lb:AddClick(CreateCharacterPanel.avatarBtn[0], this.OnChoose1);
	lb:AddClick(CreateCharacterPanel.avatarBtn[1], this.OnChoose2);
	lb:AddClick(CreateCharacterPanel.avatarBtn[2], this.OnChoose3);

	this.selAvatarDBID = ffi.new("uint64_t");
	
	Loading.instance:hideLoading();
end

--µ¥»÷ÊÂ¼þ--
function CreateCharacterCtrl.OnClick(go)
	if CreateCharacterPanel.textName.transform:GetComponent("Text").text == "" then
		log("name is empty");
	else
		Loading.instance:showLoading();
		--Network.instance:sendMsg2Server("selectAvatarGame", this.selAvatarDBID);
		Network.instance:selAvatar(this.selAvatarDBID, "CreateCharacterCtrl", "OnSelOK");
	end
end

function CreateCharacterCtrl.OnSelOK(go)
	destroy(gameObject);
	Flow.instance:changeNextState();
end


function CreateCharacterCtrl.OnChoose1(go)
	if CreateCharacterPanel.count > 0 then
		log(CreateCharacterPanel.avatarInfo[0].name.." : "..CreateCharacterPanel.avatarInfo[0].dbid);
		this.selAvatarDBID = CreateCharacterPanel.avatarInfo[0].dbid;
	end
end

function CreateCharacterCtrl.OnChoose2(go)
	if CreateCharacterPanel.count > 1 then
		log(CreateCharacterPanel.avatarInfo[1].name);
	end
end

function CreateCharacterCtrl.OnChoose3(go)
	if CreateCharacterPanel.count > 2 then
		log(CreateCharacterPanel.avatarInfo[2].name);
	end
end

function CreateCharacterCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.CreateCharacter);
end