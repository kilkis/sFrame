require "Common/define"

BattleButtonCtrl = {}
local this = BattleButtonCtrl;

local panel;
local login;
local transform;
local gameObject;

function BattleButtonCtrl.New()
	return this;
end

function BattleButtonCtrl.Awake()
	panelMgr:CreatePanel('ugui/BattleButtonPanel', this.OnCreate);
end

function BattleButtonCtrl.OnCreate(obj)
	gameObject = obj;
	transform = obj.transform;
	lb = transform:GetComponent("LuaBehaviour");

	lb:AddClick(BattleButtonPanel.btn0, this.OnClick0);
	lb:AddClick(BattleButtonPanel.btn1, this.OnClick1);
	lb:AddClick(BattleButtonPanel.btn2, this.OnClick2);
	lb:AddClick(BattleButtonPanel.btn3, this.OnClick3);
	lb:AddClick(BattleButtonPanel.btn4, this.OnClick4);

end

--µ¥»÷ÊÂ¼þ--
function BattleButtonCtrl.OnClick0(go)
	log("click 0");
	Network.instance:useTargetSkill(1);
end

function BattleButtonCtrl.OnClick1(go)
	log("click 1");
end

function BattleButtonCtrl.OnClick2(go)
	log("click 2");
end

function BattleButtonCtrl.OnClick3(go)
	log("click 3");
end

function BattleButtonCtrl.OnClick4(go)
	log("click 4");
end

function BattleButtonCtrl.CloseTogether()
	destroy(gameObject);
end

function BattleButtonCtrl.Close()
	panelMgr:ClosePanel(CtrlNames.BattleButton);
end