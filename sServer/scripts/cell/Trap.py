# -*- coding: utf-8 -*-
import random
import math
import time
import KBEngine
from KBEDebug import *
import SCDefine
from interfaces.GameObject import GameObject

#Trap基础上可以扩展出有战斗属性的比如：图腾
class Trap(KBEngine.Entity, GameObject):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		GameObject.__init__(self) 
		
		self.init = False
		
		self.castID = 0#释放者ID
		self.life = 3#trap生命
		self.inv = 1#间隔时间
		self.invTimes = 1#间隔次数
		self.effectInv = []#过程中的效果
		self.effectEnd = []#结束时的效果
		
		self.territoryControllerID = 0#trap监测ID
		self.trapEntity = []#trap范围内entity
		
		if self.life == 0 and self.invTimes == 0:
			ERROR_MSG("trap life == 0 && invTimes == 0")
			#有问题，需要删除trap
			self.destroy()
			return
		self.timerID = self.addTimer(0.1, 0.1, SCDefine.TIMER_TYPE_HEARDBEAT)
		
		#临时变量存储trapID没有赋值之前得到的entity信息,主要是为了应对kbe的机制
		self.tmpB = False
		self.tmpTrapID = 0
		self.tmpTrap = []
		#一旦trapID正常，以上就失去作用
		
		ERROR_MSG("trap init")

	def onDestroy(self):
		ERROR_MSG("trap detroy")
	
	def isEnemy(self, entityC):
		sname = entityC.getScriptName()
		if sname == "Avatar" or sname == "NPC" or sname == "Monster":
			if entityC.isDead():
				return False
			return True
		return False
	
	def onTimer(self, tid, userArg):
		if not self.init:
			self.init = True
			#增加范围监测
			self.territoryControllerID = self.addProximity(10, 0, 0)
		result = False
		self.life = self.life - 0.1
		ERROR_MSG("trap life:%f"%(self.life))
		if self.life <= 0:
			result = True
		if self.tmpB and self.territoryControllerID > 0:
			#用bool值做简化判定，是否有数据需要处理，如果有，遍历增加到标准数组内
			self.tmpB = False
			for e in self.tmpTrap:
				self.trapEntity.append(e)
			self.tmpTrap = None
		if result == True:
			#生命周期结束或者次数满足后去除范围监控
			ERROR_MSG("trap nums:%i"%(len(self.trapEntity)))
			self.trapEntity = None
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
		#暂时没有判定是什么类型的entity，到时候需要增加判定
		#放到最上方判定，为了处理临时数据方便
		if entityEntering.isDestroyed or not self.isEnemy(entityEntering):
			return
		#or entityEntering.getScriptName() != "Avatar"
		if self.territoryControllerID == 0 and controllerID != 0:
			ERROR_MSG("on enter trap:%i, %i, %s"%(controllerID, self.territoryControllerID, entityEntering.getScriptName()))
			self.tmpB = True
			self.tmpTrap.append(entityEntering)
		if controllerID != self.territoryControllerID:
			return
			
		ERROR_MSG("%s::onEnterTrap: %i entityEntering=(%s)%i, range_xz=%s, range_y=%s, controllerID=%i, userarg=%i" % \
						(self.getScriptName(), self.id, entityEntering.getScriptName(), entityEntering.id, \
						range_xz, range_y, controllerID, userarg))
		self.trapEntity.append(entityEntering)
		
	def onLeaveTrap(self, entityLeaving, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		有entity离开trap
		"""
		if entityLeaving.isDestroyed or not self.isEnemy(entityEntering):
			return
		if self.territoryControllerID == 0 and controllerID != 0:
			ERROR_MSG("on leave trap:%i, %i"%(controllerID, self.territoryControllerID))
			self.tmpB = True
			self.tmpTrap.remove(entityLeaving)
		if controllerID != self.territoryControllerID:
			return
		ERROR_MSG("%s::onLeaveTrap: %i entityLeaving=(%s)%i." % (self.getScriptName(), self.id, \
				entityLeaving.getScriptName(), entityLeaving.id))
		self.trapEntity.remove(entityEntering)
		