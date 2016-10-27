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

namespace USB_Wizard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string key; //password

       public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("암호화&복호화에 적용될 Password를 입력하여 주십시오.");
            passworddlg dlg = new passworddlg("패스워드를 입력하여 주십시오.");
            dlg.ShowDialog();
            

            
            

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
        private void lst_ComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        pBarSize.Value = dr.AvailableFreeSpace;

                        string dirPath = @dr.Name;
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dirPath);
                        String file = "*.*";
                        DirectoryInfo[] CInfo = di.GetDirectories(file, SearchOption.AllDirectories);
                        listView1.Items.Clear();
                        foreach (DirectoryInfo info in CInfo)
                        {
                            //listView1.Items.Add(info.Name);
                            
                            foreach (var item in info.GetFiles(file, SearchOption.TopDirectoryOnly))
                            {

                                // 파일 이름 출력


                                listView1.Items.Add(new MyItem {ID=item.Name, Name=item.FullName});//리스트에 파일명과 Full 경로 출력

                                // 파일 타입 (확장자) 출력

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
        




        private void pBarSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        
       

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            foreach(var list in listView1.SelectedItems)
            {
                if(list == null)
                {
                    return;
                }
                var list2 = list as MyItem;
                MessageBox.Show(list2.ID);
                
            }
        }
    }
}