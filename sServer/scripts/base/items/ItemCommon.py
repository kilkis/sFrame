# -*- coding: utf-8 -*-
import KBEngine
import random
import GlobalConst
from KBEDebug import * 
from items.base.ItemBase import ItemBase

class ItemCommon(ItemBase):
	def __init__(self):
		ItemBase.__init__(self)

	def loadFromDict(self, dictDatas):
		"""
		virtual method.
		从字典中创建这个对象
		"""
		ItemBase.loadFromDict(self, dictDatas)
		
	def canUse(self, user):
		return GlobalConst.GC_OK
		
	def use(self, user):
		return GlobalConst.GC_OK