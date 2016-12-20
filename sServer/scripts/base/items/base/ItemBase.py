

class ItemBase:
	"""物品基类"""
	def __init__(self):
		NONE_TYPE = -1
		STAFF_TYPE = 2

	def loadFromDict(self, dictDatas):
		"""
		virtual method.
		从字典中创建这个对象
		"""
		self._id = dictDatas.get('id', 0)
		self._maxnum = dictDatas.get('maxnum', 99)
		self._life = dictDatas.get('life', 0)
		self._career = dictDatas.get('career', 0)
	
	def getID(self):
		return self._id
