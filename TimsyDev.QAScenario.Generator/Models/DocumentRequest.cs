using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsyDev.QAScenario.Generator.Models
{
    public class DocumentRequest
    {
        public string TestId { get; set; } = "0";
        public string TesterFirstName { get; set; } = "INSERT_NAME";
        public string Uploader { get; set; } = "ClientA";
        public string Privacy { get; set; } = "SHARED";
        public string Category { get; set; } = "General";
        public DateTime DocumentDate { get; set; } = DateTime.Today;
        public DateTime UploadDate { get; set; } = DateTime.Today;
        public string DocumentType{ get; set; } = DocumentExtensions.PDF;
        public DocumentExtensionEnum DocExtension => GetEnumFromString(DocumentType);
        public string SubCategory { get; set; } = "NoSubCate";
        public bool IsHouseHold { get; set; } = false;
        public string AccountIdentifier { get; set; } = "1";
        public string Project { get; set; } = "Tracker";
        public string UploadDateStr => $"UPLOAD-{UploadDate:MMMdd}";
        public string DocumentDateStr => $"DOCDATE-{DocumentDate:MMMdd}";
        public string AccountTypeStr => IsHouseHold ? $"HouseHold{AccountIdentifier}" : $"Account{AccountIdentifier}";
        public string TesterName => TesterFirstName.Trim().ToUpper();

        public string FullReturnFileName =>
            $"QA-{Project}-{TestId}-{Uploader}-{Privacy}-{Category}-{SubCategory}-{UploadDateStr}-{DocumentDateStr}-{AccountTypeStr}-{GetDefaultFileNameFromEnum()}";


        public DocumentExtensionEnum GetEnumFromString(string extension)
        {
            extension = extension.ToUpper();
            if(!DocumentExtensionEnum.TryParse(extension, out DocumentExtensionEnum ext))
            {
                throw new Exception("Invalid Extension");
            }

            return ext;
        }

        public string GetDefaultFileNameFromEnum()
        {
            var ext = DocExtension.ToString();
            return $"{ext}.{ext.ToLower()}";
        }

    }
}
