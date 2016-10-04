using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace USB_Wizard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
       public MainWindow()
        {
            InitializeComponent();
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
           /* tName.Content = "";
            tFormat.Content = "";
            tType.Content = "";
            pBarSize.Value = 0;*/
            string[] ls_drivers = System.IO.Directory.GetLogicalDrives();
            foreach (string device in ls_drivers)
            {
                System.IO.DriveInfo dr = new System.IO.DriveInfo(device);
                if (dr.DriveType == System.IO.DriveType.Removable) //제거 가능한 타입이라면
                {
                   
                    lst_ComPort.Items.Add(device);
                    lst_ComPort.SelectedIndex = 0;
                    
                   tName.Content = dr.Name+Environment.NewLine;
                   tFormat.Content = dr.DriveFormat + Environment.NewLine;
                   tType.Content = dr.DriveType + Environment.NewLine;
                   pBarSize.Maximum = dr.TotalSize;
                   pBarSize.Value = dr.AvailableFreeSpace;

                    // textBox1.AppendText("총 size : " + Convert.ToString(dr.TotalSize) + Environment.NewLine);
                    //textBox1.AppendText("남은 size : " + Convert.ToString(dr.AvailableFreeSpace) + Environment.NewLine);
                    // textBox1.AppendText("포멧 : " + Convert.ToString(dr.DriveFormat) + Environment.NewLine);
                    //textBox1.AppendText("타입 : " + Convert.ToString(dr.DriveType) + Environment.NewLine);

                }
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void lst_ComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_ComPort.SelectedIndex >= 0)
            {
                itemSelected = lst_ComPort.SelectedItem as string;
                
            }
        }
        private string itemSelected;


        private void pBarSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}