// <copyright file="AttachmentRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AttachmentRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AttachmentRequest
    {
        public int ThereforeId { get; set; }
        public string? EntityName { get; set; }
        public Guid EntityId { get; set; }
        public string? Filename { get; set; }
        public string? Filesize { get; set; }
        public string? FileDescription { get; set; }
        public AttachmentType TypeAttachment { get; set; }
        public bool IsPaymentPhotoId { get; set; }
        public byte[]? Attachment { get; set; }
        public FileSource Source { get; set; }
        public string? ThereforeType { get; set; }
        public string? RequestOrigin { get; set; }
    }
}