
using System;
using System.Windows;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;


namespace USB_Wizard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        decimal LineNumber = 1;

        SerialPort SP = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("USB 드라이브를 삽입하여 주십시오.");
         
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
                    GetSerialPort();
                }
            }

            //디바이스 연결 해제시...
            if ((nMsg == WM_DEVICECHANGE) && (wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE))
            {
                int devType = Marshal.ReadInt32(lParam, 4);
                if (devType == DBT_DEVTUP_VOLUME)
                {
                    GetSerialPort();
                }
            }

            return IntPtr.Zero;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(new HwndSourceHook(this.WndProc));

            SP.DataReceived += new SerialDataReceivedEventHandler(SP_DataReceived);
        }

        private void SP_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                string str = SP.ReadLine();
                //textBox1.AppendText("[" + LineNumber++.ToString() + "] : " + str);
                //textBox1.ScrollToEnd();
            }));
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            GetSerialPort();
        }
        private void GetSerialPort()
        {
            comboBox.Items.Clear();//콤보박스 원래 리스트였음.
            foreach (string comport in SerialPort.GetPortNames())
            {
                comboBox.Items.Add(comport);
            }

            if (comboBox.Items.Count <= 0)
            {
                comboBox.Items.Add("찾을 수 없음");
            }
        }
    }

}

