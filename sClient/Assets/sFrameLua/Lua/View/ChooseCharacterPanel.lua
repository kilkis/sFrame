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
	
	this.delBtn = {}
	this.delBtn[0] = transform:FindChild("Image/Del1").gameObject;
	this.delBtn[1] = transform:FindChild("Image/Del2").gameObject;
	this.delBtn[2] = transform:FindChild("Image/Del3").gameObject;

	this.avatarInfo = {}

	this.avatarList = Network.instance:getAvatarList();
	local iter = this.avatarList:GetEnumerator();
	this.count = 0;
	while iter:MoveNext() do
		local v = iter.Current.Value;
		this.avatarNameList[this.count].transform:GetComponent("Text").text = v.name;
		this.avatarInfo[this.count] = v;
		this.delBtn[this.count]:SetActive(true);
		this.count = this.count + 1;
	end
end

function ChooseCharacterPanel.Reset()
	log("reset");
	this.avatarInfo = {}
	for i = 0,  2 do
		this.avatarNameList[i].transform:GetComponent("Text").text = "";
		this.delBtn[i]:SetActive(false);
	end
	local iter = this.avatarList:GetEnumerator();
	this.count = 0;
	while iter:MoveNext() do
		local v = iter.Current.Value;
		this.avatarNameList[this.count].transform:GetComponent("Text").text = v.name;
		this.avatarInfo[this.count] = v;
		this.delBtn[this.count]:SetActive(true);
		this.count = this.count + 1;
	end
end


function ChooseCharacterPanel.Hide()
	panelMgr:hideTransformPosition("ChooseCharacterPanel", transform);
end

function ChooseCharacterPanel.Show()
	panelMgr:showTransformPosition("ChooseCharacterPanel", transform);
end

function ChooseCharacterPanel.OnDestroy()
	log("ChooseCharacterPanel onDestroy>>>>>>>>>>>");
end