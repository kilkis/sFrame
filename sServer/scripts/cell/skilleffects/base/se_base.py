# -*- coding: utf-8 -*-
import KBEngine
import GlobalConst
import SCDefine
from KBEDebug import * 

class se_base:
	def __init__(self, castID, sid, props):
		self.castID = castID
		self.sid = sid
		
	def logicUpdate(self, recv):
		return False
