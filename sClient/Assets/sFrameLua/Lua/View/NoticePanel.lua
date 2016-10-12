local transform;
local gameObject;

NoticePanel = {};
local this = NoticePanel;

function NoticePanel.Awake(obj)
	gameObject = obj;
	transform = obj.transform;

	this.InitPanel();
end

function NoticePanel.InitPanel()
	this.btnClose = transform:FindChild("Close").gameObject;
end

function NoticePanel.OnDestroy()
	print("NoticePanel onDestroy>>>>>>>>>>>");
end