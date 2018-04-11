using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using PropertyDataGrid.Utilities;

namespace PropertyDataGrid.SampleApp
{
    public class DiagnosticsInformation
    {
        public DiagnosticsInformation(Assembly assembly)
        {
            Assembly = assembly ?? Assembly.GetExecutingAssembly();
        }

        public Assembly Assembly { get; }

        [Category("System")]
        public string VirtualMachineType => GetVirtualMachineType();

        [Category("System")]
        public string BoardModel => GetManagementInfo<string>("Win32_ComputerSystem", "Model", null);

        [Category("System")]
        public string BoardManufacturer => GetManagementInfo<string>("Win32_ComputerSystem", "Manufacturer", null);

        [Category("System")]
        public string Processor => GetManagementInfo<string>("Win32_Processor", "Name", null);

        [Category("System")]
        public int ProcessorCount => Environment.ProcessorCount;

        [Category("System")]
        public ProcessorArchitecture ProcessorArchitecture => GetProcessorArchitecture();

        [Category("System")]
        public string OSVersion => Environment.OSVersion.VersionString;

        [Category(".NET")]
        public IReadOnlyList<Version> InstalledFrameworkVersions => GetInstalledFrameworkVersions();

        [Category(".NET")]
        public Version ClrVersion => Environment.Version;

        [Category("Process")]
        public TokenElevationType TokenElevationType => GetTokenElevationType();

        [Category("Process")]
        public string Bitness => GetBitness();

        [Category("Process")]
        public string CurrentCulture => CultureInfo.CurrentCulture.Name;

        [Category("Process")]
        public string CurrentUICulture => CultureInfo.CurrentUICulture.Name;

        [Category("Process")]
        public DateTime UtcNow => DateTime.UtcNow;

        [Category("Process")]
        public DateTime Now => DateTime.Now;

        [Category("Process")]
        public string InstalledUICulture => CultureInfo.InstalledUICulture.Name;

        [Category("Shell")]
        public float DesktopDpiX => GetDpiSettings().Width;

        [Category("Shell")]
        public float DesktopDpiY => GetDpiSettings().Height;

        [Category("Shell")]
        public bool IsGlassEnabled => SystemParameters.IsGlassEnabled;

        [Category("Shell")]
        public string UxThemeColor => SystemParameters.UxThemeColor;

        [Category("Shell")]
        public string UxThemeName => SystemParameters.UxThemeName;

        [Category("Shell")]
        public bool IsTabletPC => SystemParameters.IsTabletPC;

        [Category("Shell")]
        public bool IsRemotelyControlled => SystemParameters.IsRemotelyControlled;

        [Category("Shell")]
        public bool IsRemoteSession => SystemParameters.IsRemoteSession;

        [Category("Shell")]
        public string Screens => string.Join(" | ", System.Windows.Forms.Screen.AllScreens.Select(s
            => "Name: " + s.DeviceName + (s.Primary ? "(Primary)" : null) + " Bpp: " + s.BitsPerPixel + " Bounds: " + s.Bounds + " WorkingArea: " + s.WorkingArea));

        [Category("Software")]
        public Version AssemblyVersion => Assembly.GetInformationalVersionVersion();

        [Category("Software")]
        public DateTime? AssemblyCompileDate => Assembly.GetLinkerTimestamp();

        [Category("Software")]
        public string AssemblyConfiguration => Assembly.GetConfiguration();

        [Category("Software")]
        public string AssemblyDisplayName
        {
            get
            {
                string name = "Version " + AssemblyVersion + " - " + AssemblyConfiguration + " - " + Bitness;
                DateTime? dt = AssemblyCompileDate;
                if (dt.HasValue)
                {
                    name += " - Compiled " + dt.Value;
                }
                return name;
            }
        }

        public static SizeF GetDpiSettings()
        {
            try
            {
                using (var g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    return new SizeF(g.DpiX, g.DpiY);
                }
            }
            catch
            {
                return new SizeF();
            }
        }

