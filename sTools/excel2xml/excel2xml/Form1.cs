using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Xml;
using System.IO;

namespace excel2xml
{
    public partial class Form1 : Form
    {
        DataSet excelTableDataSet = new DataSet();
        string excelname;
        string purename;
        public Form1()
        {
            InitializeComponent();
        }

        bool isClient(string name)
        {
            string tmp = name.Substring(0, 3);
            if (tmp != "(S)")
                return true;
            return false;
        }

        bool isServer(string name)
        {
            string tmp = name.Substring(0, 3);
            if (tmp != "(C)")
                return true;
            return false;
        }

        //open excel
        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "xlsx文件(*.xlsx)|*.xlsx|xls文件(*.xls)|*.xls";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.openFileDialog1.FileName;
                //提取纯粹的文件名
                if (FileName.EndsWith(".xls"))
                    purename = FileName.Replace(".xls", "");
                else if( FileName.EndsWith(".xlsx"))
                    purename = FileName.Replace(".xlsx", "");

                excelname = FileName;
                label1.Text = "[读取中]"+FileName;


                dataGridView1.DataSource = ToDataTable(ref excelTableDataSet, FileName);
                dataGridView1.DataMember = "[{0}]";

                for (int count = 0; (count <= (dataGridView1.Rows.Count - 1)); count++)
                {

                    dataGridView1.Rows[count].HeaderCell.Value = (count + 1).ToString();
                };

                loadConfig();

