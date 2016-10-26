# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import d_spaces
import d_mapinfo

class SpaceContext(dict):
	"""
	产生space上下文
	"""
	def __init__(self, entity):
		pass
	
	@staticmethod
	def create(entity):
		return {}


class SpaceDuplicateContext(SpaceContext):
	"""
	产生space副本的上下文
	进入副本需要持有钥匙（spaceKey）
	"""
	def __init__(self, entity):
		SpaceContext.__init__(self, entity)

	@staticmethod
	def create(entity):
		return {"spaceKey" : entity.dbid}
		
def createContext(entity, spaceUType):
	"""
	"""
	spaceData = d_mapinfo.datas.get(spaceUType)
	
	return {
		0 : SpaceContext,
		1 : SpaceDuplicateContext,
	}[spaceData["type"]].create(entity)