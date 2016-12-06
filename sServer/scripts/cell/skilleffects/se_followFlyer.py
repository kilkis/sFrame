# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 
import skillbases.SCObject as SCObject
from skilleffects.base.se_base import se_base

class se_followFlyer(se_base):
	def __init__(self, castID, sid, props):
		se_base.__init__(self, castID, sid, props)
		ERROR_MSG("se_followFlyer init")
		self.curpos = self.props['startpos']
		self.speed = self.props['speed']
		self.delay = 0
		
	def logicUpdate(self, recv):
		#ERROR_MSG("se_followFlyer logicUpdate:%f, %f"%(self.curTime, self.delay))
		#暂时不做动态计算的事情
		if self.delay == 0:
			self.delay = self.distToDelay(self.curpos, recv, self.speed)
			ERROR_MSG("cal delay: %f"%(self.delay))
		self.curTime = self.curTime + 0.1
		if self.curTime >= self.delay:
			hs = self.props['hs'].split(',')
			for h in hs:
				ERROR_MSG("transfrom:%i,%i,%i"%(self.castID, self.sid, int(h)))
				recv.pushSE(self.castID, self.sid, int(h), self.props)
				#recv.recvDamage(self.castID, self.sid, 0, 10)
			return True
		return False