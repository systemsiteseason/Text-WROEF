using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Text_WROEF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog opf1 = new OpenFileDialog();
        OpenFileDialog opf2 = new OpenFileDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            opf1.Filter = "UMAP|*.umap";
            if (opf1.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf1.FileName))
                {
                    if (!File.Exists(opf1.FileName + "_bk"))
                    {
                        File.Copy(opf1.FileName, opf1.FileName + "_bk");
                    }
                    FileStream fs = new FileStream(opf1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryReader rd = new BinaryReader(fs);
                    string dir = Path.GetDirectoryName(opf1.FileName);
                    StreamWriter wt = new StreamWriter(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + ".FileSize");
                    rd.ReadBytes(20);
                    int hash = rd.ReadInt32();
                    if (hash == 0)
                        rd.ReadBytes(33);
                    else
                        rd.ReadBytes(53);
                    int math = rd.ReadInt32();
                    rd.BaseStream.Seek(rd.ReadInt32(), SeekOrigin.Begin);
                    for (int i = 0; i < math; i++)
                    {
                        rd.ReadBytes(24);
                        int len = rd.ReadInt32();
                        int offset = rd.ReadInt32();
                        rd.ReadBytes(36);
                        wt.Write(offset.ToString() + " " + len.ToString() + "\n");
                    }
                    wt.Close();
                    Directory.CreateDirectory(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + "FileEx");
                    Directory.CreateDirectory(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + "FileTxT");
                    foreach (string line in File.ReadAllLines(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + ".FileSize"))
                    {
                        var data = line.Split(' ');
                        rd.BaseStream.Seek(long.Parse(data[0]), SeekOrigin.Begin);
                        byte[] buffer = rd.ReadBytes(int.Parse(data[1]));
                        MemoryStream ms = new MemoryStream(buffer);
                        BinaryReader rdms = new BinaryReader(ms);
                        rdms.ReadBytes(36);
                        try
                        {
                            float ckd = rdms.ReadSingle();
                            if (ckd >= 2.5f && ckd <= 2.6f)
                            {
                                BinaryWriter bwt = new BinaryWriter(new FileStream(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + "FileEx" + "\\" + data[0] + ".bin", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                                StreamWriter swt = new StreamWriter(new FileStream(dir + "\\" + Path.GetFileNameWithoutExtension(opf1.FileName) + "FileTxT" + "\\" + data[0] + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                                rdms.BaseStream.Seek(32, SeekOrigin.Begin);
                                int num = rdms.ReadInt32();
                                byte[] pivot = rdms.ReadBytes(num * 12);
                                BinaryReader pvt = new BinaryReader(new MemoryStream(pivot));
                                rdms.ReadBytes(16);
                                long ck = rdms.ReadInt64();
                                if (ck == 4)
                                    rdms.ReadBytes(28);
                                int len = rdms.ReadInt32();
                                if (len > 0)
                                {
                                    string convert = Encoding.ASCII.GetString(rdms.ReadBytes(len - 1));
                                    StringBuilder sbd = new StringBuilder(convert);
                                    sbd.Replace("\r\n", "[rn]");
                                    sbd.Replace("\n\r", "[nr]");
                                    sbd.Replace("\n\n", "[nn]");
                                    sbd.Replace("\r\r", "[rr]");
                                    sbd.Replace("\r", "[r]");
                                    sbd.Replace("\n", "[n]");
                                    swt.Write(sbd.ToString() + "\n");
                                    float br = 0f;
                                    for (int i = 0; i < num; i++)
                                    {
                                        pvt.ReadSingle();
                                        float x = pvt.ReadSingle();
                                        pvt.ReadSingle();
                                        if (x < br)
                                        {
                                            swt.Write(i.ToString() + " ");
                                        }
                                        br = x;
                                    }
                                }
                                else
                                {
                                    string convert = Encoding.Unicode.GetString(rdms.ReadBytes(len * -2 - 2));
                                    StringBuilder sbd = new StringBuilder(convert);
                                    sbd.Replace("\r\n", "[rn]");
                                    sbd.Replace("\n\r", "[nr]");
                                    sbd.Replace("\n\n", "[nn]");
                                    sbd.Replace("\r\r", "[rr]");
                                    sbd.Replace("\r", "[r]");
                                    sbd.Replace("\n", "[n]");
                                    swt.Write(sbd.ToString() + "\n");
                                    float br = 0f;
                                    for (int i = 0; i < num; i++)
                                    {
                                        pvt.ReadSingle();
                                        float x = pvt.ReadSingle();
                                        pvt.ReadSingle();
                                        if (x < br)
                                        {
                                            swt.Write(i.ToString() + " ");
                                        }
                                        br = x;
                                    }
                                }
                                bwt.Write(buffer);
                                bwt.Close();
                                swt.Close();
                            }
                        }
                        catch
                        {

                        }
                        rdms.Close();
                        ms.Close();
                    }
                    MessageBox.Show("Done!");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            opf2.Filter = "FileSize Table|*.FileSize";
            if (opf2.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(opf2.FileName))
                {
                    string path = Path.GetDirectoryName(opf2.FileName);
                    string name = Path.GetFileNameWithoutExtension(opf2.FileName);
                    string binF = path + "\\" + name + "FileEx";
                    string txtF = path + "\\" + name + "FileTxT";
                    var crf = new BinaryWriter(new FileStream(path + "\\" + name + ".umap", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
                    var readf = new BinaryReader(new FileStream(path + "\\" + name + ".umap_bk", FileMode.Open, FileAccess.Read, FileShare.Read));
                    crf.Write(readf.ReadBytes(20));
                    int ckhash = readf.ReadInt32();
                    crf.Write(ckhash);
                    long offset_tb = 0;
                    int push = 0;
                    if (ckhash == 1)
                    {
                        crf.Write(readf.ReadBytes(20));
                        int offset_bg = readf.ReadInt32();
                        crf.Write(offset_bg);
                        crf.Write(readf.ReadBytes(33));
                        offset_tb = readf.ReadInt32();
                        crf.Write((int)offset_tb);
                        crf.Write(readf.ReadBytes(offset_bg - 85));
                    }
                    else
                    {
                        int offset_bg = readf.ReadInt32();
                        crf.Write(offset_bg);
                        crf.Write(readf.ReadBytes(33));
                        offset_tb = readf.ReadInt32();
                        crf.Write((int)offset_tb);
                        crf.Write(readf.ReadBytes(offset_bg - 65));
                    }
                    foreach (string line in File.ReadAllLines(opf2.FileName))
                    {
                        var data = line.Split(' ');
                        crf.BaseStream.Seek(offset_tb + 24, SeekOrigin.Begin);
                        if (File.Exists(binF + "\\" + data[0] + ".bin"))
                        {
                            using (var mf = new BinaryWriter(new FileStream(binF + "\\" + data[0] + ".new.bin", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)))
                            {
                                var rf = new BinaryReader(new FileStream(binF + "\\" + data[0] + ".bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                                var txt = File.ReadAllLines(txtF + "\\" + data[0] + ".txt");
                                StringBuilder mtxt = new StringBuilder(txt[0]);
                                mtxt.Replace("[rn]", "\r\n");
                                mtxt.Replace("[nr]", "\n\r");
                                mtxt.Replace("[nn]", "\n\n");
                                mtxt.Replace("[rr]", "\r\r");
                                mtxt.Replace("[r]", "\r");
                                mtxt.Replace("[n]", "\n");
                                byte[] btxt = Encoding.Unicode.GetBytes(mtxt.ToString());
                                mtxt.Replace("\r\n", "");
                                mtxt.Replace("\n\r", "");
                                mtxt.Replace("\n\n", "");
                                mtxt.Replace("\r\r", "");
                                mtxt.Replace("\r", "");
                                mtxt.Replace("\n", "");
                                mtxt.Replace(" ", "");
                                mf.Write(rf.ReadBytes(16));
                                mf.Write((Int64)mtxt.ToString().Length * 12 + 4);
                                rf.ReadInt64();
                                mf.Write(rf.ReadInt64());
                                mf.Write(mtxt.ToString().Length);
                                int ct = rf.ReadInt32();
                                if (mtxt.ToString().Length > ct)
                                {
                                    int otp = mtxt.ToString().Length - ct;
                                    mf.Write(rf.ReadBytes(ct * 12));
                                    for (int i = 0; i < otp; i++)
                                    {
                                        mf.Write(2.5f);
                                        mf.Write(0.0f);
                                        mf.Write(0.0f);
                                    }
                                }
                                else
                                {
                                    BinaryReader rbf = new BinaryReader(new MemoryStream(rf.ReadBytes(ct * 12)));
                                    mf.Write(rbf.ReadBytes(mtxt.ToString().Length * 12));
                                    rbf.Close();
                                }
                                mf.Write(rf.ReadInt64());
                                mf.Write(rf.ReadInt64());
                                long ckd = rf.ReadInt64();
                                if (ckd == 4)
                                {
                                    mf.Write(ckd);
                                    mf.Write(rf.ReadBytes(20));
                                    long lengy = rf.ReadInt64();
                                }
                                mf.Write((Int64)(btxt.Length + 6));
                                mf.Write((btxt.Length + 2) / 2 * -1);
                                mf.Write(btxt);
                                mf.Write((Int16)0);
                                int crt = rf.ReadInt32();
                                if (crt > 0)
                                {
                                    byte[] tx = rf.ReadBytes(crt);
                                }
                                else
                                {
                                    byte[] tx = rf.ReadBytes(crt * -2);
                                }
                                long pss = rf.BaseStream.Position;
                                byte[] ef = rf.ReadBytes((int)(long.Parse(data[1]) - pss));
                                mf.Write(ef);
                                mf.Close();
                                rf.Close();
                            }
                            byte[] newbin = File.ReadAllBytes(binF + "\\" + data[0] + ".new.bin");
                            if(newbin.Length > int.Parse(data[1]))
                            {
                                crf.Write(newbin.Length);
                                crf.Write(int.Parse(data[0]) + push);
                                offset_tb = crf.BaseStream.Position + 36;
                                crf.BaseStream.Seek(int.Parse(data[0]) + push, SeekOrigin.Begin);
                                crf.Write(newbin);
                                push += newbin.Length - int.Parse(data[1]);
                            }
                            else
                            {
                                crf.Write(newbin.Length);
                                crf.Write(int.Parse(data[0]) + push);
                                offset_tb = crf.BaseStream.Position + 36;
                                crf.BaseStream.Seek(int.Parse(data[0]) + push, SeekOrigin.Begin);
                                crf.Write(newbin);
                                push -= int.Parse(data[1]) - newbin.Length;
                            }
                            
                        }
                        else
                        {
                            crf.Write(int.Parse(data[1]));
                            crf.Write(int.Parse(data[0]) + push);
                            offset_tb = crf.BaseStream.Position + 36;
                            crf.BaseStream.Seek(int.Parse(data[0]) + push, SeekOrigin.Begin);
                            readf.BaseStream.Seek(long.Parse(data[0]), SeekOrigin.Begin);
                            crf.Write(readf.ReadBytes(int.Parse(data[1])));
                        }
                    }
                    byte[] allb = File.ReadAllBytes(path + "\\" + name + ".umap_bk");
                    byte[] endbytes = readf.ReadBytes((int)(allb.Length - readf.BaseStream.Position));
                    long mds = crf.BaseStream.Position;
                    crf.Write(endbytes);
                    if (ckhash == 1)
                        crf.BaseStream.Seek(189, SeekOrigin.Begin);
                    else
                        crf.BaseStream.Seek(169, SeekOrigin.Begin);
                    crf.Write(mds);
                    crf.Close();
                    readf.Close();
                    MessageBox.Show("Done!");
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://viethoagame.com");
        }
    }
}
