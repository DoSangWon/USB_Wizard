using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Security;
using System.ComponentModel;

namespace USB_Wizard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
       // public static string key; //password
        public static string dir;
        static byte[] xBuff = null;
        static byte[] xBuff2 = null;

        static public byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }

        static public string ByteToString(byte[] strByte)
        {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }

        public MainWindow()
        {
            InitializeComponent();
            //MessageBox.Show(dlg.txtAnswer.Password);dsdsds
            //key = dlg.txtAnswer.Password;


        }
        public class MyItem
        {

            public string ID { get; set; }

            public string Name { get; set; }

        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        IntPtr WndProc(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            UInt32 WM_DEVICECHANGE = 0x0219;
            UInt32 DBT_DEVTUP_VOLUME = 0x02;
            UInt32 DBT_DEVICEARRIVAL = 0x8000;
            UInt32 DBT_DEVICEREMOVECOMPLETE = 0x8004;

            //디바이스 연결시
            if ((nMsg == WM_DEVICECHANGE) && (wParam.ToInt32() == DBT_DEVICEARRIVAL))
            {
                int devType = Marshal.ReadInt32(lParam, 4);

                if (devType == DBT_DEVTUP_VOLUME)
                {
                    MessageBox.Show("USB가 삽입 되었습니다.");
                    RefreshDevice();
                }
            }

            //디바이스 연결 해제시...
            if ((nMsg == WM_DEVICECHANGE) && (wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE))
            {
                int devType = Marshal.ReadInt32(lParam, 4);
                if (devType == DBT_DEVTUP_VOLUME)
                {

                    MessageBox.Show("USB가 제거 되었습니다.");
                    RefreshDevice();
                }
            }

            return IntPtr.Zero;
        }
        /// <summary>
        /// 
        /// </summary>
        public void RefreshDevice()
        {
            lst_ComPort.Items.Clear();
            listView1.Items.Clear();
            tName.Content = "";
            tFormat.Content = "";
            tType.Content = "";
            pBarSize.Value = 0;
            string[] ls_drivers = System.IO.Directory.GetLogicalDrives();
            foreach (string device in ls_drivers)
            {
                System.IO.DriveInfo dr = new System.IO.DriveInfo(device);
                if (dr.DriveType == System.IO.DriveType.Removable) //제거 가능한 타입이라면
                {

                    lst_ComPort.Items.Add(device);




                    // textBox1.AppendText("총 size : " + Convert.ToString(dr.TotalSize) + Environment.NewLine);
                    //textBox1.AppendText("남은 size : " + Convert.ToString(dr.AvailableFreeSpace) + Environment.NewLine);
                    // textBox1.AppendText("포멧 : " + Convert.ToString(dr.DriveFormat) + Environment.NewLine);
                    //textBox1.AppendText("타입 : " + Convert.ToString(dr.DriveType) + Environment.NewLine);

                }
            }

        }

        public void drawListView()
        {
            int sel = lst_ComPort.SelectedIndex;

            String txt = lst_ComPort.SelectedItem as String;
            if (sel > -1)
            {
                //String msg = String.Format("selected index == {0}, {1}, {2}", sel, comboBox1.Items[sel], txt);
                //MessageBox.Show(msg);
                string[] ls_drivers = System.IO.Directory.GetLogicalDrives();
                int i = 0;

                string[] allDrives = System.IO.Directory.GetLogicalDrives();

                for (i = 0; i < allDrives.Length; i++)
                {
                    //MessageBox.Show(Convert.ToString(comboBox1.Items[sel]));
                    //MessageBox.Show(Convert.ToString(allDrives[i]));
                    if (Convert.ToString(lst_ComPort.Items[sel]) == allDrives[i])
                    {
                        System.IO.DriveInfo dr = new System.IO.DriveInfo(allDrives[i]);
                        tName.Content = dr.Name + Environment.NewLine;
                        tFormat.Content = dr.DriveFormat + Environment.NewLine;
                        tType.Content = dr.DriveType + Environment.NewLine;
                        pBarSize.Maximum = dr.TotalSize;
                        pBarSize.Value = dr.TotalSize - dr.AvailableFreeSpace;
                        
                        MessageBox.Show(Convert.ToString(pBarSize.Value));

                        dir = dr.Name;

                        string dirPath = @dr.Name;

                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);
                        String file = "*.*";
                        DirectoryInfo[] CInfo = di.GetDirectories(file, SearchOption.AllDirectories);
                        listView1.Items.Clear();
                        foreach(var items in di.GetFiles(file, SearchOption.TopDirectoryOnly))
                        {
                            listView1.Items.Add(new MyItem { ID = items.Name, Name = items.FullName });//리스트에 파일명과 Full 경로 출력
                        }
                        foreach (DirectoryInfo info in CInfo)
                        {
                            //listView1.Items.Add(info.Name);
                            
                            foreach (var item in info.GetFiles(file, SearchOption.TopDirectoryOnly))
                            {

                                // 파일 이름 출력

                                
                                
                                listView1.Items.Add(new MyItem { ID = item.Name, Name = item.FullName });

                                listView1.Items.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
                                listView1.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                                //리스트에 파일명과 Full 경로 출력                                                                   
                                //listView1.ItemsSource = item;
                                /*List<MyItem> items = new List<MyItem>();
                                  items.Add(new MyItem() { ID = item.Name, Name = item.FullName });

                                  listView1.ItemsSource = items;

                                  CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView1.ItemsSource);
                                  view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
                                */
                                // 파일 타입 (확장자) 출력

                                //Sort("ID", ListSortDirection.Ascending, items1);







                                //filename = item.Name;

                                //listBox1.Items.Add(item.Extension);
                                // 파일 생성날짜 출력

                                //listBox1.Items.Add(item.CreationTime);

                                //listBox1.Items.Add(item.FullName);
                                //listBox1.Items.Add(item.Directory);
                            }
                        }
                        
                    }
                    
                }
            }
        }

        public void lst_ComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            drawListView();
        }





        private void pBarSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }



        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            foreach (var list in listView1.SelectedItems)
            {
                if (list == null)
                {
                    return;
                }
                var list2 = list as MyItem;
                MessageBox.Show(list2.ID+"를 암호화 합니다.");

                string filename = list2.ID;

                passworddlg dlg = new passworddlg("패스워드를 입력하여 주십시오.");
                dlg.ShowDialog();

                Log(filename, "암호화 시작");

                if (dlg.DialogResult == true)
                {

                    String key = dlg.txtAnswer.Password;

                    string path = @list2.Name;
                    string enfile = path.Substring(0, path.LastIndexOf("."));
                    string ext = path.Substring(path.LastIndexOf("."));

                    //String str = AES.AESEncrypt256(path, enfile + "(암호화)" + ext, key);
                    String str = AES.AESEncrypt256(path, key);
                    Log(filename, "암호화 성공");
                    MessageBox.Show("암호화 된 문자열 : " + str);
                    //MessageBox.Show("암호화 된 문자열 : " + str);

                }
                else
                {
                    Log(filename, "암호화 실패");
                    MessageBox.Show("비밀번호를 입력해 주세요.");
                }


            }
                drawListView();
            }


        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            foreach (var list in listView1.SelectedItems)
            {
                if (list == null)
                {
                    return;
                }
                var list2 = list as MyItem;
                MessageBox.Show(list2.ID+"를 복호화 합니다.");

                string filename = list2.ID;

                passworddlg dlg = new passworddlg("패스워드를 입력하여 주십시오.");
                dlg.ShowDialog();

                Log(filename, "복호화 시작");

                if (dlg.DialogResult == true)
                {

                    String key = dlg.txtAnswer.Password;

                    string path = @list2.Name;
                    string enfile = path.Substring(0, path.LastIndexOf("."));
                    string ext = path.Substring(path.LastIndexOf("."));


                    //String str = AES.AESDecrypt256(enfile + ext, enfile + "(복호화)" + ext, key);
                    String str = AES.AESDecrypt256(path, key);
                    Log(filename, "복호화 성공");
                    MessageBox.Show("복호화 된 문자열 : " + str);
                    //MessageBox.Show("복호화 된 문자열 : " + str);

                }
                else
                {
                    Log(filename, "복호화 실패");
                    MessageBox.Show("비밀번호를 입력해 주세요.");
                }
                
            }
            drawListView();
        }

        public class AES
        {
            //AES_256 암호화
            public static String AESEncrypt256(string sInputFilename, String key)
            {
                byte[] b = null;
                string ext = sInputFilename.Substring(sInputFilename.LastIndexOf("."));
                int a = ext.Length;
                ext = a.ToString() + ext;
                using (FileStream f = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read))
                {
                    b = new byte[f.Length];

                    f.Read(b, 0, b.Length);
                    
                    //StreamReader sr = new StreamReader(f);
                    //string str = sr.ReadToEnd();
                    RijndaelManaged aes = new RijndaelManaged();
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    string sha = SHA256Hash(key);
                    String md5 = CreateMD5(sha);
                    //MessageBox.Show(str);
                    //MessageBox.Show("해시값은 " + str);
                    aes.Key = Encoding.UTF8.GetBytes(md5);
                    aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                    xBuff = null;


                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                        {
                            byte[] xXml = b;
                            //cs.Write(StringToByte(ext), 0, StringToByte(ext).Length);
                            cs.Write(xXml, 0, xXml.Length);
                            
                        }

                        xBuff = ms.ToArray();
                    }

                    //Output = Convert.ToBase64String(xBuff);
                    //b = StringToByte(str);
                }

                // Write to file ...
                using (FileStream fs = new FileStream(sInputFilename+".Crypto", FileMode.Create))
                {
                    //string ext = ".txt";
                    fs.Write(xBuff, 0, xBuff.Length);
                    //fs.Write(StringToByte(ext), 0, StringToByte(ext).Length);

                }
                File.Delete(sInputFilename);
                return "dd";



            }

            public static string CreateMD5(string input)
            {
                // Use input string to calculate MD5 hash
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }


            //AES_256 복호화
            public static String AESDecrypt256(string sInputFilename, String key)
            {
                string enfile = sInputFilename.Substring(0, sInputFilename.LastIndexOf("."));

                byte[] c = null;
                using (FileStream f = new FileStream(sInputFilename, FileMode.Open))
                {
                    c = new byte[f.Length];
                    //StreamReader sr = new StreamReader(f);
                    //string str = sr.ReadToEnd();
                    f.Read(c, 0, c.Length);

                    RijndaelManaged aes = new RijndaelManaged();
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    //String md5 = CreateMD5(key); //MD5 해시화
                    //aes.Key = Encoding.UTF8.GetBytes(md5);
                    string sha = SHA256Hash(key);
                    String md5 = CreateMD5(sha);
                    //MessageBox.Show("해시값은 " + str);
                    aes.Key = Encoding.UTF8.GetBytes(md5);
                    aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    //String md5 = CreateMD5("aaaaa");
                    // MessageBox.Show(md5);

                    var decrypt = aes.CreateDecryptor();
                    xBuff2 = null;
                    using (var ms2 = new MemoryStream())
                    {
                        using (var cs2 = new CryptoStream(ms2, decrypt, CryptoStreamMode.Write))
                        {
                            byte[] xXml2 = c;
                            cs2.Write(xXml2, 0, xXml2.Length);
                            
                        }

                        xBuff2 = ms2.ToArray();
                        
                    }
                    
                    //String Output = Encoding.UTF8.GetString(xBuff);
                    //b = StringToByte(Output);
                }

                // Write to file ...
                using (FileStream fs = new FileStream(enfile, FileMode.Create))
                {
                    //string a = ByteToString(xBuff2);
                    //string ext = a.Substring(a.LastIndexOf("."));
                    //MessageBox.Show("확장자 확인 :"+ext);
                    
 
                    fs.Write(xBuff2, 0, xBuff2.Length);
                    
                    

                }
                File.Delete(sInputFilename);
                return "dd";


            }

        }

        /// 

        /// ms까지 시간을 구하는 함수
        /// 

        /// 
        public string GetDateTime()
        {
            DateTime NowDate = DateTime.Now;
            return NowDate.ToString("yyyy-MM-dd HH:mm:ss") + ":" + NowDate.Millisecond.ToString("000");
        }


        /// 

        /// 로그 기록
        /// 

        /// 로그내용
        public void Log(string filename, string str)
        {
            //string path = @dir;
            //string enfile = path.Substring(0, path.LastIndexOf(filename));
            //string ext = path.Substring(path.LastIndexOf("."));

            string FilePath = dir + @"Logs\Log" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            string DirPath = dir + @"Logs";
           // MessageBox.Show(FilePath);
            //MessageBox.Show(DirPath);


            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                if (di.Exists != true) Directory.CreateDirectory(DirPath);

                if (fi.Exists != true)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        temp = string.Format("[{0}] : {1}", GetDateTime(), filename + " " + str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        //MessageBox.Show("dddd" + filename);
                        temp = string.Format("[{0}] : {1}", GetDateTime(), filename + " " + str);
                        sw.WriteLine(temp);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        public static string SHA256Hash(string Data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }


        public static void Sort(string PropertyName, ListSortDirection Direction, object ItemsSource)
        {
            // ItemsSource의 Default 뷰 가져오기
            ICollectionView CollectionView = CollectionViewSource.GetDefaultView(ItemsSource);

            // 기존에 등록되어 있던 SortDescriptions제거
            CollectionView.SortDescriptions.Clear();

            // SortDescription 추가
            SortDescription SortDescription = new SortDescription(PropertyName, Direction);
            CollectionView.SortDescriptions.Add(SortDescription);

            // 뷰 갱신
            CollectionView.Refresh();
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort2(header, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort2(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listView1.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

    }
}