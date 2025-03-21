// <copyright file="CrmException.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class CrmException
    {
        private string traceText = string.Empty;
        private string errorMessage = string.Empty;
        private string messageDetail = string.Empty;
        private string innerFault = string.Empty;
        private string threadPluginName = string.Empty;
        private int errorCode = 0;


        /// <summary>
        /// Gets or sets value of the ErrorMessage.
        /// </summary>
        public string Message
        {
            get
            {
                if (errorMessage == null)
                {
                    errorMessage = string.Empty;
                }

                return errorMessage;
            }

            set
            {
                this.errorMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the TimeStamp.
        /// </summary>
        public DateTime TimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value of the ErrorCode.
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                this.errorCode = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the MessageDetail.
        /// </summary>
        public string MessageDetail
        {
            get
            {
                if (messageDetail == null)
                {
                    messageDetail = string.Empty;
                }

                return messageDetail;
            }

            set
            {
                this.messageDetail = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the TraceText.
        /// </summary>
        public string TraceText
        {
            get
            {
                if (traceText == null)
                {
                    traceText = string.Empty;
                }

                return traceText;
            }

            set
            {
                this.traceText = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the InnerFault.
        /// </summary>
        public string InnerFault
        {
            get
            {
                if (innerFault == null)
                {
                    innerFault = string.Empty;
                }

                return innerFault;
            }
            set
            {
                this.innerFault = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the ThreadPluginName.
        /// </summary>
        public string FunctionName
        {
            get
            {
                if (threadPluginName == null)
                {
                    threadPluginName = string.Empty;
                }

                return threadPluginName;
            }
            set
            {
                this.threadPluginName = value;
            }
        }
    }
}
