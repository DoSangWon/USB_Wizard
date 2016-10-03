
using Lenovo.Common.Devices;
using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace USB_Wizard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
        public partial class MainWindow : Window
        {
        UsbDetector usbDetector;
        public MainWindow()
       {
                InitializeComponent();
                usbDetector = new UsbDetector();
                usbDetector.StateChanged += new UsbStateChangedEventHandler(usbDetector_StateChanged);

                this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper interop = new WindowInteropHelper(this);
            HwndSource hwndSource = HwndSource.FromHwnd(interop.Handle);
            HwndSourceHook hool = new HwndSourceHook(usbDetector.HwndHandler);
            hwndSource.AddHook(hool); ;
            usbDetector.RegisterDeviceNotification(interop.Handle);
        }

        void usbDetector_StateChanged(bool arrival)
        {
            if (arrival)
                MessageBox.Show("USB가 연결되었습니다.");
            else
                MessageBox.Show("USB가 제거되었습니다.");
        }

    }
    }