                label1.Text = "[读取完成]" + FileName;
            }
        }
        //export
        private void button2_Click(object sender, EventArgs e)
        {
            if( !label1.Text.Contains("读取完成"))
            {
                MessageBox.Show("请先读取文件");
                return;
            }
            if( !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked)
            {
                MessageBox.Show("请先选择导出类型");
                return;
            }
            button2.Enabled = false;
            if (checkBox2.Checked)
                exportBin();
            if (checkBox3.Checked)
                exportPython();
            if (checkBox4.Checked)
                exportCSharp();
            saveConfig();
            MessageBox.Show("导出完成");
            button2.Enabled = true;
        }

        private void exportBin()
        {
            if( File.Exists(purename+".bin"))
            {
                File.Delete(purename + ".bin");
            }
            FileStream fs = new FileStream(purename+".bin", FileMode.CreateNew);
            BinaryWriter sw = new BinaryWriter(fs);
            //foreach (DataTable dt in excelTableDataSet.Tables)   //遍历所有的datatable
            {
                DataTable dt = excelTableDataSet.Tables[0];//只用第一个表
                //写入数量
                sw.Write(dt.Rows.Count - 1);
                for (int i = 1; i < dt.Rows.Count; ++i )//遍历所有的行
                {
                    DataRow dr = dt.Rows[i];
                    
                    foreach (DataColumn dc in dt.Columns)   //遍历所有的列
                    {
                        if (!isClient(dc.ColumnName))
                            continue;
                        //Console.WriteLine("{0},   {1},   {2}", dt.TableName, dc.ColumnName, dr[dc]);   //表名,列名,单元格数据
                        if (dc.ColumnName.EndsWith("(I)"))
                        {
                            sw.Write(int.Parse(dr[dc].ToString()));
                        }
                        else if (dc.ColumnName.EndsWith("(V)"))
                        {
                            string tmp = dr[dc].ToString();
                            string[] tmps = tmp.Split(',');
                            for (int j = 0; j < tmps.Length; ++j)
                            {
                                Console.WriteLine("x:" + tmps[j]);
                                sw.Write(float.Parse(tmps[i]));
                            }
                        }
                        else if (dc.ColumnName.EndsWith("(S)"))
                        {
                            sw.Write(dr[dc].ToString());
                        }

                    }
                }
                        
            }
            sw.Close();
            fs.Close();
        }

        private void exportPython()
        {
            DataTable dt = excelTableDataSet.Tables[0];//只用第一个表
            string[] tmps = purename.Split('\\');
            string fn = tmps[tmps.Length - 1];
            string csname = purename.Replace(fn, "d_" + fn);
            if (File.Exists(csname + ".py"))
            {
                File.Delete(csname + ".py");
            }

            FileStream fs = new FileStream(csname + ".py", FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("datas={");
            for (int i = 1; i < dt.Rows.Count; ++i )//遍历所有的行
            {
                DataRow dr = dt.Rows[i];

                string test = dt.Columns[0].ColumnName.ToString();
                if (test.EndsWith("(I)"))
                    sw.WriteLine("\t" + dr[dt.Columns[0]].ToString() + ":{");
                else if (test.EndsWith("(S)"))
                    sw.WriteLine("\t\"" + dr[dt.Columns[0]].ToString() + "\":{");
                else
                {
                    MessageBox.Show("导出python的索引不是I或者S");
                    return;
                }
                foreach (DataColumn dc in dt.Columns)   //遍历所有的列
                {
                    if (!isServer(dc.ColumnName))
                        continue;
                    if (dc.ColumnName.EndsWith("(V)"))
                    {
                        sw.WriteLine("\t\t\"" + getPName(dc.ColumnName) + "\":(" + dr[dc].ToString() + "),");
                    }
                    else if( dc.ColumnName.EndsWith("(S)"))
                    {
                        sw.WriteLine("\t\t\"" + getPName(dc.ColumnName) + "\":\"" + dr[dc].ToString() + "\",");
                    }
                    else
                    {
                        sw.WriteLine("\t\t\"" + getPName(dc.ColumnName) + "\":" + dr[dc].ToString() + ",");
                    }
                }
                sw.WriteLine("\t\t},");
            }
            sw.WriteLine("}");
            sw.Close();
            fs.Close();
        }

        private string getReadFunc(string name)
        {
            if (name.EndsWith("(I)"))
                return "ReadInt32()";
            else if (name.EndsWith("(V)"))
                return "ReadSingle()";
            else if (name.EndsWith("(S)"))
                return "ReadString()";
            return "Read()";
        }
        private string getType(string name)
        {
            if (name.EndsWith("(I)"))
                return "int";
            else if (name.EndsWith("(V)"))
                return "Vector3";
            else if (name.EndsWith("(S)"))
                return "string";
            return "object";
        }

        private string getPName(string name)
        {
            string tmp = name.Substring(0, 3);
            if (tmp == "(C)" || tmp == "(S)")
                name = name.Substring(3, name.Length - 3);
            if (name.EndsWith("(I)"))
                return name.Replace("(I)", "");
            else if (name.EndsWith("(V)"))
                return name.Replace("(V)", "");
            else if (name.EndsWith("(S)"))
                return name.Replace("(S)", "");
            return name;
        }
        private void exportCSharp()
        {
            DataTable dt = excelTableDataSet.Tables[0];//只用第一个表

            //以第一位的类型为索引，做dictionary
            string[] tmps = purename.Split('\\');
            string fn = tmps[tmps.Length - 1];
            string csname = purename.Replace(fn, "sLoadBin_" + fn);
            if (File.Exists(csname + ".cs"))
            {
                File.Delete(csname + ".cs");
            }
            FileStream fs = new FileStream(csname + ".cs", FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine("using System.Collections;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.IO;");
            sw.WriteLine("");
            sw.WriteLine("namespace sFrame.LoadBin");
            sw.WriteLine("{");
            string dataname = "data_" + fn;
            sw.WriteLine("\tpublic class data_"+fn);
            sw.WriteLine("\t{");
            for (int i = 0; i < dt.Columns.Count; ++i)
            {
                DataColumn dc = dt.Columns[i];
                if (!isClient(dc.ColumnName))
                    continue;
                sw.WriteLine("\t\tpublic " + getType(dc.ColumnName) + " " + getPName(dc.ColumnName) + ";");
            }
            sw.WriteLine("\t}");
            sw.WriteLine("");
            sw.WriteLine("\tpublic class sLoadBin_" + fn);
            sw.WriteLine("\t{");
            if( comboBox1.SelectedIndex == 0 )
                sw.WriteLine("\t\tpublic List<data_" + fn + "> data = new List<data_" + fn + ">();");
            else
                sw.WriteLine("\t\tpublic Dictionary<" + getType(dt.Columns[0].ColumnName) + " ,data_" + fn + "> data = new Dictionary<" + getType(dt.Columns[0].ColumnName) + " ,data_" + fn + ">();");
            sw.WriteLine("");
            sw.WriteLine("\t\tpublic void load(string name)");
            sw.WriteLine("\t\t{");
            sw.WriteLine("\t\t\tFileStream fs = new FileStream(name, FileMode.Open);");
            sw.WriteLine("\t\t\tBinaryReader br = new BinaryReader(fs);");
            sw.WriteLine("\t\t\tint num = br.ReadInt32();");
            sw.WriteLine("\t\t\tfor (int i = 0; i < num; ++i)");
            sw.WriteLine("\t\t\t{");
            sw.WriteLine("\t\t\t\t" + dataname + " tmp = new " + dataname + "();");
            for (int i = 0; i < dt.Columns.Count; ++i )
            {
                DataColumn dc = dt.Columns[i];
                if (!isClient(dc.ColumnName))
                    continue;
                if( getType(dc.ColumnName) == "Vector3")
                {
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t\tfloat x = br.ReadSingle();");
                    sw.WriteLine("\t\t\t\t\tfloat y = br.ReadSingle();");
                    sw.WriteLine("\t\t\t\t\tfloat z = br.ReadSingle();");
                    sw.WriteLine("\t\t\t\t\ttmp." + getPName(dc.ColumnName) + " = new Vector3(x, y, z);");
                    sw.WriteLine("\t\t\t\t}");
                }
                else
                    sw.WriteLine("\t\t\t\ttmp." + getPName(dc.ColumnName) + " = br." + getReadFunc(dc.ColumnName) + ";");
            }
            if (comboBox1.SelectedIndex == 0)
                sw.WriteLine("\t\t\t\tdata.Add(tmp);");
            else
            {
                sw.WriteLine("\t\t\t\tdata.Add(tmp." + getPName(dt.Columns[0].ColumnName) + " ,tmp);");
            }
            sw.WriteLine("\t\t\t}");
            sw.WriteLine("\t\t\tbr.Close();");
            sw.WriteLine("\t\t\tfs.Close();");
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Close();
            fs.Close();
        }

       
        public static DataSet ToDataTable(ref DataSet excelTableDataSet, string filePath)
        {
            excelTableDataSet = new DataSet();
            string connStr = "";
            string fileType = System.IO.Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(fileType)) return null;

            if (fileType == ".xls")
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            string sql_F = "Select * FROM [{0}]";

            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            DataTable dtSheetName = null;

            try
            {
                // 初始化连接，并打开
                conn = new OleDbConnection(connStr);
                conn.Open();

                // 获取数据源的表定义元数据                        
                string SheetName = "";
                dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                // 初始化适配器
                da = new OleDbDataAdapter();
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    SheetName = (string)dtSheetName.Rows[i]["TABLE_NAME"];

                    if (SheetName.Contains("$") && !SheetName.Replace("'", "").EndsWith("$"))
                    {
                        continue;
                    }

                    da.SelectCommand = new OleDbCommand(String.Format(sql_F, SheetName), conn);
                    DataSet dsItem = new DataSet();
                    da.Fill(dsItem, "[{0}]");

                    excelTableDataSet.Tables.Add(dsItem.Tables[0].Copy());
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // 关闭连接
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    da.Dispose();
                    conn.Dispose();
                }
            }
            return excelTableDataSet;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if( checkBox1.Checked )
            {
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //默认设为第一个选项
            comboBox1.SelectedIndex = 0;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if( checkBox4.Checked )
            {
                groupBox4.Enabled = true;
            }
            else
            {
                groupBox4.Enabled = false;
            }
        }

        //默认保存设置,用于打开原有文件，可以不用重复设定
        private void saveConfig()
        {
            checkConfigDictionary();
            string[] tmps = purename.Split('\\');
            string fn = tmps[tmps.Length - 1];
            string cname = purename.Replace(fn, "config\\" + fn);

            if (File.Exists(cname + ".config"))
            {
                File.Delete(cname + ".config");
            }
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            XmlElement root = doc.CreateElement("config");
            doc.AppendChild(root);

            XmlElement fe = doc.CreateElement("isCombine");
            fe.InnerText = checkBox1.Checked.ToString();
            root.AppendChild(fe);
            XmlElement fe1 = doc.CreateElement("combineS");
            fe1.InnerText = textBox1.Text;
            root.AppendChild(fe1);
            XmlElement fe2 = doc.CreateElement("combineE");
            fe2.InnerText = textBox2.Text;
            root.AppendChild(fe2);


            XmlElement be = doc.CreateElement("isBin");
            be.InnerText = checkBox2.Checked.ToString();
            root.AppendChild(be);
            XmlElement pe = doc.CreateElement("isPython");
            pe.InnerText = checkBox3.Checked.ToString();
            root.AppendChild(pe);
            XmlElement ce = doc.CreateElement("isCSharp");
            ce.InnerText = checkBox4.Checked.ToString();
            root.AppendChild(ce);
            XmlElement ce1 = doc.CreateElement("isSecret");
            ce1.InnerText = checkBox5.Checked.ToString();
            root.AppendChild(ce1);
            XmlElement ce2 = doc.CreateElement("CSharpType");
            ce2.InnerText = comboBox1.SelectedIndex.ToString();
            root.AppendChild(ce2);

            doc.Save(cname + ".config");
        }

        private void loadConfig()
        {
            string[] tmps = purename.Split('\\');
            string fn = tmps[tmps.Length - 1];
            string cname = purename.Replace(fn, "config\\" + fn);

            if (File.Exists(cname + ".config"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(cname + ".config");
                //XmlNode node = doc.GetElementsByTagName("root");
                XmlNodeList nodes = doc.GetElementsByTagName("isCombine");
                checkBox1.Checked = bool.Parse(nodes[0].InnerText);
                nodes = doc.GetElementsByTagName("combineS");
                textBox1.Text = nodes[0].InnerText;
                nodes = doc.GetElementsByTagName("combineE");
                textBox2.Text = nodes[0].InnerText;

                nodes = doc.GetElementsByTagName("isBin");
                checkBox2.Checked = bool.Parse(nodes[0].InnerText);
                nodes = doc.GetElementsByTagName("isPython");
                checkBox3.Checked = bool.Parse(nodes[0].InnerText);
                nodes = doc.GetElementsByTagName("isCSharp");
                checkBox4.Checked = bool.Parse(nodes[0].InnerText);
                nodes = doc.GetElementsByTagName("isSecret");
                checkBox5.Checked = bool.Parse(nodes[0].InnerText);
                nodes = doc.GetElementsByTagName("CSharpType");
                comboBox1.SelectedIndex = int.Parse(nodes[0].InnerText);

            }
            else
            {
                checkBox1.Checked = false;
                textBox1.Text = "[";
                textBox2.Text = "]";

                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                comboBox1.SelectedIndex = 0;
            }
        }

        private void checkConfigDictionary()
        {
            string[] tmps = purename.Split('\\');
            string fn = tmps[tmps.Length - 1];
            string dir = purename.Replace(fn, "config\\");
            if( !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
