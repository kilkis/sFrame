require "Common/define"

NoticeCtrl = {}
local this = NoticeCtrl;

local panel;
local login;
local transform;
local gameObject;

function NoticeCtrl.New()
	return this;
end

function NoticeCtrl.Awake()
	panelMgr:CreatePanel('ugui/NoticePanel', this.OnCreate);
end

function NoticeCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	notice = transform:GetComponent("LuaBehaviour");

	notice:AddClick(NoticePanel.btnClose, this.OnClick);

	Loading.instance:hideLoading();
end

--µ¥»÷ÊÂ¼þ--
function NoticeCtrl.OnClick(go)
	print("click");
	local ctrl = CtrlManager.GetCtrl(CtrlNames.Login);
	if ctrl ~= nil then
		ctrl:Awake();
	end
	destroy(gameObject);
end

function NoticeCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.Notice);
end