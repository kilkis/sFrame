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

class Flyer(KBEngine.Entity, NPCObject, Motion, SkillEffectMgr, Combat, State):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		Motion.__init__(self)
		Combat.__init__(self)
		State.__init__(self) 
		SkillEffectMgr.__init__(self)
		
	def isFlyer(self):
		return True
		
	def onTimer(self, tid, userArg):
		#SkillEffectMgr.onTimer(self, tid, userArg)
		pass