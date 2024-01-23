using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsyDev.QAScenario.Generator.Models
{
    public static class DocumentExtensions
    {
        public static string PDF { get; } = "pdf";
        public static string DOCX { get; } = "docx";
        public static string DOC { get; } = "doc";
        public static string PPT { get; } = "pdf";
        public static string PPTX { get; } = "pdf";
        public static string XLS { get; } = "xls";
        public static string XLSX { get; } = "xlsx";
        public static string MDB { get; } = "mdf";
        public static string JPG { get; } = "jpg";
        public static string JPEG { get; } = "jpeg";
        public static string PNG { get; } = "png";
        public static string GIF { get; } = "gif";
        public static string MP4 { get; } = "mp4";
        public static string MP3 { get; } = "mp3";
        public static string WEBM { get; } = "webm";
        public static string OGG { get; } = "ogg";
        //public static string PDF { get; } = ".pdf";
        //public static string DOCX { get; } = ".docx";
        //public static string DOC { get; } = ".doc";
        //public static string PPT { get; } = ".pdf";
        //public static string PPTX { get; } = ".pdf";
        //public static string XLS { get; } = ".xls";
        //public static string XLSX { get; } = ".xlsx";
        //public static string MDB { get; } = ".mdf";
        //public static string JPG { get; } = ".jpg";
        //public static string JPEG { get; } = ".jpeg";
        //public static string PNG { get; } = ".png";
        //public static string GIF { get; } = ".gif";
        //public static string MP4 { get; } = ".mp4";
        //public static string MP3 { get; } = ".mp3";
        //public static string WEBM { get; } = ".webm";
        //public static string OGG { get; } = ".ogg";



    }

    public enum DocumentExtensionEnum
    {
        PDF,
        DOCX,
        DOC,
        PPT,
        PPTX,
        XLS,
        XLSX,
        MDB,
        JPG,
        JPEG,
        PNG,
        GIF,
        MP4,
        MP3,
        WEBM,
        OGG,

    }
}
