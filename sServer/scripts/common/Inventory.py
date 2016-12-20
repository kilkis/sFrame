import weakref
import KBEngine
from KBEDebug import *

import d_items
from ITEM_INFO import TItemInfo

import d_commonconfig

class InventoryMgr:
	"""docstring for InventoryMgr"""
	#NOITEM = -1
	MAX_RESOURCEID = 100
	MIN_MISSIONID = 10000
	MAX_MISSIONID = 99999
	
	def __init__(self, entity):
		self._entity = weakref.proxy(entity)
		#self._curItemIndex = NOITEM
		
		#背包格子
		self.maxBagNum = d_commonconfig.datas['bagFreeNum'] + self._entity.buyBagNum
		self.maxBagBuyNum = d_commonconfig.datas['bagBuyNum']
		#装备格子总数
		self.equipNum = d_commonconfig.datas['equipNum']
		#仓库格子
		self.maxWarehouseNum = d_commonconfig.datas['warehouseFreeNum'] + self._entity.buyWarehouseNum
		self.maxWarehouseBuyNum = d_commonconfig.datas['warehouseBuyNum']
		#任务背包格子
		self.maxMissionBagNum = d_commonconfig.datas['missionBagNum']
		
		#初始化背包索引index to Uid
		self.invIndex2Uids = [0]*self.maxBagNum
		for key, info in self._entity.itemList.items():
			self.invIndex2Uids[info[3]] = key
			
		#初始化任务道具背包索引index to Uid
		self.missionIndex2Uids = [0]*self.maxMissionBagNum
		for key, info in self._entity.missionItemList.items():
			self.missionIndex2Uids[info[3]] = key

		#初始化仓库道具背包索引index to Uid
		self.warehouseIndex2Uids = [0]*self.maxWarehouseNum
		for key, info in self._entity.warehouseItemList.items():
			self.warehouseIndex2Uids[info[3]] = key
			
		#初始化装备索引index to Uid
		self.equipIndex2Uids = [0]*self.equipNum
		for key, info in self._entity.equipItemList.items():
			self.equipIndex2Uids[info[3]] = key
	
	def isResource(self, itemID):
		if itemID < MAX_RESOURCEID:
			return True
		return False
	
	def isMissionItem(self, itemID):
		if itemID >= MIN_MISSIONID and itemID <= MIN_MISSIONID:
			return True
		return False
	
	def addItem(self, itemID, itemCount = 1):
		if self.isResource(itemID) or self.isMissionItem(itemID):
			return
		result = []
		emptyIndex = -1
		for i in range(0,self.maxBagNum):
			if self.invIndex2Uids[i] == 0:
				emptyIndex = i
				break
		#背包已经满了
		if emptyIndex == -1:
			return result
		#放置物品
		itemStack = d_items.datas[itemID]['maxnum']
		
		#不可堆叠物品
		if itemStack == 1:
			itemUUID = KBEngine.genUUID64()
			iteminfo = TItemInfo()
			iteminfo.extend([itemUUID, itemID, 1, emptyIndex])
			self.invIndex2Uids[emptyIndex] = itemUUID
			self._entity.itemList[itemUUID] = iteminfo
			result.append(itemUUID)
			
		#可堆叠物品
		else:
			for key, info in self._entity.itemList.items():
				if info[1] == itemID and info[2] < itemStack:
					info[2] += itemCount
					result.append(key)
					if info[2] > itemStack:
						itemCount = info[2]-itemStack
						info[2] = itemStack
					else:
						itemCount = 0
						break

			if itemCount > 0:
				itemUUID = KBEngine.genUUID64()
				iteminfo = TItemInfo()
				iteminfo.extend([itemUUID, itemID, itemCount, emptyIndex])
				self.invIndex2Uids[emptyIndex] = itemUUID
				self._entity.itemList[itemUUID] = iteminfo
				result.append(itemUUID)
			#这里有个bug，当背包里没有此道具，而插入道具数量超过可叠加数量时，会一起放一个格子
		return result
		

	def removeItem(self, itemUUID, itemCount):
		if self.isResource(itemID) or self.isMissionItem(itemID):
			return
		itemId = self._entity.itemList[itemUUID][1]
		itemC = self._entity.itemList[itemUUID][2]
		itemIndex = self._entity.itemList[itemUUID][3]
		if itemCount < itemC:
			self._entity.itemList[itemUUID][2] = itemC - itemCount
			return -1
		else:
			self.invIndex2Uids[itemIndex] = 0
			del self._entity.itemList[itemUUID]
		return itemId
	
	def swapItem(self, srcIndex, dstIndex):
		srcUid = self.invIndex2Uids[srcIndex]
		dstUid = self.invIndex2Uids[dstIndex]
		self.invIndex2Uids[srcIndex] = dstUid
		if dstUid != 0:
			self._entity.itemList[dstUid][3] = srcIndex
		self.invIndex2Uids[dstIndex] = srcUid
		if srcUid != 0:
			self._entity.itemList[srcUid][3] = dstIndex
	
	def addMissionItem(self, itemID, itemCount):
		if not self.isMissionItem(itemID):
			return
		result = []
		emptyIndex = -1
		for i in range(0,self.maxMissionBagNum):
			if self.missionIndex2Uids[i] == 0:
				emptyIndex = i
				break
		#背包已经满了
		if emptyIndex == -1:
			return result
		#放置物品
		itemStack = d_items.datas[itemID]['maxnum']
		
		#不可堆叠物品
		if itemStack == 1:
			itemUUID = KBEngine.genUUID64()
			iteminfo = TItemInfo()
			iteminfo.extend([itemUUID, itemID, 1, emptyIndex])
			self.missionIndex2Uids[emptyIndex] = itemUUID
			self._entity.missionItemList[itemUUID] = iteminfo
			result.append(itemUUID)
			
		#可堆叠物品
		else:
			for key, info in self._entity.missionItemList.items():
				if info[1] == itemID and info[2] < itemStack:
					info[2] += itemCount
					result.append(key)
					if info[2] > itemStack:
						itemCount = info[2]-itemStack
						info[2] = itemStack
					else:
						itemCount = 0
						break

			if itemCount > 0:
				itemUUID = KBEngine.genUUID64()
				iteminfo = TItemInfo()
				iteminfo.extend([itemUUID, itemID, itemCount, emptyIndex])
				self.missionIndex2Uids[emptyIndex] = itemUUID
				self._entity.missionItemList[itemUUID] = iteminfo
				result.append(itemUUID)
			#这里有个bug，当背包里没有此道具，而插入道具数量超过可叠加数量时，会一起放一个格子
		return result
			
	def removeMissionItem(self, itemUUID, itemCount):
		if not self.isMissionItem(itemID):
			return
		itemId = self._entity.missionItemList[itemUUID][1]
		itemC = self._entity.missionItemList[itemUUID][2]
		itemIndex = self._entity.missionItemList[itemUUID][3]
		if itemCount < itemC:
			self._entity.missionItemList[itemUUID][2] = itemC - itemCount
			return -1
		else:
			self.missionIndex2Uids[itemIndex] = 0
			del self._entity.missionItemList[itemUUID]
		return itemId
		
	def addWarehouseItem(self, itemID, itemCount):
		if self.isResource(itemID) or self.isMissionItem(itemID):
			return
		result = []
		emptyIndex = -1
		for i in range(0,self.maxWarehouseNum):
			if self.warehouseIndex2Uids[i] == 0:
				emptyIndex = i
				break
		#背包已经满了
		if emptyIndex == -1:
			return result
		#放置物品
		itemStack = d_items.datas[itemID]['maxnum']
		
		#不可堆叠物品
		if itemStack == 1:
			itemUUID = KBEngine.genUUID64()
			iteminfo = TItemInfo()
			iteminfo.extend([itemUUID, itemID, 1, emptyIndex])
			self.warehouseIndex2Uids[emptyIndex] = itemUUID
			self._entity.warehouseItemList[itemUUID] = iteminfo
			result.append(itemUUID)
			
		#可堆叠物品
		else:
			for key, info in self._entity.warehouseItemList.items():
				if info[1] == itemID and info[2] < itemStack:
					info[2] += itemCount
					result.append(key)
					if info[2] > itemStack:
						itemCount = info[2]-itemStack
						info[2] = itemStack
					else:
						itemCount = 0
						break

			if itemCount > 0:
				itemUUID = KBEngine.genUUID64()
				iteminfo = TItemInfo()
				iteminfo.extend([itemUUID, itemID, itemCount, emptyIndex])
				self.warehouseIndex2Uids[emptyIndex] = itemUUID
				self._entity.warehouseItemList[itemUUID] = iteminfo
				result.append(itemUUID)
			#这里有个bug，当背包里没有此道具，而插入道具数量超过可叠加数量时，会一起放一个格子
		return result
			
	def removeWarehouseItem(self, itemUUID, itemCount):
		if self.isResource(itemID) or self.isMissionItem(itemID):
			return
		itemId = self._entity.warehouseItemList[itemUUID][1]
		itemC = self._entity.warehouseItemList[itemUUID][2]
		itemIndex = self._entity.warehouseItemList[itemUUID][3]
		if itemCount < itemC:
			self._entity.warehouseItemList[itemUUID][2] = itemC - itemCount
			return -1
		else:
			self.warehouseIndex2Uids[itemIndex] = 0
			del self._entity.warehouseItemList[itemUUID]
		return itemId
	
	#装备或脱下
	def equipItem(self, itemIndex, equipIndex):
		invUid = self.invIndex2Uids[itemIndex]
		equipUid = self.equipIndex2Uids[equipIndex]
		#背包索引位置没有物品
		if invUid == 0 and equipUid == 0:
			return -1
		
		equipItem = {}
		if equipUid != 0:
			equipItem = self._entity.equipItemList[equipUid]
			del self._entity.equipItemList[equipUid]
			self.equipIndex2Uids[equipIndex] = 0

		if invUid != 0:
			self._entity.equipItemList[invUid] = self._entity.itemList[invUid]
			self._entity.equipItemList[invUid][3] = equipIndex
			self.equipIndex2Uids[equipIndex] = invUid
			del self._entity.itemList[invUid]
			self.invIndex2Uids[itemIndex] = 0

		if equipUid != 0:
			self._entity.itemList[equipUid] = equipItem
			self._entity.itemList[equipUid][3] = itemIndex
			self.invIndex2Uids[itemIndex] = equipUid
			
	#购买背包格子
	def buyBagNum(self, num):
		#todo：监测是否有足够的钻石
		if self.buyBagNum + num >= self.maxBagBuyNum:
			return GlobalConst.GC_BAG_BUY_MAX
		self.buyBagNum += num
		self.maxBagNum += num
		return GlobalConst.GC_OK
		
	#购买仓库格子
	def buyWarehouseNum(self, num):
		#todo：监测是否有足够的钻石
		if self.buyWarehouseNum + num >= self.maxWarehouseBuyNum:
			return GlobalConst.GC_WAREHOUSE_BUY_MAX
		self.buyWarehouseNum += num
		self.maxWarehouseNum += num
		return GlobalConst.GC_OK
		
	def getItemUidByIndex(self, itemIndex):
		return self.invIndex2Uids[itemIndex]
	def getEquipUidByIndex(self, equipIndex):
		return self.equipIndex2Uids[equipIndex]
	def getMissionItemUidByIndex(self, itemIndex):
		return self.missionIndex2Uids[itemIndex]
	def getWarehouseItemUidByIndex(self, itemIndex):
		return self.warehouseItemList[itemIndex]



		