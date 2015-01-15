using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MudObjectTransformTool
{
    [ComVisible(true)]
    [Guid("6C670E19-A809-4E9C-AA59-EF12A4F3BA7F")]
    [CustomTool("Mud Transform", "Transforms Mud Object source into csharp")]
    public class Transform : IVsSingleFileGenerator
    {
        internal static Guid CSharpCategoryGuid = new Guid("FAE04EC1-301F-11D3-BF4B-00C04F79EFBC");
        private const string VisualStudioVersion = "12.0";

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            var bytes = Encoding.UTF8.GetBytes(bstrInputFileContents);
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
            pcbOutput = (uint)bytes.Length;
            return VSConstants.S_OK;
        }

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            GuidAttribute guidAttribute = GetAttribute<GuidAttribute>(t);
            CustomToolAttribute customToolAttribute = GetAttribute<CustomToolAttribute>(t);
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
              GetKeyName(CSharpCategoryGuid, ".mud")))
            {
                key.SetValue("", ".mud");
                key.SetValue("CLSID", "{" + guidAttribute.Value + "}");
                key.SetValue("GeneratesDesignTimeSource", 1);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            CustomToolAttribute customToolAttribute = GetAttribute<CustomToolAttribute>(t);
            Registry.LocalMachine.DeleteSubKey(GetKeyName(
              CSharpCategoryGuid, ".mud"), false);
        }

         internal static A GetAttribute<A>(Type t)
       {
         object[] attributes = t.GetCustomAttributes(typeof(A), /* inherit */ true);
         if (attributes.Length == 0)
           throw new Exception(
             String.Format("Class '{0}' does not provide a '{1}' attribute.",
             t.FullName, typeof(A).FullName));
         return (A)attributes[0];
       }

        internal static string GetKeyName(Guid categoryGuid, string toolName)
       {
         return
           String.Format("SOFTWARE\\Microsoft\\VisualStudio\\" + VisualStudioVersion +
             "\\Generators\\{{{0}}}\\{1}\\", categoryGuid, toolName);
       }
   
    }
}
