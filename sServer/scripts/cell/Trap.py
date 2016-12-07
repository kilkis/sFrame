# -*- coding: utf-8 -*-
import random
import math
import time
import KBEngine
from KBEDebug import *
import SCDefine
from interfaces.GameObject import GameObject

class Trap(KBEngine.Entity, GameObject):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		GameObject.__init__(self) 
		
		self.life = 1
		self.inv = 1
		self.invTimes = 1
		
		if self.life == 0 and self.invTimes == 0:
			ERROR_MSG("trap life == 0 && invTimes == 0")
			#有问题，需要删除trap
		
		self.timerID = self.addTimer(0.1, 0.1, SCDefine.TIMER_TYPE_HEARDBEAT)
		
		ERROR_MSG("trap init")
	
	def onEnterSpace(self):
		ERROR_MSG("trap enter space")
		#增加范围监测
		self.territoryControllerID = self.addProximity(10, 0, 0)
		
	def onDestroy(self):
		ERROR_MSG("trap detroy")
	
	def onTimer(self):
		result = False
		self.life = self.life - 0.1
		ERROR_MSG("trap life:%f"%(self.life))
		if self.life <= 0:
			result = True
		
		if result == True:
			#生命周期结束或者次数满足后去除范围监控
			if self.territoryControllerID > 0:
				self.cancelController(self.territoryControllerID)
				self.territoryControllerID = 0
			if self.timerID > 0:
				self.delTimer(self.timerID)
				self.timerID = 0
			#删除自己
			self.destroy()
			return
		GameObject.onTimer(self, tid, userArg)
	
		
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