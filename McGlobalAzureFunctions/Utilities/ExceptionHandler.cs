// <copyright file="ExceptionHandler.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    using McGlobalAzureFunctions.Models.Common;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// ExceptionHandler class
    /// </summary>
    public class ExceptionHandler
    {
        /// <summary>
        /// Gets the formatted exception
        /// from OrganizationServiceFault
        /// </summary>
        /// <param name="ex">OrganizationServiceFault</param>
        /// <returns>CrmException</returns>
        public static CrmException GetFormattedOrganizationServiceFault(FaultException<OrganizationServiceFault> ex)
        {
            CrmException crmEx = new CrmException();
            crmEx.Message = "The application terminated with an error.";
            crmEx.TimeStamp = ex.Detail.Timestamp;
            crmEx.ErrorCode = ex.Detail.ErrorCode;
            crmEx.MessageDetail = ex.Detail.Message;
            crmEx.TraceText = ex.Detail.TraceText;

            if (ex.Detail.InnerFault == null)
            {
                crmEx.InnerFault = "No Inner Fault";
            }
            else
            {
                crmEx.InnerFault = "Has Inner Fault";
            }

            return crmEx;
        }

        /// <summary>
        /// Gets the formatted exception
        /// from TimeoutException
        /// </summary>
        /// <param name="ex">TimeoutException</param>
        /// <returns>CrmException</returns>
        public static CrmException GetFormattedTimeoutException(System.TimeoutException ex)
        {
            CrmException crmEx = new CrmException();
            crmEx.Message = "The application timed out.";
            crmEx.MessageDetail = ex.Message;

            if (ex.StackTrace != null)
            {
                crmEx.TraceText = ex.StackTrace;
            }

            if (ex.InnerException != null)
            {
                if (ex.InnerException.Message == null)
                {
                    crmEx.InnerFault = "No Inner Fault";
                }
                else
                {
                    crmEx.InnerFault = ex.InnerException.Message;
                }
            }

            return crmEx;
        }

        /// <summary>
        /// Gets the formatted exception
        /// from System Exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>CrmException</returns>
        public static CrmException GetFormattedSystemException(System.Exception ex, string errorMessage)
        {
            CrmException crmEx = new CrmException();
            crmEx.Message = errorMessage;
            crmEx.MessageDetail = ex.Message;

            if (ex.StackTrace != null)
            {
                crmEx.TraceText = ex.StackTrace;
            }

            if (ex.InnerException != null)
            {
                crmEx.InnerFault = ex.InnerException.Message;
                FaultException<OrganizationServiceFault> fe = (FaultException<OrganizationServiceFault>)ex.InnerException;

                if (fe != null)
                {
                    crmEx.TimeStamp = fe.Detail.Timestamp;
                    crmEx.ErrorCode = fe.Detail.ErrorCode;
                    crmEx.MessageDetail = fe.Detail.Message;
                    crmEx.TraceText = fe.Detail.TraceText;

                    if (fe.Detail.InnerFault == null)
                    {
                        crmEx.InnerFault = "No Inner Fault";
                    }
                    else
                    {
                        crmEx.InnerFault = "Has Inner Fault";
                    }
                }
                else
                {
                    crmEx.InnerFault = "No Inner Fault";
                }
            }

            return crmEx;
        }
    }
}