        public static TokenElevationType GetTokenElevationType()
        {
            var type = TokenElevationType.Unknown;
            int size = IntPtr.Size;
            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, out IntPtr handle))
                return type;

            try
            {
                GetTokenInformation(handle, TokenElevationTypeInformation, out type, size, out int returnLength);
                return type;
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        public static string GetBitness()
        {
            if (IntPtr.Size == 8)
                return "64-bit";

            if (Environment.Is64BitOperatingSystem)
                return "32-bit on a 64-bit OS";

            return "32-bit";
        }

        public static ProcessorArchitecture GetProcessorArchitecture()
        {
            var si = new SYSTEM_INFO();
            GetNativeSystemInfo(ref si);
            switch (si.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_AMD64:
                    return ProcessorArchitecture.Amd64;

                case PROCESSOR_ARCHITECTURE_IA64:
                    return ProcessorArchitecture.IA64;

                case PROCESSOR_ARCHITECTURE_INTEL:
                    return ProcessorArchitecture.X86;

                default:
                    return ProcessorArchitecture.None;
            }
        }

        public static IReadOnlyList<Version> GetInstalledFrameworkVersions()
        {
            var versions = new List<Version>();
            // http://astebner.sts.winisp.net/Tools/detectFX.cpp.txt
            using (var ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP"))
            {
                if (ndpKey != null)
                {
                    foreach (var keyName in ndpKey.GetSubKeyNames())
                    {
                        if (keyName != null && !keyName.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                            continue;

                        using (var key = ndpKey.OpenSubKey(keyName))
                        {
                            if (key == null || "deprecated".Equals(key.GetValue(null)))
                                continue;

                            string s = key.GetValue("Version", null) as string;
                            if (string.IsNullOrEmpty(s)) // FX4+?
                            {
                                foreach (var skeyName in key.GetSubKeyNames())
                                {
                                    using (var sk = key.OpenSubKey(skeyName))
                                    {
                                        if (sk == null)
                                            continue;

                                        s = sk.GetValue("Version", null) as string;
                                        if (!string.IsNullOrEmpty(s))
                                            break;
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(s))
                            {
                                // FX1
                                s = keyName.Substring(1);
                            }

                            if (Version.TryParse(s, out Version v))
                            {
                                versions.Add(v);
                            }
                        }
                    }

                }
            }
            versions.Sort(); // Version is IComparable
            return versions;
        }

        public static T GetManagementInfo<T>(string className, string propertyName, T defaultValue)
        {
            if (className == null)
                throw new ArgumentNullException(nameof(className));

            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            try
            {
                foreach (ManagementObject mo in new ManagementObjectSearcher(new WqlObjectQuery("select * from " + className)).Get())
                {
                    foreach (PropertyData data in mo.Properties)
                    {
                        if (data == null || data.Name == null)
                            continue;

                        if (data.Name.EqualsIgnoreCase(propertyName))
                            return Conversions.ChangeType(data.Value, defaultValue);
                    }
                }
            }
            catch
            {
                // do nothing
            }
            return defaultValue;
        }

        private static bool SearchASCIICaseInsensitive(byte[] bytes, string asciiString)
        {
            if (bytes == null || bytes.Length == 0)
                return false;

            var s = Encoding.ASCII.GetBytes(asciiString);
            if (s.Length > bytes.Length)
                return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                bool equals = true;
                for (int j = 0; j < s.Length; j++)
                {
                    var c1 = (char)bytes[i + j];
                    var c2 = (char)s[j];
                    if (char.ToLowerInvariant(c1) != char.ToLowerInvariant(c2))
                    {
                        equals = false;
                        break;
                    }
                }

                if (equals)
                    return true;
            }
            return false;
        }

        public static string GetVirtualMachineType()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\mssmbios\Data", false))
                {
                    if (key?.GetValue("SMBiosData") is byte[] bytes)
                    {
                        // detect known emulators
                        if (SearchASCIICaseInsensitive(bytes, "Microsoft Corporation - Virtual Machine"))
                            return "Hyper-V";

                        if (SearchASCIICaseInsensitive(bytes, "Microsoft"))
                            return "Virtual PC";

                        if (SearchASCIICaseInsensitive(bytes, "VMWare"))
                            return "VMWare";

                        if (SearchASCIICaseInsensitive(bytes, "VBox"))
                            return "Virtual Box";

                        if (SearchASCIICaseInsensitive(bytes, "Bochs"))
                            return "Bochs";

                        if (SearchASCIICaseInsensitive(bytes, "QEMU"))
                            return "QEMU";

                        if (SearchASCIICaseInsensitive(bytes, "Plex86"))
                            return "Plex86";

                        if (SearchASCIICaseInsensitive(bytes, "Parallels"))
                            return "Parallels";

                        if (SearchASCIICaseInsensitive(bytes, "Xen"))
                            return "Xen";

                        if (SearchASCIICaseInsensitive(bytes, "Virtual"))
                            return "Generic Virtual Machine";
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static string Serialize(Assembly assembly)
        {
            var di = new DiagnosticsInformation(assembly);
            var dic = new Dictionary<string, List<PropertyInfo>>();
            foreach (var prop in di.GetType().GetProperties())
            {
                var cat = prop.GetCustomAttribute<CategoryAttribute>();
                string catName = cat?.Category ?? "General";
                if (!dic.TryGetValue(catName, out List<PropertyInfo> props))
                {
                    props = new List<PropertyInfo>();
                    dic.Add(catName, props);
                }
                props.Add(prop);
            }

            var sb = new StringBuilder();
            foreach (var kv in dic)
            {
                sb.AppendLine("[" + kv.Key + "]");
                foreach (var prop in kv.Value)
                {
                    sb.AppendLine(" " + prop.Name + " = " + prop.GetValue(di));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            public short wProcessorArchitecture;
            public short wReserved;
            public int dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public int dwNumberOfProcessors;
            public int dwProcessorType;
            public int dwAllocationGranularity;
            public short wProcessorLevel;
            public short wProcessorRevision;
        }

        private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
        private const int PROCESSOR_ARCHITECTURE_IA64 = 6;
        private const int PROCESSOR_ARCHITECTURE_INTEL = 0;
        private const int TOKEN_QUERY = 8;
        private const int TokenElevationTypeInformation = 18;

        [DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, int TokenInformationClass, out TokenElevationType TokenInformation, int TokenInformationLength, out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
    }

    public enum TokenElevationType
    {
        Unknown = 0,
        Default = 1,
        Full = 2,
        Limited = 3,
    }

    public static class AssemblyUtilities
    {
        public static string GetConfiguration(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            object[] atts = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            if (atts != null && atts.Length > 0)
                return ((AssemblyConfigurationAttribute)atts[0]).Configuration;

            return null;
        }

        public static string GetTitle(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            object[] atts = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (atts != null && atts.Length > 0)
                return ((AssemblyTitleAttribute)atts[0]).Title;

            return null;
        }

        public static Version GetInformationalVersionVersion(this Assembly assembly)
        {
            var version = GetInformationalVersion(assembly);
            if (version == null)
                return new Version(0, 0);

            return new Version(version);
        }

        public static string GetInformationalVersion(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                object[] atts = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if ((atts != null) && (atts.Length > 0))
                    return ((AssemblyInformationalVersionAttribute)atts[0]).InformationalVersion;

                return null;
            }
            catch (Exception e)
            {
                return "!" + e.Message + "!";
            }
        }

        public static DateTime? GetLinkerTimestamp(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                return GetLinkerTimestamp(assembly.Location);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? GetLinkerTimestamp(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            try
            {
                const int peHeaderOffset = 60;
                const int linkerTimestampOffset = 8;
                var bytes = new byte[2048];

                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    file.Read(bytes, 0, bytes.Length);
                }

                int headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
                int secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
                var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                dt = dt.AddSeconds(secondsSince1970);
                dt = dt.ToLocalTime();
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}
