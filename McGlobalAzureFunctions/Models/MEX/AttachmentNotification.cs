// <copyright file="AttachmentNotification.cs" company ="TTT Moneycorp Ltd">
// Copyright 2025 moneycorp
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System;

    public class AttachmentNotification
    {
        public int ThereforeId { get; set; }

        public Guid ClientCRMGuid { get; set; }

        public Guid ContactCRMGuid { get; set; }

        public string? Filename { get; set; }

        public string? Filesize { get; set; }

        public string? FileDescription { get; set; }

        public AttachmentType TypeAttachment { get; set; }
        
        public bool IsPaymentPhotoId { get; set; }

        public byte[]? Attachment { get; set; }
    }
}
