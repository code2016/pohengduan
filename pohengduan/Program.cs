using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pohengduan
{
    class Program
    {
        static void Main(string[] args)
        {
            //第一步遍历当前文件夹下所有txt文件
            //第二步逐个打开，略过第一行，并读取文件名字转为桩号
            //第三步构造两个TXT，第一个用于百图，第二个用于hec
            string path = Directory.GetCurrentDirectory();//应用程序所在目录
            //string path = @"D:\微信\WeChat Files\li13526503670\FileStorage\File\2019-12\塘房河2";
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.txt");//获取所有txt文件
            files = files.Where(e => e.Name != "百图.txt" && e.Name != "hec.txt").ToArray();//首先剔除两个输出文件，如果存在的话
            //其次要对文件名排序
            //files = files.OrderBy(a=> Convert.ToInt32(a.Name.Substring(0, a.Name.Length - 4).Replace("+",""))).ToArray();
            files = files.OrderBy(a => Convert.ToInt32(a.Name.Substring(0, a.Name.Length - 4))).ToArray();

            string bpath=path + "\\百图.txt";
            string hpath=path + "\\hec.txt";
            FileStream baitu = new FileStream(bpath, FileMode.Create);
            FileStream hec = new FileStream(hpath, FileMode.Create);
            baitu.Close();
            hec.Close();

            int ind=0;
            bool titou = true;
            foreach(FileInfo fi in files)//对于每个文件
            {
                //先读取名字
                string  fname = fi.Name;                
                //再构造有效数据
                List<List<string>> strs = new List<List<string>>();
                using( StreamReader sr = new StreamReader(fi.FullName, Encoding.Default))
                {
                    string line;
                    int row = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (row > 0)//如果不是第一行就开始处理数据
                        {
                            string[] data = line.Split(new char[2] { ' ', '+' });//检查切割结果
                            data= data.Where(e=>!string.IsNullOrEmpty(e)).ToArray();
                            List<string> temp = new List<string>();
                            temp.Add(data[0]);
                            if(data[1].Contains("-"))
                            {
                                temp.Add((Convert.ToDouble(data[1]) * 1000 - Convert.ToDouble(data[2])).ToString());
                            }
                            else
                            {
                                temp.Add(Convert.ToDouble(data[data.Length - 1]).ToString());

                            }
                            strs.Add(temp);
                        }
                        row++;
                    }
                }
                
                //写入百图
                using(StreamWriter sr=new StreamWriter(bpath,true))
                {
                    int zhuang= Convert.ToInt32(fname.Substring(0, fname.Length - 4));
                    int ge = zhuang % 1000;
                    int qian = (zhuang - ge)/ 1000;
                    sr.WriteLine((qian+"+"+ge.ToString().PadLeft(3,'0')));
                    foreach(var p in strs)
                    {
                        sr.WriteLine(p[1] + "\t" + p[0]);
                    }
                }
                //写入hec
                using (StreamWriter sr = new StreamWriter(hpath,true))
                {
                    if(titou)
                    {
                        sr.WriteLine("river,reach,riverstation,station,elevation");
                        titou = false;
                    }
                    foreach(var p in strs)
                    {
                        sr.WriteLine("jriver,jreach,"+(files.Length-ind)+","+ p[1] + "," + p[0]);
                    }
                }
                ind++;

            }






        }
    }
}
