# -*- coding: utf-8 -*-
import random
import math
import time
import KBEngine
from KBEDebug import *
import SCDefine
from interfaces.Combat import Combat
from interfaces.NPCObject import NPCObject
from interfaces.Motion import Motion
from interfaces.SkillEffectMgr import SkillEffectMgr
from interfaces.State import State

class Trap(KBEngine.Entity, NPCObject, Motion, SkillEffectMgr, Combat, State):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		Combat.__init__(self)
		State.__init__(self) 
		SkillEffectMgr.__init__(self)
		
		self.territoryControllerID = self.addProximity(10, 0, 0)
		if self.territoryControllerID > 0:
			self.cancelController(self.territoryControllerID)
			self.territoryControllerID = 0
		
	def isFlyer(self):
		return True
		
	def onTimer(self, tid, userArg):
		#SkillEffectMgr.onTimer(self, tid, userArg)
		pass
	
	#暂时还是默认使用系统的方形算法来确认敌人是否在范围内
	def onEnterTrap(self, entityEntering, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		有entity进入trap
		"""
		if controllerID != self.territoryControllerID:
			return
		
		#if entityEntering.isDestroyed or entityEntering.getScriptName() != "Avatar" or entityEntering.isDead():
		#	return
		
			
		ERROR_MSG("%s::onEnterTrap: %i entityEntering=(%s)%i, range_xz=%s, range_y=%s, controllerID=%i, userarg=%i" % \
						(self.getScriptName(), self.id, entityEntering.getScriptName(), entityEntering.id, \
						range_xz, range_y, controllerID, userarg))
		
		
	def onLeaveTrap(self, entityLeaving, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		有entity离开trap
		"""
		if controllerID != self.territoryControllerID:
			return
		
		#if entityLeaving.isDestroyed or entityLeaving.getScriptName() != "Avatar" or entityLeaving.isDead():
		#	return
			
		ERROR_MSG("%s::onLeaveTrap: %i entityLeaving=(%s)%i." % (self.getScriptName(), self.id, \
				entityLeaving.getScriptName(), entityLeaving.id))