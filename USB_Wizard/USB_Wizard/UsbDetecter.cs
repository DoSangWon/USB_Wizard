using System.Runtime.InteropServices;
using System;

namespace Lenovo.Common.Devices
{
    public delegate void UsbStateChangedEventHandler(bool arrival);
    public class UsbDetector
    {
        public enum WM_DEVICECHANGE_WPPARAMS
        {
            DBT_DEVICEARRIVAL = 0x8000,
            DBT_DEVICEQUERYREMOVE = 0x8001,
            DBT_DEVICEREMOVECOMPLETE = 0x8004,
            DBT_CONFIGCHANGECANCELED = 0x19,
            DBT_CONFIGCHANGED = 0x18,
            DBT_CUSTOMEVENT = 0x8006,
            DBT_DEVICEQUERYREMOVEFAILED = 0x8002,
            DBT_DEVICEREMOVEPENDING = 0x8003,
            DBT_DEVICETYPESPECIFIC = 0x8005,
            DBT_DEVNODES_CHANGED = 0x7,
            DBT_QUERYCHANGECONFIG = 0x17,
            DBT_USERDEFINED = 0xFFFF
        }
        const int WM_DEVICECHANGE = 0x0219;

        public IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr LParam, ref bool handled)
        {
            ProcessWinMessage(msg, wParam, LParam);
            //  handled = false;
            return IntPtr.Zero;
        }

        public event UsbStateChangedEventHandler StateChanged;

        public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr LParam)
        {
            //  if ((msg == WM_DEVICECHANGE) && (LParam != IntPtr.Zero))
            if (msg == WM_DEVICECHANGE)
            {
                switch (wParam.ToInt32())
                {
                    case Win32.DBT_DEVICEARRIVAL:
                        if (StateChanged != null)
                        {
                            StateChanged(true);
                        }
                        break;
                    case Win32.DBT_DEVICEREMOVECOMPLETE:
                        if (StateChanged != null)
                        {
                            StateChanged(false);
                        }
                        break;
                    case Win32.DBT_DEVNODES_CHANGED:
                        if (StateChanged != null)
                        {
                            StateChanged(false);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        // private const string USBClassID = "c671678c-82c1-43f3-d700-0049433e9a4b";
        //http://msdn.microsoft.com/en-us/library/ff545972.aspx
        private const string USBClassID = "A5DCBF10-6530-11D2-901F-00C04FB951ED";
        public IntPtr RegisterDeviceNotification(IntPtr hwnd)
        {
            Win32.DEV_BROADCAST_DEVICEINTERFACE deviceInterface = new Win32.DEV_BROADCAST_DEVICEINTERFACE();
            int size = Marshal.SizeOf(deviceInterface);
            deviceInterface.dbcc_size = size;
            //    deviceInterface.dbcc_devicetype = Win32.DBT_DEVTYP_VOLUME;
            deviceInterface.dbcc_reserved = 0;
            //deviceInterface.dbcc_handle = hwnd;
            //deviceInterface.dbcc_hdevnotify = (IntPtr)0;
            deviceInterface.dbcc_classguid = new Guid(USBClassID).ToByteArray();
            IntPtr buffer = IntPtr.Zero;
            buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);
            IntPtr r = IntPtr.Zero;
            r = Win32.RegisterDeviceNotification(hwnd, buffer,
                (Int32)(Win32.DEVICE_NOTIFY.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES
                        | Win32.DEVICE_NOTIFY.DEVICE_NOTIFY_SERVICE_HANDLE |
                        Win32.DEVICE_NOTIFY.DEVICE_NOTIFY_WINDOW_HANDLE)
                );

            return r;
        }
    }
}