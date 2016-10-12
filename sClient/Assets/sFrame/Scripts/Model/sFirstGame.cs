using UnityEngine;
using System.Collections;
using System.IO;

public class sFirstGame : sSingleton<sFirstGame>
{

	public override void Init()
    {
        string versionFile = sStringBuilder.combine(Application.persistentDataPath, "/version.txt");
        if( checkFirst(versionFile))
        {
            _markFirst(versionFile);
        }
        else
        {
            _readVersion(versionFile);
        }

        Debug.Log("version:" + sConst.version_1 + " - " + sConst.version_2 + " - " + sConst.version_3);
    }

    /// <summary>
    /// 确认是否第一次安装
    /// </summary>
    /// <returns></returns>
    public bool checkFirst(string filename)
    {
        if (!File.Exists(filename))
            return true;
        return false;
    }
    /// <summary>
    /// 标记第一次安装
    /// </summary>
    private void _markFirst(string filename)
    {
        FileStream fs = File.Open(filename, FileMode.Create);
        StreamWriter bw = new StreamWriter(fs);
        bw.Write("1.0.0");
        bw.Flush();
        bw.Close();
        fs.Close();
        sConst.version_1 = 1;
        sConst.version_2 = 0;
        sConst.version_3 = 0;

        //建立文件更新所需要的目录
        string dirname = sStringBuilder.combine(Application.persistentDataPath, "/tmpzip/");
        if( !Directory.Exists(dirname))
        {
            Directory.CreateDirectory(dirname);
        }
    }

    /// <summary>
    /// 读取版本号
    /// </summary>
    /// <param name="filename"></param>
    private void _readVersion(string filename)
    {
        FileStream fs = File.Open(filename, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        string ver = sr.ReadLine();
        sr.Close();
        fs.Close();

        string[] vers = ver.Split('.');
        if (vers.Length != 3)
        {
            sConst.version_1 = 1;
            sConst.version_2 = 0;
            sConst.version_3 = 0;
        }
        else
        {
            sConst.version_1 = int.Parse(vers[0]);
            sConst.version_2 = int.Parse(vers[1]);
            sConst.version_3 = int.Parse(vers[2]);
        }
    }
}
