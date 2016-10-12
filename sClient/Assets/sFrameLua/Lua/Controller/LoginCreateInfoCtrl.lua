require "Common/define"

LoginCreateInfoCtrl = {}
local this = LoginCreateInfoCtrl;

local panel;
local login;
local transform;
local gameObject;

function LoginCreateInfoCtrl.New()
	return this;
end

function LoginCreateInfoCtrl.Awake()
	panelMgr:CreatePanel('ugui/LoginCreateInfoPanel', this.OnCreate);
	
end

function LoginCreateInfoCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;

	login = transform:GetComponent("LuaBehaviour");

	login:AddClick(LoginCreateInfoPanel.btnLogin, this.OnClick);
	login:AddClick(LoginCreateInfoPanel.btnClose, this.OnClose);
end

--µ¥»÷ÊÂ¼þ--
function LoginCreateInfoCtrl.OnClick(go)
	Network.instance:createAccount(LoginCreateInfoPanel.textUsrName.transform:GetComponent("Text").text, LoginCreateInfoPanel.textPassword.transform:GetComponent("Text").text, "LoginCreateInfoCtrl", "OnServerCB");
end

function LoginCreateInfoCtrl.OnServerCB()
	local ctrl = CtrlManager.GetCtrl(CtrlNames.Login);
	if ctrl ~= nil then
		ctrl:CloseTogether();
	end
		
	Loading.instance:showLoading();

	local ctrl = CtrlManager.GetCtrl(CtrlNames.CreateCharacter);
	if ctrl ~= nil then
		ctrl:Awake();
	end

	Flow.instance:changeNextState();

	destroy(gameObject);
end

function LoginCreateInfoCtrl.OnClose(go)
	destroy(gameObject);
end

function LoginCreateInfoCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.LoginCreate);
end