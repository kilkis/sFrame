local transform;
local gameObject;

ChooseCharacterPanel = {};
local this = ChooseCharacterPanel;

function ChooseCharacterPanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function ChooseCharacterPanel.InitPanel()
	this.btnEnter = transform:FindChild("Choose").gameObject;
	this.avatarNameList = {};
	this.avatarNameList[0] = transform:FindChild("Image/Tooltip/CName1").gameObject;
	this.avatarNameList[1] = transform:FindChild("Image/Tooltip1/CName2").gameObject;
	this.avatarNameList[2] = transform:FindChild("Image/Tooltip2/CName3").gameObject;

	this.avatarBtn = {}
	this.avatarBtn[0] = transform:FindChild("Image/Tooltip").gameObject;
	this.avatarBtn[1] = transform:FindChild("Image/Tooltip1").gameObject;
	this.avatarBtn[2] = transform:FindChild("Image/Tooltip2").gameObject;

	this.avatarInfo = {}

	this.avatarList = Network.instance:getAvatarList();
	local iter = this.avatarList:GetEnumerator();
	this.count = 0;
	while iter:MoveNext() do
		local v = iter.Current.Value;
		this.avatarNameList[this.count].transform:GetComponent("Text").text = v.name;
		this.avatarInfo[this.count] = v;
		this.count = this.count + 1;
	end
end

function ChooseCharacterPanel.OnDestroy()
	log("ChooseCharacterPanel onDestroy>>>>>>>>>>>");
end