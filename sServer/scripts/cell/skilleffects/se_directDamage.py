# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 
import skillbases.SCObject as SCObject

from skilleffects.base.se_base import se_base

class se_directDamage(se_base):
	def __init__(self, castID, sid, props):
		se_base.__init__(self, castID, sid, props)
		ERROR_MSG("se_directDamage init")
	
	def logicUpdate(self, recv):
		ERROR_MSG("se_directDamage logicUpdate")
		recv.recvDamage(self.castID, self.sid, 0, 10)
		return True