local transform;
local gameObject;

BattleButtonPanel = {};
local this = BattleButtonPanel;

function BattleButtonPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function BattleButtonPanel.InitPanel()
	this.btn0 = transform:FindChild("Button").gameObject;
	this.btn1 = transform:FindChild("Button1").gameObject;
	this.btn2 = transform:FindChild("Button2").gameObject;
	this.btn3 = transform:FindChild("Button3").gameObject;
	this.btn4 = transform:FindChild("Button4").gameObject;
end

function BattleButtonPanel.OnDestroy()
	print("BattleButtonPanel onDestroy>>>>>>>>>>>");
end