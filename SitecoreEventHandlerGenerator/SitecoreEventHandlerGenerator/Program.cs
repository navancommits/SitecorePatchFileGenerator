using System;
using System.IO;
using System.Text;

namespace SitecoreEventHandlerGenerator
{
    class Program
    {
        private static string _folderPath = string.Empty;
        private static string _configPatchFileName = string.Empty;
        private static string _patchCSFileName = string.Empty;
        private static string _eventHandlersPath = string.Empty;
        private static string _includeFolderPath = string.Empty;

        private static string _eventHandlersFolderName = string.Empty;
        private static string _eventName = string.Empty;
        private static string _patchIncludeSubFolderPath = string.Empty;

        static void Main(string[] args)
        {
            _folderPath = args[0];
            _configPatchFileName = args[1];
            _patchCSFileName = args[2];
            _eventHandlersFolderName = args[3];
            _patchIncludeSubFolderPath = args[4];
            _eventName = args[5];

            //_eventHandlersFolderName = "EventHandlers";
            //_patchIncludeSubFolderPath = @"App_Config\Include\";
            //_eventName = "item:copying";

            //_folderPath =
            //    @"C:\Projects\xyz\xyz-main-site-9.2\xyz.MainSite\xyz.Feature.Calculator\";
            //_configPatchFileName = "ClientName.Feature.FeatureName";
            //_patchCSFileName = "ItemCopying";
            _eventHandlersPath = _folderPath + _eventHandlersFolderName +  @"\";
            _includeFolderPath= _folderPath + _patchIncludeSubFolderPath;

            if (!ConfigPatchFileFound(_folderPath)) CreateConfigPatchFile(_configPatchFileName, AddPatch());
            if (!EventHandlersFolderFound(_folderPath)) CreateEventHandlersFolder(_folderPath);
            if (!CSFileFound(_folderPath)) CreatePatchClassMethod(AddCSCode());
        }

        private static bool ConfigPatchFileFound(string folderPath)
        {
           return folderPath.Contains(_patchIncludeSubFolderPath);
        }

        private static bool CSFileFound(string folderPath)
        {
            return folderPath.Contains(@"\" + _eventHandlersFolderName + @"\" + _patchCSFileName + ".cs");
        }

        private static bool EventHandlersFolderFound(string folderPath)
        {
            return folderPath.Contains(_eventHandlersFolderName);
        }

        private static void CreateEventHandlersFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath + _eventHandlersFolderName);
        }

        private static void CreateConfigPatchFile(string configPatchFileName,string concatLines)
        {
            if (!string.IsNullOrWhiteSpace(_includeFolderPath))
            {
                if (!File.Exists(Path.GetDirectoryName(_includeFolderPath) + configPatchFileName + ".config"))
                {
                    using (FileStream fs = File.Create(_includeFolderPath + configPatchFileName + ".config"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(concatLines);
                        fs.Write(info, 0, info.Length);
                    }
                }
            }

            Console.WriteLine("Successful write to " + _includeFolderPath + configPatchFileName + ".config");
        }

        private static void CreatePatchClassMethod(string concatCSLines)
        {
            if (!string.IsNullOrWhiteSpace(_eventHandlersPath))
            {
                if (!File.Exists(Path.GetDirectoryName( _eventHandlersPath)  + _patchCSFileName + ".cs"))
                {
                    using (FileStream fs = File.Create(_eventHandlersPath  + _patchCSFileName + ".cs"))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(concatCSLines);
                        fs.Write(info, 0, info.Length);
                    }
                }
            }

            Console.WriteLine("Successful write to " +  _eventHandlersPath  + _patchCSFileName + ".cs");
        }

        private static string AddPatch()
        {
            var openingLines =
                "<configuration xmlns:patch=\"http://www.sitecore.net/xmlconfig/\" xmlns:role=\"http://www.sitecore.net/xmlconfig/role/\" >" +
                Environment.NewLine + "  <sitecore>" + Environment.NewLine + "    <events>" + Environment.NewLine;
            var actualLines= "      <event name=\"" + _eventName + "\"  help=\"Receives an argument of type array\">" + Environment.NewLine + "        <handler type=\"" + _configPatchFileName + "." + _eventHandlersFolderName + "." + _patchCSFileName +", "  + _configPatchFileName + "\" method=\"Process" + _patchCSFileName + "\" />" +
                             Environment.NewLine + "      </event>";
            var endingLines = Environment.NewLine + "    </events>" + Environment.NewLine + "  </sitecore>" + Environment.NewLine + "</configuration>";

            return openingLines + actualLines + endingLines;
        }

        private static string AddCSCode()
        {
            var openingLines =
                "using Sitecore.Data.Items;" +
                Environment.NewLine + "using Sitecore.Events;" + Environment.NewLine + "using System;" + Environment.NewLine + Environment.NewLine;
            var nameSpaceLine = "namespace " + _configPatchFileName + "." + _eventHandlersFolderName + Environment.NewLine + "{" + Environment.NewLine;
            var classOpeningLine = "    public class " + _patchCSFileName + Environment.NewLine + "    {" + Environment.NewLine;
            var methodOpeningLine = "        public void Process" + _patchCSFileName + "(object sender, EventArgs args)" + Environment.NewLine + "        {" + Environment.NewLine;

            var actualLines = "            //Item item = Event.ExtractParameter(args, 0) as Item;" + Environment.NewLine;
            actualLines += "            //if(item?.TemplateID?.ToString() != \"Dummy Id\") return" + Environment.NewLine;

            var methodClosingLine = "        }" + Environment.NewLine;
            var classClosingLine = "    }" + Environment.NewLine;
            var nameSpaceClosingLine = "}";

            return openingLines + nameSpaceLine + classOpeningLine + methodOpeningLine + actualLines + methodClosingLine + classClosingLine + nameSpaceClosingLine;
        }
    }

}
