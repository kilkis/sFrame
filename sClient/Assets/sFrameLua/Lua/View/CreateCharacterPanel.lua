local transform;
local gameObject;

CreateCharacterPanel = {};
local this = CreateCharacterPanel;

function CreateCharacterPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function CreateCharacterPanel.InitPanel()
	this.btnEnter = transform:FindChild("NameEnter/Enter").gameObject;
	this.avatarNameList = {};
	this.avatarNameList[0] = transform:FindChild("Image/Tooltip/CName1").gameObject;
	this.avatarNameList[1] = transform:FindChild("Image/Tooltip1/CName2").gameObject;
	this.avatarNameList[2] = transform:FindChild("Image/Tooltip2/CName3").gameObject;
	
	this.avatarNameList[0].transform:GetComponent("Text").text = "warrior";
	this.avatarNameList[1].transform:GetComponent("Text").text = "magic";
	this.avatarNameList[2].transform:GetComponent("Text").text = "prist";

	this.avatarBtn = {}
	this.avatarBtn[0] = transform:FindChild("Image/Tooltip").gameObject;
	this.avatarBtn[1] = transform:FindChild("Image/Tooltip1").gameObject;
	this.avatarBtn[2] = transform:FindChild("Image/Tooltip2").gameObject;
	
	this.textName = transform:FindChild("NameEnter/InputField/Text").gameObject;

end

function CreateCharacterPanel.Hide()
	panelMgr:hideTransformPosition("CreateCharacterPanel", transform);
end

function CreateCharacterPanel.Show()
	panelMgr:showTransformPosition("CreateCharacterPanel", transform);
end

function CreateCharacterPanel.OnDestroy()
	log("CreateCharacterPanel onDestroy>>>>>>>>>>>");
end