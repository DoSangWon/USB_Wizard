using System.Runtime.InteropServices;
using System;
namespace Lenovo.Common.Devices
{
    public class Win32
    {

        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

        [Flags]
        public enum DEVICE_NOTIFY : uint
        {
            DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000,
            DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001,
            DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004
        }


        public const int SERVICE_CONTROL_STOP = 0x00000001;
        public const int SERVICE_CONTROL_DEVICEEVENT = 0x00000011;
        public const int SERVICE_CONTROL_SHUTDOWN = 0x00000005;

        /*DBT_DEVTYP_DEVICEINTERFACE 0x00000005
            Class of devices. This structure is a DEV_BROADCAST_DEVICEINTERFACE structure.
 
            DBT_DEVTYP_HANDLE   0x00000006
            File system handle. This structure is a DEV_BROADCAST_HANDLE structure.
 
            DBT_DEVTYP_OEM  0x00000000
            OEM- or IHV-defined device type. This structure is a DEV_BROADCAST_OEM structure.
 
            DBT_DEVTYP_PORT 0x00000003
            Port device (serial or parallel). This structure is a DEV_BROADCAST_PORT structure.
 
            DBT_DEVTYP_VOLUME 0x00000002
            Logical volume. This structure is a DEV_BROADCAST_VOLUME structure.
 */

        public const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        public const int DBT_DEVTYP_HANDLE = 0x00000006;
        public const int DBT_DEVTYP_OEM = 0x00000000;
        public const int DBT_DEVTYP_PORT = 0x00000003;
        public const int DBT_DEVTYP_VOLUME = 0x00000002;


        public const int WM_DEVICECHANGE = 0x0219;                // device state change
        public const int DBT_DEVICEARRIVAL = 0x8000;            // detected a new device
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;        // preparing to remove
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // removed 
        public const int DBT_DEVNODES_CHANGED = 0x0007; //A device has been added to or removed from the system.


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr intPtr, IntPtr notificationFilter, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            //public IntPtr dbcc_handle;
            //public IntPtr dbcc_hdevnotify;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            public byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public Char[] dbcc_name;
            //public byte dbcc_data;
            //public byte dbcc_data1; 

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            public Byte dbch_data;
            public Byte dbch_data1;
        }
    }
}