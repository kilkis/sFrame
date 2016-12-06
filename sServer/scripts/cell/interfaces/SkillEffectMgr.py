# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 
import skillbases.SCObject as SCObject
import d_skill
import d_skillEffect

from skilleffects.se_directDamage import se_directDamage
from skilleffects.se_followFlyer import se_followFlyer

SE_DIRECT 						= 0x001
SE_FOLLOWFLYER					= 0x002
SE_FLYER						= 0x003
SE_TRAP							= 0x004

SE_DIRECTDAMAGE					= 0x001

class SkillEffectMgr:
	def __init__(self):
		self.ses = {}
		self.needdel = []
	
	def onTimer(self, tid, userArg):
		sescopy = self.ses.copy()
		for key,value in sescopy.items():
			lenv = len(value)
			self.needdel = []
			#ERROR_MSG("len:%i"%(lenv))
			for i in range(0, lenv):
				if value[i].logicUpdate(self):
					self.ses[key].remove(value[i])
				
	#这个是一个类似通用的接口，直接效果可以调用，飞行物命中后，陷阱命中后都可以调用
	#以增加效果到entity身上
	def pushSE(self, casterID, sid, seType, props):
		ERROR_MSG("push se:%i, %i"%(seType, SE_DIRECTDAMAGE))
		if seType not in self.ses:
			self.ses[seType] = []
		seData = d_skillEffect.datas.get(seType)
		if seData['type'] == SE_DIRECTDAMAGE:
			props['vs'] = seData['v']
			self.ses[seType].append(se_directDamage(casterID, sid, props))
		
	def createFollowFlyerSE(self, casterID, sid, props):
		ERROR_MSG("push folloy flyer se:%i"%(SE_FOLLOWFLYER))
		if SE_FOLLOWFLYER not in self.ses:
			self.ses[SE_FOLLOWFLYER] = []
		self.ses[SE_FOLLOWFLYER].append(se_followFlyer(casterID, sid, props))
		
	def popSE(self, seType):
		ERROR_MSG("push se:%i"%(seType))
		
	def use(self, sid, caster, scObject):
		ERROR_MSG("use skill:%i"%(sid))
		sid = 2
		props = {}
		skillData = d_skill.datas.get(sid)
		if skillData['skillType'] == SE_DIRECT:
			hs = skillData['h'].split(',')
			for h in hs:
				scObject.pushSE(caster.id, sid, int(h), props)
		elif skillData['skillType'] == SE_FOLLOWFLYER:
			props['startpos'] = caster.position
			props['speed'] = 1.0
			props['hs'] = skillData['h']
			scObject.createFollowFlyerSE(caster.id, sid, props)
		elif skillData['skillType'] == SE_FLYER:
			pass
		elif skillData['skillType'] == SE_TRAP:
			pass
		
	def canUse(self, sid, caster, scObject):
		ERROR_MSG("skill effecg mgr: canUSE")
		skillData = d_skill.datas.get(sid)
		if skillData['career'] != caster.roleTypeCell:
			return GlobalConst.GC_SKILL_CAREER_NOT_RIGHT
		return GlobalConst.GC_OK
		
		