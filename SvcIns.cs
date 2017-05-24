using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Toolkits {

    /// <summary>
    /// toolkit of manager of service
    /// </summary>
    public class SvcIns {
        private const string InstallCommand = "i";
        private const string UninstallCommand = "u";

        public static void Main(string[] args, string svcLocation, string svcName, string displayName, ServiceBase svcBase) {
            Hashtable custom = GetArgsFromCommand(args);
            bool result = false;
            if (custom.ContainsKey(InstallCommand)) {
                result = InstallService(svcLocation, svcName, displayName);
            } else if (custom.ContainsKey(UninstallCommand)) {
                result = UnInstallService(svcName);
            } else {
                try {
                    ServiceBase.Run(svcBase);
                    result = true;
                } catch {
                    throw;
                }
            }
            if (result) {
                Console.WriteLine("Successful");
            } else {
                Console.WriteLine("Error occurs");
            }
        }

        private static Hashtable GetArgsFromCommand(string[] args) {
            Hashtable custom = CollectionsUtil.CreateCaseInsensitiveHashtable();
            for (int i = 0; i < args.Length; i++) {
                string arg = args[i];
                string value = bool.TrueString;
                if (!string.IsNullOrEmpty(arg)) {
                    if (arg.StartsWith("/", StringComparison.CurrentCulture)) {
                        arg = arg.Substring(1);
                    } else if (arg.StartsWith("-", StringComparison.CurrentCulture)) {
                        arg = arg.Substring(2);
                    }
                    int index = arg.IndexOf(":", StringComparison.CurrentCulture);
                    if (index > 0) {
                        value = arg.Substring(index + 1);
                        arg = arg.Substring(0, index);
                    }
                    custom.Add(arg, value);
                }
            }
            return custom;
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, uint dwDesiredAccess, uint dwServiceType, uint dwStartType, uint dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, string lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        internal static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll")]
        internal static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll")]
        internal static extern int DeleteService(IntPtr SVHANDLE);

        /// <summary>
        /// install the service
        /// </summary>
        /// <param name="servicePath">location of the service</param>
        /// <param name="serviceName">name of the service</param>
        /// <param name="serviceDisplayName">showing name of the service</param>
        /// <returns>
        /// Success : true
        /// Failue : false
        /// </returns>
        public static bool InstallService(string servicePath, string serviceName, string serviceDisplayName) {
            uint SC_MANAGER_CREATE_SERVICE = 0x0002;
            uint SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            uint SERVICE_ERROR_NORMAL = 0x00000001;
            uint STANDARD_RIGHTS_REQUIRED = 0xF0000;
            uint SERVICE_QUERY_CONFIG = 0x0001;
            uint SERVICE_CHANGE_CONFIG = 0x0002;
            uint SERVICE_QUERY_STATUS = 0x0004;
            uint SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            uint SERVICE_START = 0x0010;
            uint SERVICE_STOP = 0x0020;
            uint SERVICE_PAUSE_CONTINUE = 0x0040;
            uint SERVICE_INTERROGATE = 0x0080;
            uint SERVICE_USER_DEFINED_CONTROL = 0x0100;
            uint SERVICE_ALL_ACCESS = (
                STANDARD_RIGHTS_REQUIRED |
                SERVICE_QUERY_CONFIG |
                SERVICE_CHANGE_CONFIG |
                SERVICE_QUERY_STATUS |
                SERVICE_ENUMERATE_DEPENDENTS |
                SERVICE_START |
                SERVICE_STOP |
                SERVICE_PAUSE_CONTINUE |
                SERVICE_INTERROGATE |
                SERVICE_USER_DEFINED_CONTROL);
            uint SERVICE_AUTO_START = 0x00000002;

            try {
                IntPtr handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                bool result = false;
                if (handle.ToInt64() != 0) {
                    IntPtr serviceHandle = CreateService(handle, serviceName, serviceDisplayName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, servicePath, null, null, null, null, null);
                    result = (serviceHandle.ToInt64() != 0);
                    CloseServiceHandle(handle);
                }
                return result;
            } catch {
                throw;
            }
        }

        /// <summary>
        /// uninstall
        /// </summary>
        /// <param name="serviceName">name of service</param>
        public static bool UnInstallService(string serviceName) {
            uint GENERIC_WRITE = 0x40000000;

            try {
                IntPtr handle = OpenSCManager(null, null, GENERIC_WRITE);
                bool result = false;
                if (handle.ToInt64() != 0) {
                    uint DELETE = 0x10000;
                    IntPtr serviceHandle = OpenService(handle, serviceName, DELETE);
                    if (serviceHandle.ToInt64() != 0) {
                        result = (DeleteService(serviceHandle) != 0);
                        CloseServiceHandle(handle);
                    }
                }
                return result;
            } catch {
                throw;
            }
        }

        /// <summary>
        /// start the service
        /// </summary>
        /// <param name="serviceName">the serverice to start</param>
        public static bool StartService(string serviceName) {
            uint GENERIC_WRITE = 0x40000000;
            uint STANDARD_RIGHTS_REQUIRED = 0xF0000;
            uint SERVICE_QUERY_CONFIG = 0x0001;
            uint SERVICE_CHANGE_CONFIG = 0x0002;
            uint SERVICE_QUERY_STATUS = 0x0004;
            uint SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            uint SERVICE_START = 0x0010;
            uint SERVICE_STOP = 0x0020;
            uint SERVICE_PAUSE_CONTINUE = 0x0040;
            uint SERVICE_INTERROGATE = 0x0080;
            uint SERVICE_USER_DEFINED_CONTROL = 0x0100;
            uint SERVICE_ALL_ACCESS = (
                STANDARD_RIGHTS_REQUIRED |
                SERVICE_QUERY_CONFIG |
                SERVICE_CHANGE_CONFIG |
                SERVICE_QUERY_STATUS |
                SERVICE_ENUMERATE_DEPENDENTS |
                SERVICE_START |
                SERVICE_STOP |
                SERVICE_PAUSE_CONTINUE |
                SERVICE_INTERROGATE |
                SERVICE_USER_DEFINED_CONTROL);

            try {
                IntPtr handle = OpenSCManager(null, null, GENERIC_WRITE);
                bool result = false;
                if (handle.ToInt64() != 0) {
                    IntPtr serviceHandle = OpenService(handle, serviceName, SERVICE_ALL_ACCESS);
                    if (serviceHandle.ToInt64() != 0) {
                        result = (StartService(serviceHandle, 0, null) != 0);
                        CloseServiceHandle(handle);
                    }
                }
                return result;
            } catch {
                throw;
            }
        }
    }
}
