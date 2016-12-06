# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 

class se_base:
	def __init__(self, castID, sid, props):
		self.castID = castID
		self.sid = sid
		self.props = props
		self.curTime = 0
		
	def logicUpdate(self, recv):
		return False
		
	def distToDelay(self, fromPos, toEntity, speed):
		"""
		"""
		if speed > 0.0:#1.0
			ERROR_MSG("dis:%f"%(fromPos.distTo(toEntity.position)))
			return fromPos.distTo(toEntity.position) / speed
		return 0.0