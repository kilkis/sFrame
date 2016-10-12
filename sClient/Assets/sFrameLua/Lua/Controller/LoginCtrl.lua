require "Common/define"

LoginCtrl = {}
local this = LoginCtrl;

local panel;
local login;
local transform;
local gameObject;

function LoginCtrl.New()
	return this;
end

function LoginCtrl.Awake()
	panelMgr:CreatePanel('ugui/LoginPanel', this.OnCreate);
end

function LoginCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;

	login = transform:GetComponent("LuaBehaviour");

	login:AddClick(LoginPanel.btnLogin, this.OnClick);
	login:AddClick(LoginPanel.btnCreate, this.OnCreateClick);
end

--µ¥»÷ÊÂ¼þ--
function LoginCtrl.OnClick(go)
	
	--Loading.instance:showLoading();

	local ctrl = CtrlManager.GetCtrl(CtrlNames.LoginInfo);
	if ctrl ~= nil then
		ctrl:Awake();
	end

	--Flow.instance:changeNextState();

	--destroy(gameObject);
end

function LoginCtrl.OnCreateClick(go)
	local ctrl = CtrlManager.GetCtrl(CtrlNames.LoginCreate);
	if ctrl ~= nil then
		ctrl:Awake();
	end
end


function LoginCtrl.CloseTogether()
	destroy(gameObject);
end


function LoginCtrl.Close()
	print("destroy login!!!!!!!!!");
	
	panelMgr:ClosePanel(CtrlNames.Login);
end