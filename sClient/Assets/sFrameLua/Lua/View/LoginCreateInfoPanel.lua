local transform;
local gameObject;

LoginCreateInfoPanel = {};
local this = LoginCreateInfoPanel;

function LoginCreateInfoPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function LoginCreateInfoPanel.InitPanel()
	this.btnLogin = transform:FindChild("CreateBtn").gameObject;
	this.btnClose = transform:FindChild("close").gameObject;

	this.textUsrName = transform:FindChild("userInputField/userNameText").gameObject;
	this.textPassword = transform:FindChild("pwdInputField/passWordText").gameObject;
end

function LoginCreateInfoPanel.OnDestroy()
	print("LoginCreateInfoPanel onDestroy>>>>>>>>>>>");
end