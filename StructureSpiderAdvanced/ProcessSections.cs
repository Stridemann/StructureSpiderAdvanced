////Many thanks to https://github.com/ReClassNET/ReClass.NET

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StructureSpiderAdvanced
{
    public class ProcessSections
    {
        public delegate void EnumerateRemoteModuleCallback(ref EnumerateRemoteModuleData data);

        public delegate void EnumerateRemoteSectionCallback(ref EnumerateRemoteSectionData data);

        public delegate void EnumerateRemoteSectionsAndModulesDelegate(IntPtr process,
            [MarshalAs(UnmanagedType.FunctionPtr)] EnumerateRemoteSectionCallback callbackSection,
            [MarshalAs(UnmanagedType.FunctionPtr)] EnumerateRemoteModuleCallback callbackModule);

        private readonly List<Section> _sections = new List<Section>();
        private readonly EnumerateRemoteSectionsAndModulesDelegate enumerateRemoteSectionsAndModulesDelegate;

        public ProcessSections()
        {
            var libraryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NativeCore.dll");
            var handle = LoadLibrary(libraryPath);

            enumerateRemoteSectionsAndModulesDelegate =
                GetFunctionDelegate<EnumerateRemoteSectionsAndModulesDelegate>(handle, "EnumerateRemoteSectionsAndModules");
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        protected static TDelegate GetFunctionDelegate<TDelegate>(IntPtr handle, string function)
        {
            var address = GetProcAddress(handle, function);

            if (address.IsNull())
                throw new Exception($"Function '{function}' not found.");

            return Marshal.GetDelegateForFunctionPointer<TDelegate>(address);
        }

        public Section GetSectionToPointer(IntPtr address)
        {
            return _sections.BinaryFind(s => address.CompareToRange(s.Begin, s.End));
        }

        public void UpdateProcessInformations(IntPtr process)
        {
            EnumerateRemoteSectionsAndModules(process, out var newSections);
            newSections.Sort((s1, s2) => s1.Begin.CompareTo(s2.Begin));
            _sections.Clear();
            _sections.AddRange(newSections);
        }

        public void EnumerateRemoteSectionsAndModules(IntPtr process, out List<Section> sections)
        {
            sections = new List<Section>();
            EnumerateRemoteSectionsAndModules(process, sections.Add);
        }

        public void EnumerateRemoteSectionsAndModules(IntPtr process, Action<Section> callbackSection)
        {
            var c1 = callbackSection == null
                ? null
                : (EnumerateRemoteSectionCallback) delegate(ref EnumerateRemoteSectionData data)
                {
                    callbackSection(new Section
                    {
                        Begin = data.BaseAddress,
                        End = data.BaseAddress.Add(data.Size),
                        Size = data.Size,
                        Name = data.Name,
                        Protection = data.Protection,
                        Type = data.Type,
                        ModulePath = data.ModulePath,
                        ModuleName = Path.GetFileName(data.ModulePath),
                        Category = data.Category
                    });
                };

            var c2 = (EnumerateRemoteModuleCallback) delegate { };

            EnumerateRemoteSectionsAndModules(process, c1, c2);
        }

        public void EnumerateRemoteSectionsAndModules(IntPtr process, EnumerateRemoteSectionCallback callbackSection,
            EnumerateRemoteModuleCallback callbackModule)
        {
            enumerateRemoteSectionsAndModulesDelegate(process, callbackSection, callbackModule);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct EnumerateRemoteSectionData
        {
            public IntPtr BaseAddress;
            public IntPtr Size;
            public SectionType Type;
            public SectionCategory Category;
            public SectionProtection Protection;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string Name;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string ModulePath;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public struct EnumerateRemoteModuleData
        {
            public IntPtr BaseAddress;
            public IntPtr Size;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string Path;
        }
    }
}
