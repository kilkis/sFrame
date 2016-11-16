require "Common/define"

LoginInfoCtrl = {}
local this = LoginInfoCtrl;

local panel;
local login;
local transform;
local gameObject;

function LoginInfoCtrl.New()
	return this;
end

function LoginInfoCtrl.Awake()
	panelMgr:CreatePanel('ugui/LoginInfoPanel', this.OnCreate);
	
end

function LoginInfoCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;

	login = transform:GetComponent("LuaBehaviour");
	
	login:AddClick(LoginInfoPanel.btnLogin, this.OnClick);
	login:AddClick(LoginInfoPanel.btnClose, this.OnClose);
end

--µ¥»÷ÊÂ¼þ--
function LoginInfoCtrl.OnClick(go)
	--log(LoginInfoPanel.textUsrName.transform:GetComponent("Text").text);

	Network.instance:login(LoginInfoPanel.textUsrName.transform:GetComponent("Text").text, LoginInfoPanel.textPassword.transform:GetComponent("Text").text, "LoginInfoCtrl", "OnServerCB");
end

function LoginInfoCtrl.OnServerCB()
	local ctrl = CtrlManager.GetCtrl(CtrlNames.Login);
	if ctrl ~= nil then
		ctrl:CloseTogether();
	end
		
	Loading.instance:showLoading();

	local ctrl = CtrlManager.GetCtrl(CtrlNames.ChooseCharacter);
	if ctrl ~= nil then
		ctrl:Awake();
	end

	Flow.instance:changeNextState();

	destroy(gameObject);
end

function LoginInfoCtrl.OnClose(go)
	destroy(gameObject);
end

function LoginInfoCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.LoginInfo);
end