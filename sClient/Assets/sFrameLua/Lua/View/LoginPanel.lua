local transform;
local gameObject;

LoginPanel = {};
local this = LoginPanel;

function LoginPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function LoginPanel.InitPanel()
	this.btnLogin = transform:FindChild("Login").gameObject;
	this.btnCreate = transform:FindChild("create").gameObject;
end

function LoginPanel.OnDestroy()
	print("LoginPanel onDestroy>>>>>>>>>>>");
end