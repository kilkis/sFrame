local transform;
local gameObject;

LoginInfoPanel = {};
local this = LoginInfoPanel;

function LoginInfoPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function LoginInfoPanel.InitPanel()
	this.btnLogin = transform:FindChild("LoginBtn").gameObject;
	this.btnClose = transform:FindChild("close").gameObject;
	
	this.textUsrName = transform:FindChild("userInputField/userNameText").gameObject;
	this.textPassword = transform:FindChild("pwdInputField/passWordText").gameObject;
end

function LoginInfoPanel.OnDestroy()
	log("LoginInfoPanel onDestroy>>>>>>>>>>>");
end