using SELib;
using SolarXCod.SeLibWrapper.XModel;
using System.IO;

namespace SolarXCod.Converter
{
    public static class SEModel2Xmodel
    {
        public static void ConvertXmodelExport(FileInfo fileInfo)
        {
            string filePath = $"{fileInfo.Directory.FullName}\\{Path.GetFileNameWithoutExtension(fileInfo.Name)}.xmodel_export";
            SEModel originalModel = SEModel.Read(fileInfo.FullName);
            XModel xModel = new XModel(originalModel);

            xModel.WriteExport(File.Create(filePath));
        }

        public static void ConvertXmodelBin(FileInfo fileInfo)
        {
            string filePath = $"{fileInfo.Directory.FullName}\\{Path.GetFileNameWithoutExtension(fileInfo.Name)}.xmodel_bin";
            SEModel originalModel = SEModel.Read(fileInfo.FullName);
            XModel xModel = new XModel(originalModel);

            xModel.WriteBin(filePath);
        }
    }
}
