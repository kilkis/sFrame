using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class sBuildAssetbundle
{
    public static string sourcePath = Application.dataPath + "/sResources";
    public static string avatarPath = Application.dataPath + "/sAvatar";
    public static string shaderPath = Application.dataPath + "/sShader";
    public static string binPath = Application.dataPath + "/sBin";
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";
    const string AssetBundlesOutputPath1 = "Assets/sUpdateResource";

    [MenuItem("sFrame/懒人一键")]
    static void OneKey()
    {
        ClearAssetBundlesName();
        BuildAssetBundle();
        BuildLua();
        BuildAvatar();
        CopyBin();
        if (Directory.Exists(LuaFramework.Util.DataPath))
        {
            Directory.Delete(LuaFramework.Util.DataPath, true);
        }

        EditorUtility.DisplayDialog("打包完成", "打包完成啦！","ok");

    }
    //在Unity编辑器中添加菜单
    /// <summary>
	/// 清除所有的abname，以保证不会打包多余资源
	/// 但是，如果不是所有共享资源都在sourcePath下，则会达不到节省资源的作用
	/// </summary>
    [MenuItem("sFrame/第一步.ClearAssetBundlesName~清除所有资源名称")]
	static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;

        mapname.Clear();
        maps.Clear();
    }

    


    [MenuItem("sFrame/第二步.Build AssetBundle~随游戏资源准备")]
    static void BuildAssetBundle()
    {
        //if (EditorUtility.DisplayDialog("ABName确定", "确定所有需要依赖的材质（非打包目录下）都设定ABName为smat了吗？", "cancel", "ok"))
        {
        }
        //else
        {
            Debug.Log("sFrame ----- start packing....");
            PackShader(shaderPath);
            Pack(sourcePath);
            
            /*string outputPath = Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }*/

            //根据BuildSetting里面所激活的平台进行打包
            //使用LZ4格式压缩，兼顾加载速度和节约内存
            //BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();

            Debug.Log("sFrame ----- pack complete!");
        }
        
 
	}

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    [MenuItem("sFrame/第三步.Build Lua~打包Lua")]
    static void BuildLua()
    {
        if (LuaFramework.AppConst.LuaBundleMode)
        {
            HandleLuaBundle();
        }
        else
        {
            HandleLuaFile();
        }

    }
    static void HandleLuaBundle()
    {
        string streamDir = Application.dataPath + "/Lua/";
        if (!Directory.Exists(streamDir)) Directory.CreateDirectory(streamDir);

        string[] srcDirs = { CustomSettings.luaDir, CustomSettings.FrameworkPath + "/ToLua/Lua" };
        for (int i = 0; i < srcDirs.Length; i++)
        {
            if (LuaFramework.AppConst.LuaByteMode)
            {
                string sourceDir = srcDirs[i];
                string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                int len = sourceDir.Length;

                if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
                {
                    --len;
                }
                for (int j = 0; j < files.Length; j++)
                {
                    string str = files[j].Remove(0, len);
                    string dest = streamDir + str + ".bytes";
                    string dir = Path.GetDirectoryName(dest);
                    Directory.CreateDirectory(dir);
                    EncodeLuaFile(files[j], dest);
                }
            }
            else
            {
                ToLuaMenu.CopyLuaBytesFiles(srcDirs[i], streamDir);
            }
        }
        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.AllDirectories);
        for (int i = 0; i < dirs.Length; i++)
        {
            string name = dirs[i].Replace(streamDir, string.Empty);
            name = name.Replace('\\', '_').Replace('/', '_');
            name = "lua/lua_" + name.ToLower() + ".sab";
            string path = "Assets" + dirs[i].Replace(Application.dataPath, "");
            AddBuildMap(name, "*.bytes", path);
        }
        AddBuildMap("lua/lua.sab", "*.bytes", "Assets/" + LuaFramework.AppConst.LuaTempDir);

        //-------------------------------处理非Lua文件----------------------------------
        string luaPath = AppDataPath + "/StreamingAssets/lua/";
        for (int i = 0; i < srcDirs.Length; i++)
        {
            paths.Clear(); files.Clear();
            string luaDataPath = srcDirs[i].ToLower();
            Recursive(luaDataPath);
            foreach (string f in files)
            {
                if (f.EndsWith(".meta") || f.EndsWith(".lua")) continue;
                string newfile = f.Replace(luaDataPath, "");
                string path = Path.GetDirectoryName(luaPath + newfile);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                string destfile = path + "/" + Path.GetFileName(f);
                File.Copy(f, destfile, true);
            }
        }
        AssetDatabase.Refresh();
    }
    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();
    static List<string> mapname = new List<string>();
    static void AddBuildMap(string bundleName, string pattern, string path)
    {
        if (mapname.Contains(path))
            return;
        mapname.Add(path);
        string[] files = Directory.GetFiles(path, pattern);
        if (files.Length == 0) return;

        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace('\\', '/');
        }
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;
        build.assetNames = files;
        maps.Add(build);
    }
    /// <summary>
    /// 处理Lua文件
    /// </summary>
    static void HandleLuaFile()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        string luaPath = resPath + "/lua/";

        //----------复制Lua文件----------------
        if (!Directory.Exists(luaPath))
        {
            Directory.CreateDirectory(luaPath);
        }
        string[] luaPaths = { AppDataPath + "/LuaFramework/lua/",
                              AppDataPath + "/LuaFramework/Tolua/Lua/" };

        for (int i = 0; i < luaPaths.Length; i++)
        {
            paths.Clear(); files.Clear();
            string luaDataPath = luaPaths[i].ToLower();
            Recursive(luaDataPath);
            int n = 0;
            foreach (string f in files)
            {
                if (f.EndsWith(".meta")) continue;
                string newfile = f.Replace(luaDataPath, "");
                string newpath = luaPath + newfile;
                string path = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                if (File.Exists(newpath))
                {
                    File.Delete(newpath);
                }
                if (LuaFramework.AppConst.LuaByteMode)
                {
                    EncodeLuaFile(f, newpath);
                }
                else
                {
                    File.Copy(f, newpath, true);
                }
                //UpdateProgress(n++, files.Count, newpath);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }
    public static void EncodeLuaFile(string srcFile, string outFile)
    {
        if (!srcFile.ToLower().EndsWith(".lua"))
        {
            File.Copy(srcFile, outFile, true);
            return;
        }
        
        string luaexe = string.Empty;
        string args = string.Empty;
        string exedir = string.Empty;
        string currDir = Directory.GetCurrentDirectory();
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
        
            luaexe = "luajit.exe";
            args = "-b " + srcFile + " " + outFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
        
            luaexe = "./luac";
            args = "-o " + outFile + " " + srcFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luavm/";
        }
        Directory.SetCurrentDirectory(exedir);
        /*ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = luaexe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.ErrorDialog = true;
        info.UseShellExecute = isWin;
        Util.Log(info.FileName + " " + info.Arguments);

        Process pro = Process.Start(info);
        pro.WaitForExit();*/
        Directory.SetCurrentDirectory(currDir);
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    [MenuItem("sFrame/第四步.Build Avatar~打包Avatar和其他资源(请选择sAvatar目录)")]
    static void BuildAvatar()
    {
        AvatarPack(avatarPath);

        string outputPath = AssetBundlesOutputPath;// Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        //根据BuildSetting里面所激活的平台进行打包
        //使用LZ4格式压缩，兼顾加载速度和节约内存
        //BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        

        AssetDatabase.Refresh();
    }

    [MenuItem("sFrame/第五步.Copy Bin File")]
    static void CopyBin()
    {
        if (!Directory.Exists(AssetBundlesOutputPath+"/bin"))
        {
            Directory.CreateDirectory(AssetBundlesOutputPath + "/bin");
        }
        System.IO.DirectoryInfo dir = new DirectoryInfo(binPath);
        if (dir.Exists)
        {
            FileInfo[] fiList = dir.GetFiles();
            for (int i = 0; i < fiList.Length; ++i)
            {
                if (fiList[i].Name.EndsWith(".bin"))
                {
                    System.IO.File.Copy(binPath + "/" + fiList[i].Name, Application.dataPath + "/StreamingAssets/bin/" + fiList[i].Name, true);
                }
            }
        }
        
    }
    [MenuItem("sFrame/-----------分隔线-NavMesh--------------")]
    static void fgxs()
    { }

    [MenuItem("sFrame/打包NavMesh(需点选目录)")]
    static void BuildNavMesh()
    {
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            string pathname = AssetDatabase.GetAssetPath(o);
            
            if( pathname.EndsWith(".unity") )
            {
                string[] levels = { pathname };
                string[] ns = pathname.Split('/');
                pathname = ns[ns.Length - 1];
                ns = pathname.Split('.');
                Debug.Log("output:" + AssetBundlesOutputPath + "/" + ns[0] + ".sab");
                BuildPipeline.BuildPlayer(levels, AssetBundlesOutputPath + "/"+ns[ns.Length-2]+".sab", EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes);
            }
        }
        
        AssetDatabase.Refresh();
    }

    [MenuItem("sFrame/-----------分隔线-Shader--------------")]
    static void fgxss()
    { }

    [MenuItem("sFrame/打包Shader")]
    static void BuildShader()
    {
        PackShader(shaderPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("sFrame/-----------分隔线1--------------")]
    static void fgx()
    { }

    [MenuItem("sFrame/Build AssetBundle~更新资源打包")]
    static void BuildAssetBundle1()
    {
        //if (EditorUtility.DisplayDialog("ABName确定", "确定所有需要依赖的材质（非打包目录下）都设定ABName为smat了吗？", "cancel", "ok"))
        {
        }
        //else
        {
            Debug.Log("sFrame ----- start packing....");
            
            Pack(sourcePath);

            string outputPath = AssetBundlesOutputPath1;// Path.Combine(AssetBundlesOutputPath1, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            //根据BuildSetting里面所激活的平台进行打包
            //默认使用LZMA方式压缩,最大压缩程度
            BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();

            Debug.Log("sFrame ----- pack complete!");
        }


    }

    [MenuItem("sFrame/Zip Update~压缩更新文件")]
    static void ZipAssetBundel()
    {
        if (Directory.Exists(Application.dataPath+"/UpateZip/"))
        {

            Directory.Delete(Application.dataPath + "/UpateZip/", true);
            Directory.CreateDirectory(Application.dataPath + "/UpateZip/");
        }
        else
        {
            Directory.CreateDirectory(Application.dataPath + "/UpateZip/");
        }
        ZipHelper.StartZipDir(Application.dataPath + "/sUpdateResource/" + Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget)+"/", Application.dataPath + "/UpateZip" + "/"+sConst.ZipName, 9);
        EditorUtility.ClearProgressBar();
        
        //刷新编辑器  
        AssetDatabase.Refresh();
        if (EditorUtility.DisplayDialog("压缩", Application.streamingAssetsPath, "完成"))
        {
        }
    }


    public static string AssetbundlePath
    {
        get { return "assetbundles" + Path.DirectorySeparatorChar; }
    }

    static void AvatarPack(string source)
    {
        List<AnimationClip> _anim = new List<AnimationClip>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (AssetDatabase.GetAssetPath(o).Contains("/Animation/"))
            {
                AnimationClip anim = (AnimationClip)o;
                _anim.Add(anim);
            }
        }
        List<Object> toinclude = new List<Object>();
        List<GameObject> delt = new List<GameObject>();
        List<GameObject> characterClones = new List<GameObject>();
        List<Object> cbp = new List<Object>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (o.name.Contains("@"))
                continue;
            if (!(o is GameObject)) continue;
            if (AssetDatabase.GetAssetPath(o).Contains("/Materials/"))
                continue;
            if (AssetDatabase.GetAssetPath(o).Contains("/Animation/"))
                continue;
            
            Debug.Log(o.name);
            if( o.name.Contains("_bone"))
            {
                GameObject characterFBX = (GameObject)o;
                string name = characterFBX.name.ToLower();
                GameObject characterClone = (GameObject)Object.Instantiate(characterFBX);
                characterClones.Add(characterClone);
                //删除多余组件
                Animator tmp = characterClone.GetComponent<Animator>();
                Object.DestroyImmediate(tmp);
                //加入动作组件，这个要看情况，如果动作绑定在模型上，则不需要，否则需要读取进来
                Animation aa = characterClone.GetComponent<Animation>();
                if( aa == null )
                {
                    aa = characterClone.AddComponent<Animation>();
                    for (int i = 0; i < _anim.Count; ++i)
                    {
                        aa.AddClip(_anim[i], _anim[i].name);
                    }
                    aa.clip = aa.GetClip("breath");
                }
                
                foreach (SkinnedMeshRenderer smr in characterClone.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Debug.Log("smr:" + smr.name);
                    Object.DestroyImmediate(smr.gameObject);
                }
                SkinnedMeshRenderer smr1 = characterClone.AddComponent<SkinnedMeshRenderer>();
                smr1.useLightProbes = false;
                smr1.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                Debug.Log("characterClone.name:" + characterClone.name);
                Object characterBasePrefab = GetPrefab(characterClone, o.name);
                cbp.Add(characterBasePrefab);
                //AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/" + o.name + ".prefab");
                //assetImporter.assetBundleName = o.name + ".sab";
                AddBuildMap(o.name + ".sab", "*.bytes", "Assets/" + o.name + ".prefab");

                
            }
            else
            {
                GameObject characterFBX = (GameObject)o;
                string name = characterFBX.name.ToLower();
                GameObject characterClone = (GameObject)Object.Instantiate(characterFBX);
                Animator tmp = characterClone.GetComponent<Animator>();
                Object.DestroyImmediate(tmp);
                characterClones.Add(characterClone);
                SkinnedMeshRenderer smr1 = characterClone.GetComponentInChildren<SkinnedMeshRenderer>();
                smr1.useLightProbes = false;
                smr1.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                Object rendererPrefab = GetPrefab(characterClone, o.name);
                //AssetImporter assetImporter1 = AssetImporter.GetAtPath("Assets/" + o.name + ".prefab");
                //assetImporter1.assetBundleName = o.name + ".sab";
                AddBuildMap(o.name + ".sab", "*.bytes", "Assets/" + o.name + ".prefab");
                cbp.Add(rendererPrefab);

                
                string[] depens = AssetDatabase.GetDependencies("Assets/" + o.name + ".prefab");
                for (int i = 0; i < depens.Length; ++i)
                {
                    //Debug.Log("all depend:" + depens[i]);
                    if (!depens[i].ToLower().EndsWith(".mat"))
                        continue;
                    Debug.Log("depend:" + depens[i]);
                    file2(depens[i]);
                }
                
            }
            
            
           
        }
        //最终打包
        string outputPath = AssetBundlesOutputPath;// Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
                                          BuildAssetBundleOptions.ChunkBasedCompression;
        BuildPipeline.BuildAssetBundles(outputPath, maps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //删除角色相关go
        for(int i = 0;i < characterClones.Count; ++i )
        {
            Object.DestroyImmediate(characterClones[i]);
        }
        
        for( int i = 0; i < cbp.Count; ++i )
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cbp[i]));
        }
        
        for (int i = 0; i < delt.Count; ++i)
        {

        }
        for (int i = 0; i < toinclude.Count; ++i)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(toinclude[i]));
        }
        BuildFileIndex();
        string streamDir = Application.dataPath + "/" + LuaFramework.AppConst.LuaTempDir;
        if (Directory.Exists(streamDir)) Directory.Delete(streamDir, true);
        AssetDatabase.Refresh();
    }

    static void BuildFileIndex()
    {
        string resPath = AppDataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = LuaFramework.Util.md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close(); fs.Close();
    }

    static Object GetPrefab(GameObject go, string name)
    {
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
        tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
        //Object.DestroyImmediate(go);
        return tempPrefab;
    }
    static void Pack(string source)
	{
        //Debug.Log("pack:" + source);
		DirectoryInfo folder = new DirectoryInfo (source);
		FileSystemInfo[] files = folder.GetFileSystemInfos ();
		int length = files.Length;
		for (int i = 0; i < length; i++) {
			if(files[i] is DirectoryInfo)
			{
				Pack(files[i].FullName);
			}
			else
			{
				if(!files[i].Name.EndsWith(".meta"))
				{
					file (files[i].FullName);
				}
			}
		}
	}

    static void PackShader(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                PackShader(files[i].FullName);
            }
            else
            {
                if (files[i].Name.EndsWith(".shader"))
                {
                    fileShader(files[i].FullName);
                }
            }
        }
    }

    static void fileShader(string source)
    {
        string _source = Replace(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
        //Debug.Log (_assetPath);
        //Debug.Log(_assetPath2);

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        if( source.EndsWith("shader"))
            assetName = assetName.Replace(Path.GetExtension(assetName), ".s");
        else
            assetName = assetName.Replace(Path.GetExtension(assetName), ".sab");
        //Debug.Log ("######################## "+assetName);
        //assetImporter.assetBundleName = assetName;

        AddBuildMap(assetName, "*.bytes", _assetPath);
    }
 
	static void file(string source)
	{
        //Debug.Log("source:" + source);
		string _source = Replace (source);
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);
		string _assetPath2 = _source.Substring (Application.dataPath.Length + 1);
		//Debug.Log (_assetPath);
        //Debug.Log(_assetPath2);
 
		//在代码中给资源设置AssetBundleName
		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
		string assetName = _assetPath2.Substring (_assetPath2.IndexOf("/") + 1);
		assetName = assetName.Replace(Path.GetExtension(assetName),".sab");
        //Debug.Log ("######################## "+assetName);
        //assetImporter.assetBundleName = assetName;

        AddBuildMap(assetName, "*.bytes", _assetPath);

        string[] depens = AssetDatabase.GetDependencies(_assetPath);
        for( int i = 0; i < depens.Length; ++i )
        {
            //Debug.Log("all depend:" + depens[i]);
            if (!depens[i].ToLower().EndsWith(".mat") && !_assetPath.Contains("ugui") )
                continue;
            if (depens[i].ToLower().EndsWith(".prefab"))
                continue;
            Debug.Log("depend:" + depens[i]);
            file2(depens[i]);
        }
	}

    static void file2(string source)
    {
        string _assetPath2 = source.Substring (Application.dataPath.Length + 1);
        AssetImporter assetImporter = AssetImporter.GetAtPath(source);
		//string assetName = _assetPath2.Substring (_assetPath2.IndexOf("/") + 1);
		//assetName = assetName.Replace(Path.GetExtension(assetName),".sab");
        //Debug.Log (assetName);
        string[] tmp = source.Split('.');
        if( tmp[1] == "mat")
            AddBuildMap(AssetDatabase.AssetPathToGUID(source) + ".m", "*.bytes", source);
        //assetImporter.assetBundleName = AssetDatabase.AssetPathToGUID(source) + ".m";
        else
            AddBuildMap(AssetDatabase.AssetPathToGUID(source) + ".sab", "", source);
        //  assetImporter.assetBundleName = AssetDatabase.AssetPathToGUID(source) + "." + tmp[1];

        string[] depens = AssetDatabase.GetDependencies(source);
        for( int i = 0; i < depens.Length; ++i )
        {
            
            if (depens[i].EndsWith(".mat"))
                continue;
            Debug.Log("depend1:" + depens[i]);
            file3(depens[i]);
        }
    }

    static void file3(string source)
    {
        string _assetPath2 = source.Substring(Application.dataPath.Length + 1);
        AssetImporter assetImporter = AssetImporter.GetAtPath(source);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
        assetName = assetName.Replace(Path.GetExtension(assetName), ".sab");
        //Debug.Log (assetName);
        string[] tmp = source.Split('.');
        if (tmp[1] == "shader")
            AddBuildMap(AssetDatabase.AssetPathToGUID(source) + ".s", "*.bytes", source);
        //assetImporter.assetBundleName = AssetDatabase.AssetPathToGUID(source) + ".s";
        else
            AddBuildMap(AssetDatabase.AssetPathToGUID(source) + ".t", "*.bytes", source);
        //  assetImporter.assetBundleName = AssetDatabase.AssetPathToGUID(source) + ".t";


    }
 
	static string Replace(string s)
	{
		return s.Replace("\\","/");
	}
    [MenuItem("sFrame/-----------分隔线2--------------")]
    static void fgx2()
    { }
    [MenuItem("sFrame/!~Clear All AssetBundle")]
	static void clearAssetBundle()
	{
		string outPath=Application.streamingAssetsPath;
		if (!Directory.Exists (outPath)) 
		{
			return;
		}

		deleteFileOrFolder (outPath);
		AssetDatabase.Refresh ();
	}

    [MenuItem("sFrame/!~Clear All Update AssetBundle")]
    static void clearAssetBundle1()
    {
        string outPath = Application.dataPath + "/sUpdateResource";
        if (!Directory.Exists(outPath))
        {
            return;
        }

        deleteFileOrFolder(outPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("sFrame/RefreshMat")]
    static void RefreshMat()
    {
        var guids = AssetDatabase.FindAssets("t:Material");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.ToLower().EndsWith("mat"))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat && mat.shader)
                {
                    Debugger.Log("{0}\n{1}\n{2}\n{3}\n", path, mat.shader.name,
                        mat.shader.GetInstanceID(),
                        Shader.Find(mat.shader.name).GetInstanceID());
                    mat.shader = Shader.Find(mat.shader.name);
                }
            }
        }
    }

    static void deleteFileOrFolder(string fileOrFolder)
    {
        if (Directory.Exists(fileOrFolder))
        {
            string[] allFiles = Directory.GetFiles(fileOrFolder);
            if (allFiles != null)
            {
                for (int i = 0; i < allFiles.Length; i++)
                {
                    deleteFileOrFolder(allFiles[i]);
                }
            }

            string[] allFolders = Directory.GetDirectories(fileOrFolder);
            if (allFolders != null)
            {
                for (int i = 0; i < allFolders.Length; i++)
                {
                    deleteFileOrFolder(allFolders[i]);
                }
            }

            Directory.Delete(fileOrFolder);
        }
        else
        {
            if (File.Exists(fileOrFolder))
            {
                File.Delete(fileOrFolder);
            }
        }
    }

    
}
 
public class Platform 
{
	public static string GetPlatformFolder(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "IOS";
		case BuildTarget.WebPlayer:
			return "WebPlayer";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
		default:
			return null;
		}
	}
}
   
