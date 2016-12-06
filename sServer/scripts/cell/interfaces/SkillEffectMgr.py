# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 
import skillbases.SCObject as SCObject
from skilleffects.se_directDamage import se_directDamage
from skilleffects.se_followFlyer import se_followFlyer

SE_DIRECTDAMAGE 				= 0x001
SE_FOLLOWFLYER					= 0x002

class SkillEffectMgr:
	def __init__(self):
		self.ses = {}
		self.needdel = []
	
	def onTimer(self, tid, userArg):
		for key,value in self.ses.items():
			lenv = len(value)
			self.needdel = []
			for i in range(0, lenv):
				if value[i].logicUpdate(self):
					self.needdel.append(value[i])
			lenv = len(self.needdel)
			for i in range(0, lenv):
				value.remove(self.needdel[i])
		
	def pushSE(self, casterID, sid, seType, props):
		ERROR_MSG("push se:%i"%(seType))
		if seType not in self.ses:
			self.ses[seType] = []
		if seType == SE_DIRECTDAMAGE:
			self.ses[seType].append(se_directDamage(casterID, sid, props))
		elif seType == SE_FOLLOWFLYER:
			self.ses[seType].append(se_followFlyer(casterID, sid, props))
	
	def popSE(self, seType):
		ERROR_MSG("push se:%i"%(seType))
		
		
	def use(self, sid, caster, scObject):
		ERROR_MSG("use skill:%i"%(sid))
		props = {}
		#scObject.pushSE(caster.id, sid, SE_DIRECTDAMAGE, {})
		props['startpos'] = caster.position
		props['speed'] = 1.0
		scObject.pushSE(caster.id, sid, SE_FOLLOWFLYER, props)
		