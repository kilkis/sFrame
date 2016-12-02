# -*- coding: utf-8 -*-
import random
import math
import time
import KBEngine
from KBEDebug import *
from interfaces.Combat import Combat
from interfaces.NPCObject import NPCObject
from interfaces.Motion import Motion
from interfaces.State import State

class NPC(KBEngine.Entity, NPCObject, Motion, Combat, State):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		NPCObject.__init__(self)
		Motion.__init__(self)
		Combat.__init__(self)
		State.__init__(self) 
		
		ERROR_MSG("npc init, id is :%i"%(self.mid))

	def initEntity(self):
		"""
		virtual method.
		"""
		pass

	def checkInTerritory(self):
		"""
		virtual method.
		检查自己是否在可活动领地中
		"""
		return AI.checkInTerritory(self)
		
	def isNPC(self):
		"""
		virtual method.
		"""
		return True

	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		#DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.getScriptName(), self.id, tid, userArg))
		NPCObject.onTimer(self, tid, userArg)
	
	def onForbidChanged_(self, forbid, isInc):
		"""
		virtual method.
		entity禁止 条件改变
		@param isInc		:	是否是增加
		"""
		#State.onForbidChanged_(self, forbid, isInc)
		pass
	
	def onStateChanged_(self, oldstate, newstate):
		"""
		virtual method.
		entity状态改变了
		"""
		State.onStateChanged_(self, oldstate, newstate)
		#AI.onStateChanged_(self, oldstate, newstate)
		NPCObject.onStateChanged_(self, oldstate, newstate)
		
	def onSubStateChanged_(self, oldSubState, newSubState):
		"""
		virtual method.
		子状态改变了
		"""
		State.onSubStateChanged_(self, oldSubState, newSubState)
		#AI.onSubStateChanged_(self, oldSubState, newSubState)

	def onFlagsChanged_(self, flags, isInc):
		"""
		virtual method.
		"""
		#Flags.onFlagsChanged_(self, flags, isInc)
		#AI.onFlagsChanged_(self, flags, isInc)
		pass

	def onEnterTrap(self, entity, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		引擎回调进入陷阱触发
		"""
		#AI.onEnterTrap(self, entity, range_xz, range_y, controllerID, userarg)
		pass

	def onLeaveTrap(self, entity, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		引擎回调离开陷阱触发
		"""
		#AI.onLeaveTrap(self, entity, range_xz, range_y, controllerID, userarg)
		pass

	def onAddEnemy(self, entityID):
		"""
		virtual method.
		有敌人进入列表
		"""
		#AI.onAddEnemy(self, entityID)
		Combat.onAddEnemy(self, entityID)

	def onRemoveEnemy(self, entityID):
		"""
		virtual method.
		删除敌人
		"""
		#AI.onRemoveEnemy(self, entityID)
		Combat.onRemoveEnemy(self, entityID)

	def onEnemyEmpty(self):
		"""
		virtual method.
		敌人列表空了
		"""
		#AI.onEnemyEmpty(self)
		Combat.onEnemyEmpty(self)
		
	def onDestroy(self):
		"""
		entity销毁
		"""
		NPCObject.onDestroy(self)
		Combat.onDestroy(self)
