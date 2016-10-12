一个可以快速扩展出大多数类型游戏的基于unity5.3.5的框架：sFrame
===========================================================================
made by ShawnSu
---------------------------------------------------------------------------
todo:
1.playermanager对角色的管理，范围，最大显示数量，开关
2.camera遮挡换shader透视


功能：
-1.优化
	文件：sFrame/Util/sStringBuilder.cs
	功能：所有的string整合建议直接使用此方法

	文件：sFrame/Util/sCheckMemory.cs
	功能：用于所有相关的内存数据获取和检测

0.Core
	文件：sFrame/Prefab/sGameRoot.prefab	
	介绍：这个prefab需要拖到第一场景！用于维护整个框架所有的singleton

1.导出
	文件：sFrame/Editor/sBuildAssetbundle.cs
	介绍：分两种导出（压缩格式不同，分别适用于随包资源和下载资源两种情况）
	      并且提供用于下载资源的压缩机制
	
	打包步骤：1.第一步，第二步，第三步，第三步点之前需要选定sAvatar目录，不用avatar系统可无视

2.游戏版本
	游戏版本默认从1.0.0开始

	文件：sFrame/Scripts/Model/sFirstGame.cs
	介绍：第一次安装后会由gameRoot主导自动运行，进行版本文件初始化

	文件：sFrame/Scripts/ViewModel/sUpdateGame.cs
	介绍：下载cdn上的版本文件，进行比较，反射到View，通过View的选择进行下载更新,下载完后会自动进行解压覆盖，然后继续下载直到最新

3.Cache
	文件：sFrame/Scripts/Model/sCache.cs
	介绍：用于cache游戏内加载的gameobject，也允许不cache加载

	文件：sFrame/Scripts/ViewModel/sLoadingGame.cs
	文件：文件加载类，加载文件的接口都从这里走，cache行为会在内部自动处理

4.Avatar
	
Q & A:
1.服务器始终在下发坐标，客户端始终在上传坐标
	客户端没有和服务器坐标同步，距离触发了作弊保护，导致服务器一直在重置坐标
