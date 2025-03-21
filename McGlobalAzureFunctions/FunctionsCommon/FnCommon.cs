// <copyright file="FnCommon.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.FunctionsCommon
{
    using Azure.Messaging.ServiceBus;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Xrm.Sdk;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// Common Methods shared by functions.
    /// </summary>
    public class FnCommon
    {
        /// <summary>
        /// Gets the Sp Credentials to authenticate SharePoint.
        /// </summary>
        /// <returns>SpCredentials</returns>
        public static SpCredentials GetSpCredentials()
        {
            SpCredentials creds = new SpCredentials();
            creds.TenantId = SharePointConstants.SpTenantId;
            string? clientId = Environment.GetEnvironmentVariable(SharePointConstants.SharePointAppClientId, EnvironmentVariableTarget.Process);
            string? clientSecret = Environment.GetEnvironmentVariable(SharePointConstants.SharePointAppSecret, EnvironmentVariableTarget.Process);
            string? spSiteName = Environment.GetEnvironmentVariable(SharePointConstants.SharePointSiteName, EnvironmentVariableTarget.Process);

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(spSiteName))
            {
                creds.ClientId = clientId;
                creds.ClientSecret = clientSecret;
                creds.SpSiteName = spSiteName;
            }

            return creds;
        }

        /// <summary>
        /// Reads the Service Bus Message properties and validates the input message.
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>AsbMessageInfo</returns>
        public static AsbMessageInfo GetMessageProperties(ServiceBusReceivedMessage message)
        {
            AsbMessageInfo msgData = new AsbMessageInfo();
            string requestId = string.Empty;
            string recordId = string.Empty;
            string errMsg = string.Empty;

            try
            {
                msgData.MessageType = GetAsbMessageProperty(message.ApplicationProperties, FnConstants.MessageType);
                requestId = GetAsbMessageProperty(message.ApplicationProperties, FnConstants.RequestId);
                recordId = GetAsbMessageProperty(message.ApplicationProperties, FnConstants.RecordId);

                if (!string.IsNullOrEmpty(recordId))
                {
                    Guid recId = Guid.NewGuid();
                    bool isRecGuid = Guid.TryParse(requestId, out recId);

                    if (isRecGuid)
                    {
                        msgData.RecordId = recId;
                    }
                }

                Guid reqId = Guid.NewGuid();

                bool isReqGuid = Guid.TryParse(requestId, out reqId);

                if (isReqGuid)
                {
                    msgData.RequestId = reqId;

                    bool isValidMessage = IsMessageValid(msgData.MessageType, out errMsg);

                    if (isValidMessage && message.Body != null)
                    {
                        msgData.CanProcess = true;
                    }
                    else
                    {
                        msgData.ErrorMessage = "Message type is not valid";
                    }
                }
            }
            catch (Exception ex)
            {
                msgData.ErrorMessage = "Unable to read Message Properties:" + ex.Message;
            }

            return msgData;
        }

        /// <summary>
        /// Converts ASB message to Meaningful object.
        /// </summary>
        /// <typeparam name="T">Type of Message</typeparam>
        /// <param name="message">Azure Message</param>
        /// <param name="messageActions">Message actions</param>
        /// <param name="canProcess">out bool</param>
        /// <returns>Converted Object</returns>
        /// <exception cref="Exception">exception</exception>
        public static T DeSerializeServiceBusMessage<T>(ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, out bool canProcess)
        {
            canProcess = false;

            if (message.Body != null)
            {
                T? respCheck = message.Body.ToObjectFromJson<T>();

                if (respCheck != null)
                {
                    canProcess = true;
                    return respCheck;
                }
            }

            throw new Exception("Cannot convert json data");
        }

        /// <summary>
        /// Read Message Properties.
        /// </summary>
        /// <param name="userProperties">userProperties</param>
        /// <param name="keyName">keyName</param>
        /// <returns>string</returns>
        public static string GetAsbMessageProperty(IReadOnlyDictionary<string, object> userProperties, string keyName)
        {
            string messageType = string.Empty;
            string? retValue;

            if (userProperties.Count > 0)
            {
                foreach (var userProperty in userProperties)
                {
                    string keyItem = userProperty.Key.ToString();

                    if (userProperty.Value != null)
                    {
                        if (keyItem == keyName)
                        {
                            retValue = userProperty.Value.ToString();

                            if (retValue != null)
                            {
                                messageType = retValue;
                                break;
                            }
                        }
                    }
                }
            }

            return messageType;
        }

        /// <summary>
        /// Send the message to ASB for Gateway Service to pickup.
        /// </summary>
        /// <param name="messageContent">messageContent</param>
        /// <param name="messageTypeName">messageTypeName</param>
        /// <returns>DynTaskResponse</returns>
        public static DynTaskResponse SendMessageASB(string messageContent, string messageTypeName)
        {
            DynTaskResponse task = new DynTaskResponse();
            task.IsSuccess = false;

            Protect.ForNullOrWhiteSpace(messageContent, nameof(messageContent));
            Protect.ForNullOrWhiteSpace(messageTypeName, nameof(messageTypeName));

            try
            {
                ServiceBusClient client;

                string? endPoint = Environment.GetEnvironmentVariable(FnConstants.D365tomexpublishing, EnvironmentVariableTarget.Process);

                if (endPoint != null)
                {
                    client = new ServiceBusClient(endPoint);
                    ServiceBusSender sender = client.CreateSender(FnConstants.D365tomexpublishing, new ServiceBusSenderOptions());
                    ServiceBusMessage msg = new ServiceBusMessage(messageContent);
                    msg.ApplicationProperties.Add(FnConstants.MessageType, messageTypeName);
                    Task sendtask = sender.SendMessageAsync(msg);
                    sendtask.Wait();
                    Task closeTask = sender.CloseAsync();
                    closeTask.Wait();
                    sender.DisposeAsync();
                    task.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                task.Exception = ex;
                task.ErrorMessage = "Failed to send message to Service Bus:" + ex.Message;
            }

            return task;
        }

        /// <summary>
        /// Check if target is PROD.
        /// </summary>
        /// <returns>returns bool</returns>
        public static bool GetIsProd()
        {
            string? d365ConnString = Environment.GetEnvironmentVariable(FnConstants.D365ConnectionString, EnvironmentVariableTarget.Process);

            bool isProd = false;

            if (!string.IsNullOrEmpty(d365ConnString))
            {
                if (d365ConnString.Contains(FnConstants.Produrl))
                {
                    isProd = true;
                }
            }

            return isProd;
        }

        /// <summary>
        /// Gets Entity object from json String.
        /// </summary>
        /// <param name="jsonString">json string</param>
        /// <returns>Entity object</returns>
        public static Entity GetEntityFromRequest(string jsonString)
        {
            RemoteExecutionContext context = new RemoteExecutionContext();

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RemoteExecutionContext));
                var obj = serializer.ReadObject(ms);
                if (obj != null)
                {
                    context = (RemoteExecutionContext)obj;
                }
            }

            Entity targetEntity = new Entity();

            if (context != null && context.InputParameters.Contains("Target"))
            {
                targetEntity = (Entity)context.InputParameters["Target"];
            }

            return targetEntity;
        }

        /// <summary>
        /// Gets Entity object from json String.
        /// </summary>
        /// <param name="jsonString">json string</param>
        /// <returns>Entity object</returns>
        public static EntityReference GetEntityReferenceFromRequest(string jsonString)
        {
            RemoteExecutionContext context = new RemoteExecutionContext();

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RemoteExecutionContext));
                var obj = serializer.ReadObject(ms);
                if (obj != null)
                {
                    context = (RemoteExecutionContext)obj;
                }
            }

            EntityReference targetEntityRef = new();

            if (context != null && context.InputParameters.Contains("Target"))
            {
                targetEntityRef = (EntityReference)context.InputParameters["Target"];
            }

            return targetEntityRef;
        }


        /// <summary>
        /// Gets the entity object from request string.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="msgName"></param>
        /// <returns>Entity object</returns>
        public static Entity GetEntityFromRequestWithMessageType(string jsonString, out string msgName)
        {
            msgName = string.Empty;
            RemoteExecutionContext context = new RemoteExecutionContext();

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RemoteExecutionContext));
                var obj = serializer.ReadObject(ms);

                if (obj != null)
                {
                    context = (RemoteExecutionContext)obj;
                }
            }

            msgName = context.MessageName.ToLower();
            Entity targetEntity = new Entity();

            if (context != null && context.InputParameters.Contains("Target"))
            {
                targetEntity = (Entity)context.InputParameters["Target"];
            }

            return targetEntity;
        }

        /// <summary>
        /// Gets the remote execution context from function request.
        /// </summary>
        /// <param name="jsonString">Json Request</param>
        /// <returns>Remote execution context</returns>
        public static RemoteExecutionContext GetRemoteExecContext(string jsonString)
        {
            RemoteExecutionContext context = new RemoteExecutionContext();

            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RemoteExecutionContext));
                var obj = serializer.ReadObject(ms);
                if (obj != null)
                {
                    context = (RemoteExecutionContext)obj;
                }
            }

            return context;
        }

        /// <summary>
        /// Check if the message in ASB is valid for processing.
        /// </summary>
        /// <param name="messageType">Message type from properties</param>
        /// <param name="errMsg">String Error Msg</param>
        /// <returns>bool isvalid</returns>
        public static bool IsMessageValid(string messageType, out string errMsg)
        {
            errMsg = string.Empty;

            bool isValid = true;

            if (string.IsNullOrEmpty(messageType))
            {
                isValid = false;
                errMsg = "Message type is null";
            }

            return isValid;
        }
    }
}