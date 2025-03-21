// <copyright file="MimeTypeHandler.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    /// <summary>
    /// Mime type handler
    /// for document uploads
    /// </summary>
    public class MimeTypeHandler
    {
        /// <summary>
        /// Gets the mime type
        /// based on the file name
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>string mime type</returns>
        public static string? GetMimeTypeBasedOnFileName(string? fileName)
        {
            string? mimeType = null;
            string? extension = GetFileExtension(fileName);

            switch (extension)
            {
                case ".aac": 
                    mimeType = "audio/aac"; 
                    break;
                case ".abw": 
                    mimeType = "application/x-abiword"; 
                    break;
                case ".apng": 
                    mimeType = "image/apng"; 
                    break;
                case ".arc": 
                    mimeType = "application/x-freearc"; 
                    break;
                case ".avif": 
                    mimeType = "image/avif"; 
                    break;
                case ".avi": 
                    mimeType = "video/x-msvideo"; 
                    break;
                case ".azw": 
                    mimeType = "application/vnd.amazon.ebook"; 
                    break;
                case ".bin": 
                    mimeType = "application/octet-stream"; 
                    break;
                case ".bmp": 
                    mimeType = "image/bmp"; 
                    break;
                case ".bz": 
                    mimeType = "application/x-bzip"; 
                    break;
                case ".bz2": 
                    mimeType = "application/x-bzip2"; 
                    break;
                case ".cda": 
                    mimeType = "application/x-cdf"; 
                    break;
                case ".csh": 
                    mimeType = "application/x-csh"; 
                    break;
                case ".css": 
                    mimeType = "text/css"; 
                    break;
                case ".csv": 
                    mimeType = "text/csv"; 
                    break;
                case ".doc": 
                    mimeType = "application/msword"; 
                    break;
                case ".docx": 
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; 
                    break;
                case ".eot": 
                    mimeType = "application/vnd.ms-fontobject"; 
                    break;
                case ".epub": 
                    mimeType = "application/epub+zip"; 
                    break;
                case ".gz": 
                    mimeType = "application/x-gzip."; 
                    break;
                case ".gif": 
                    mimeType = "image/gif"; 
                    break;
                case ".htm": 
                    mimeType = "text/html"; 
                    break;
                case ".html": 
                    mimeType = "text/html"; 
                    break;
                case ".ico": 
                    mimeType = "image/vnd.microsoft.icon"; 
                    break;
                case ".ics": 
                    mimeType = "text/calendar"; 
                    break;
                case ".jar": 
                    mimeType = "application/java-archive"; 
                    break;
                case ".jpg": 
                    mimeType = "image/jpeg"; 
                    break;
                case ".jpeg": 
                    mimeType = "image/jpeg"; 
                    break;
                case ".js": 
                    mimeType = "text/javascript"; 
                    break;
                case ".json": 
                    mimeType = "application/json"; 
                    break;
                case ".jsonld": 
                    mimeType = "application/ld+json"; 
                    break;
                case ".mid": 
                    mimeType = "audio/midi"; 
                    break;
                case ".midi": 
                    mimeType = "audio/x-midi"; 
                    break;
                case ".mjs": 
                    mimeType = "text/javascript";
                    break;
                case ".mp3": 
                    mimeType = "audio/mpeg"; 
                    break;
                case ".mp4": 
                    mimeType = "video/mp4"; 
                    break;
                case ".mpeg": 
                    mimeType = "video/mpeg"; 
                    break;
                case ".mpkg": 
                    mimeType = "application/vnd.apple.installer+xml"; 
                    break;
                case ".odp": 
                    mimeType = "application/vnd.oasis.opendocument.presentation"; 
                    break;
                case ".ods": 
                    mimeType = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case ".odt": 
                    mimeType = "application/vnd.oasis.opendocument.text"; 
                    break;
                case ".oga": 
                    mimeType = "audio/ogg"; 
                    break;
                case ".ogv": 
                    mimeType = "video/ogg"; 
                    break;
                case ".ogx": 
                    mimeType = "application/ogg"; 
                    break;
                case ".opus": 
                    mimeType = "audio/ogg"; 
                    break;
                case ".otf": 
                    mimeType = "font/otf"; 
                    break;
                case ".png": 
                    mimeType = "image/png"; 
                    break;
                case ".pdf": 
                    mimeType = "application/pdf"; 
                    break;
                case ".php": 
                    mimeType = "application/x-httpd-php"; 
                    break;
                case ".ppt": 
                    mimeType = "application/vnd.ms-powerpoint"; 
                    break;
                case ".pptx": 
                    mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; 
                    break;
                case ".rar": 
                    mimeType = "application/vnd.rar"; 
                    break;
                case ".rtf": 
                    mimeType = "application/rtf"; 
                    break;
                case ".sh": 
                    mimeType = "application/x-sh"; 
                    break;
                case ".svg": 
                    mimeType = "image/svg+xml"; 
                    break;
                case ".tar": 
                    mimeType = "application/x-tar"; 
                    break;
                case ".tif": 
                    mimeType = "image/tiff"; 
                    break;
                case ".tiff":
                    mimeType = "image/tiff"; 
                    break;
                case ".ts": 
                    mimeType = "video/mp2t"; 
                    break;
                case ".ttf": 
                    mimeType = "font/ttf"; 
                    break;
                case ".txt":
                    mimeType = "text/plain"; 
                    break;
                case ".vsd": 
                    mimeType = "application/vnd.visio"; 
                    break;
                case ".wav": 
                    mimeType = "audio/wav"; 
                    break;
                case ".weba": 
                    mimeType = "audio/webm"; 
                    break;
                case ".webm": 
                    mimeType = "video/webm"; 
                    break;
                case ".webp": 
                    mimeType = "image/webp"; 
                    break;
                case ".woff": 
                    mimeType = "font/woff"; 
                    break;
                case ".woff2": 
                    mimeType = "font/woff2"; 
                    break;
                case ".xhtml": 
                    mimeType = "application/xhtml+xml"; 
                    break;
                case ".xls": 
                    mimeType = "application/vnd.ms-excel"; 
                    break;
                case ".xlsx": 
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; 
                    break;
                case ".xml": 
                    mimeType = "application/xml"; 
                    break;
                case ".xul": 
                    mimeType = "application/vnd.mozilla.xul+xml"; 
                    break;
                case ".zip": 
                    mimeType = "application/zip"; 
                    break;
                case ".7z": 
                    mimeType = "application/x-7z-compressed"; 
                    break;
            }

            return mimeType;
        }

        /// <summary>
        /// Gets the file extension
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>string extension</returns>
        private static string GetFileExtension(string? fileName)
        {
            string fileExtenion = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.Contains("."))
                {
                    string[] strings = fileName.Split('.');
                    fileExtenion = "." + strings[1];
                }
            }

            return fileExtenion;
        }
    }
}
