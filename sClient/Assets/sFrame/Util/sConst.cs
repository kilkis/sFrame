using UnityEngine;
using System.Collections;

/// <summary>
/// 常量
/// </summary>
public class sConst
{
    #region Update
    //更新包的压缩文件名称
    public static string ZipName = "update.zip";
#if UNITY_IOS
    public static string updateURL = "http://1251007321.cdn.myqcloud.com/1251007321/sFrame/ios/";
#elif UNITY_ANDROID
    public static string updateURL = "http://1251007321.cdn.myqcloud.com/1251007321/sFrame/android/";
#else
    public static string updateURL = "http://1251007321.cdn.myqcloud.com/1251007321/sFrame/other/";
#endif
    #endregion

    #region Version
    /// <summary>
    /// 版本规则：
    /// 大版本不一致，提示去平台下载，而不是更新包
    /// 中版本不一致，直接跳过小版本更新包
    /// 小版本不一致，以1为差值多次更新包
    /// </summary>
    public static int version_1 = 0;//大版本
    public static int version_2 = 0;//中版本
    public static int version_3 = 0;//小版本
    #endregion

    #region Avatar
    public static string combineMaterial = "Out/Mobile/Diffuse";
    public static int combineTextureMax = 512;
    public static string combineDiffuseTexture = "_MainTex";
    #endregion

    #region Game
    //游戏逻辑时间
    public static float logictInvTime = 0.1f;
    //cemera检测遮挡物频率
    public static float cameraCheckInv = 1.0f;
    public static string terrainLayer = "terrain";
    public static string cameraColLayer = "scenecol";
    #endregion
}
