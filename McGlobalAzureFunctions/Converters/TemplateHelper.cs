// <copyright file="TemplateHelper.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Utilities;
    using McGlobalAzureFunctions.DAL;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// TemplateHelper.
    /// </summary>
    public class TemplateHelper
    {
        /// <summary>
        /// GetEmailBody.
        /// </summary>
        /// <param name="contextEntity">contextRecordId</param>
        /// <param name="crmService">crmService</param>
        /// <param name="rawContent">entityName</param>
        /// <returns>DynTaskResponse</returns>
        public static DynTaskResponse GetEmailBody(Entity contextEntity, IOrganizationServiceAsync2 crmService, string rawContent, string subject)
        {
            DynTaskResponse response = new DynTaskResponse();
            bool? isSuccess = null;
            string emailContent = string.Empty;
            TemplateItem templateData = new TemplateItem();

            try
            {
                List<string> schemaNames = GetAttributesUsedInTemplate(rawContent);

                if (schemaNames.Count > 0)
                {
                    List<string> columnsToGet = ColumnsToGet(schemaNames);
                    ColumnSet entityCols = new ColumnSet(columnsToGet.ToArray());
                    string toEmail = string.Empty;
                    List<CrmAttributeData> attributeInfo = GetContextEntityData(contextEntity, out toEmail);
                    List<CrmAttributeData> contentData = GetReplacedTokenText(attributeInfo, schemaNames, crmService);
                    List<string> conditionalColumns = GetConditionalColumns(schemaNames);

                    if (conditionalColumns.Count > 0)
                    {
                        List<CrmAttributeData> conditionalAttributes = GetSchemaNamesWithPipeSymbol(attributeInfo, conditionalColumns);
                        contentData.AddRange(conditionalAttributes);
                    }

                    foreach (CrmAttributeData data in contentData)
                    {
                        if (data.HtmlKeyWord == "mcorp_opportunitysellcurrencyid.name")
                        {
                            response.SellCurrency = data.HtmlKeyWordContent;
                        }

                        if (data.HtmlKeyWord == "mcorp_opportunitybuycurrencyid.name")
                        {
                            response.BuyCurrency = data.HtmlKeyWordContent;
                        }

                        if (data.HtmlKeyWord == "mcorp_stoprate!mcorp_limitrate")
                        {
                            response.TargetRate = data.HtmlKeyWordContent;
                        }

                        if (string.IsNullOrEmpty(data.HtmlKeyWordContent))
                        {
                            if (data.HtmlKeyWord != "ownerid.title")
                            {
                                isSuccess = false;
                                response.ErrorMessage = "The record is missing the below data used by the template.\n";
                                break;
                            }
                        }
                    }

                    foreach (CrmAttributeData data in contentData)
                    {
                        if (!string.IsNullOrEmpty(data.HtmlKeyWordContent))
                        {
                            rawContent = rawContent.Replace("[" + data.HtmlKeyWord + "]", data.HtmlKeyWordContent);
                        }
                        else
                        {
                            if (data.HtmlKeyWord != "ownerid.title")
                            {
                                response.ErrorMessage += data.HtmlKeyWord + "\n";
                            }
                        }
                    }

                    string htmlFrame = GetTemplateBackgroundRateTrackerCorpNotification();
                    string fullContent = htmlFrame.Replace("[CONTENT]", rawContent);

                    if (fullContent.Contains("{CURRENTDATETIME}"))
                    {
                        fullContent = fullContent.Replace("{CURRENTDATETIME}", DateTime.Now.ToString());
                    }

                    if (fullContent.Contains("[leadid]") && contextEntity.LogicalName.ToLower() == "lead")
                    {
                        fullContent = fullContent.Replace("[leadid]", contextEntity.Id.ToString());
                    }

                    templateData.ContentHtml = fullContent;
                    templateData.TemplateSubject = subject;

                    if (!isSuccess.HasValue)
                    {
                        isSuccess = true;
                        response.TemplateData = templateData;
                        response.IsSuccess = true;
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "GetEmailBody failed:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "GetEmailBody failed:" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            return response;
        }

        
        private static List<CrmAttributeData> GetReplacedTokenText(List<CrmAttributeData> attributeInfo, List<string> schemaNames, IOrganizationServiceAsync2 service)
        {
            List<CrmAttributeData> contentData = new();

            if (schemaNames.Count <= 0)
            {
                return contentData;
            }

            List<CrmAttributeData> contentData0 = GetSchemaNamesWithNoDots(attributeInfo, schemaNames);
            contentData.AddRange(contentData0);
            List<CrmAttributeData> contentData1 = GetSchemaNamesWithDots(attributeInfo, schemaNames, service);
            contentData.AddRange(contentData1);

            return contentData;
        }

        private static List<CrmAttributeData> GetSchemaNamesWithNoDots(List<CrmAttributeData> attributeInfo, List<string> schemaNames)
        {
            List<CrmAttributeData> contentData = new List<CrmAttributeData>();

            string owningBusinessValue = "";

            var busUnit = attributeInfo.Where(x => x.AttributeName == "mcorp_owningbusiness").ToList<CrmAttributeData>();
            if (busUnit.Count > 0)
            {
                owningBusinessValue = busUnit[0].AttributeValue;
            }
            else
            { owningBusinessValue = ""; }

            foreach (string schemaName in schemaNames)
            {
                CrmAttributeData returnInfo = new CrmAttributeData();
                returnInfo.HtmlKeyWord = schemaName;
                returnInfo.IsConditional = false;

                if (!schemaName.Contains(".") && !schemaName.Contains("!"))
                {
                    var attrInfo = attributeInfo.Where(x => x.AttributeName == schemaName).ToList<CrmAttributeData>();

                    if (attrInfo.Count > 0)
                    {
                        if (attrInfo[0].AttributeType == "DateTime" && owningBusinessValue != "" && owningBusinessValue == "100000018")//Moneycorp US Inc date format as MM-dd-YYYY (100000018)
                        {
                            returnInfo.HtmlKeyWordContent = Convert.ToDateTime(attrInfo[0].AttributeValue).ToUniversalTime().ToString("MM-dd-yyyy");
                        }
                        else
                        {
                            returnInfo.HtmlKeyWordContent = attrInfo[0].AttributeValue;
                        }
                    }

                    contentData.Add(returnInfo);
                }
            }

            return contentData;
        }

        private static List<CrmAttributeData> GetSchemaNamesWithPipeSymbol(List<CrmAttributeData> attributeInfo, List<string> schemaNames)
        {
            List<CrmAttributeData> contentData = new List<CrmAttributeData>();

            string owningBusinessValue = "";

            var busUnit = attributeInfo.Where(x => x.AttributeName == "mcorp_owningbusiness").ToList<CrmAttributeData>();
            if (busUnit.Count > 0)
            {
                owningBusinessValue = busUnit[0].AttributeValue;
            }
            else
            { owningBusinessValue = ""; }

            foreach (string schemaName in schemaNames)
            {
                CrmAttributeData returnInfo = new CrmAttributeData();
               
                returnInfo.IsConditional = true;

                if (schemaName.Contains("!"))
                {
                    string[] attrAry = schemaName.Split('!');

                    string attr1 = attrAry[0];
                    string attr2 = attrAry[1];

                    var attrInfo1 = attributeInfo.Where(x => x.AttributeName == attr1).ToList<CrmAttributeData>();
                    var attrInfo2 = attributeInfo.Where(x => x.AttributeName == attr2).ToList<CrmAttributeData>();

                    if (attrInfo1.Count > 0)
                    {
                        returnInfo.HtmlKeyWord = schemaName;
                        returnInfo.HtmlKeyWordContent = attrInfo1[0].AttributeValue;
                    }

                    if (attrInfo2.Count > 0)
                    {
                        returnInfo.HtmlKeyWord = schemaName;
                        returnInfo.HtmlKeyWordContent = attrInfo2[0].AttributeValue;
                    }

                    contentData.Add(returnInfo);
                }
            }

            return contentData;
        }

        private static List<CrmAttributeData> GetSchemaNamesWithDots(List<CrmAttributeData> attributeInfo, List<string> schemaNames, IOrganizationServiceAsync2 service)
        {
            List<CrmAttributeData> contentData = new List<CrmAttributeData>();
            List<string> multiAttributes = new List<string>();
            // Dictionary<string, int> multiAttributes = new Dictionary<string, int>();
            string owningBusinessValue = "";

            var busUnit = attributeInfo.Where(x => x.AttributeName == "mcorp_owningbusiness").ToList<CrmAttributeData>();
            owningBusinessValue = busUnit.Count > 0 ? busUnit[0].AttributeValue : "";

            foreach (string schemaName in schemaNames)
            {
                CrmAttributeData returnInfo = new CrmAttributeData();
                returnInfo.HtmlKeyWord = schemaName;

                if (schemaName.Contains("."))
                {
                    int firstIndex = schemaName.IndexOf('.');
                    string linkingAttributeName = schemaName.Substring(0, firstIndex);
                    string linkedAttributeName = schemaName.Substring(firstIndex + 1);

                    var attrInfo = attributeInfo.Where(x => x.AttributeName == linkingAttributeName).ToList<CrmAttributeData>();
                    if (attrInfo.Count > 0)
                    {
                        if (linkedAttributeName.Contains("."))
                        {
                            string[] dsv = linkedAttributeName.Split(new char[] { '.' });
                            string extendedAttribute = dsv[1];
                            string extendedAttributeLinkingAttributeName = dsv[0];

                            if (extendedAttribute == "id")
                            {
                                var subAttrInfo = attributeInfo.Where(x => x.AttributeName == schemaName).ToList<CrmAttributeData>();

                                if (attrInfo.Count > 0)
                                {
                                    returnInfo.HtmlKeyWordContent = subAttrInfo[0].AttributeValue;
                                }
                            }
                            else
                            {
                                // get entity based on first attribute name
                                Entity entity = service.Retrieve(attrInfo[0].AttributeLogicalName, Guid.Parse(attrInfo[0].AttributeValue), new ColumnSet(new string[] { extendedAttributeLinkingAttributeName }));
                                if (entity.Attributes.Contains(extendedAttributeLinkingAttributeName))
                                {
                                    object value = new object();
                                    bool isAttrib = entity.Attributes.TryGetValue(extendedAttributeLinkingAttributeName, out value);

                                    if (isAttrib)
                                    {
                                        if (value != null)
                                        {
                                            if (value.GetType().Name == "String")
                                            {
                                                returnInfo.HtmlKeyWordContent = value.ToString();
                                            }

                                            if (value.GetType().Name == "DateTime")
                                            {
                                                if (owningBusinessValue != "" && owningBusinessValue == "100000018" && value != null)//Moneycorp US Inc date format as MM-dd-YYYY (100000018)
                                                {
                                                    returnInfo.HtmlKeyWordContent = Convert.ToDateTime(value.ToString()).ToUniversalTime().ToString("MM-dd-yyyy");
                                                }
                                                else
                                                {
                                                    returnInfo.HtmlKeyWordContent = value.ToString();
                                                }
                                            }

                                            if (value.GetType().Name == "EntityReference")
                                            {
                                                string lkpEntityName = entity.GetAttributeValue<EntityReference>(extendedAttributeLinkingAttributeName).LogicalName;
                                                Guid lkpId = entity.GetAttributeValue<EntityReference>(extendedAttributeLinkingAttributeName).Id;
                                                Entity linkedEntity = service.Retrieve(lkpEntityName, lkpId, new ColumnSet(new string[] { extendedAttribute }));
                                                List<CrmAttributeData> linkedAttributeInfo = GetEntityAttributeData(linkedEntity, lkpEntityName);

                                                var lkpAttrInfo = linkedAttributeInfo.Where(x => x.AttributeName == extendedAttribute).ToList<CrmAttributeData>();

                                                if (lkpAttrInfo.Count > 0)
                                                {
                                                    CrmAttributeData dt = lkpAttrInfo[0];

                                                    if (dt != null)
                                                    {
                                                        if (dt.AttributeType == "String")
                                                        {
                                                            returnInfo.HtmlKeyWordContent = linkedEntity.GetAttributeValue<string>(extendedAttribute);
                                                        }
                                                        if (dt.AttributeType == "DateTime")
                                                        {
                                                            if (owningBusinessValue != "" && owningBusinessValue == "100000018" && dt != null)//Moneycorp US Inc date format as MM-dd-YYYY (100000018)
                                                            {
                                                                returnInfo.HtmlKeyWordContent = Convert.ToDateTime(dt.AttributeValue.ToString()).ToUniversalTime().ToString("MM-dd-yyyy");
                                                            }
                                                            else
                                                            {
                                                                returnInfo.HtmlKeyWordContent = dt.AttributeValue.ToString();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (linkedAttributeName == "id")
                            {
                                returnInfo.HtmlKeyWordContent = attrInfo[0].AttributeValue;
                            }
                            else if (linkedAttributeName == "name")
                            {
                                returnInfo.HtmlKeyWordContent = attrInfo[0].AttributeText;
                            }
                            else
                            {
                                // Get Data from CRM
                                Entity entity = service.Retrieve(attrInfo[0].AttributeLogicalName, Guid.Parse(attrInfo[0].AttributeValue), new ColumnSet(new string[] { linkedAttributeName }));

                                if (entity.Attributes.Contains(linkedAttributeName))
                                {
                                    object value = new object();
                                    bool isAttrib = entity.Attributes.TryGetValue(linkedAttributeName, out value);

                                    if (value.GetType().Name == "String")
                                    {
                                        returnInfo.HtmlKeyWordContent = value.ToString();
                                    }

                                    if (value.GetType().Name == "DateTime")
                                    {
                                        if (owningBusinessValue != "" && owningBusinessValue == "100000018" && value != null)//Moneycorp US Inc date format as MM-dd-YYYY (100000018)
                                        {
                                            returnInfo.HtmlKeyWordContent = Convert.ToDateTime(value.ToString()).ToUniversalTime().ToString("MM-dd-yyyy");
                                        }
                                        else
                                        {
                                            returnInfo.HtmlKeyWordContent = value.ToString();
                                        }
                                    }
                                    if (value.GetType().Name == "EntityReference")
                                    {
                                        returnInfo.HtmlKeyWordContent = entity.GetAttributeValue<EntityReference>(linkedAttributeName).Name;
                                    }
                                }

                            }
                        }
                    }

                    contentData.Add(returnInfo);
                }
            }

            return contentData;
        }

        private static List<CrmAttributeData> GetEntityAttributeData(Entity contextEntity, string entityName)
        {
            List<CrmAttributeData> keyValueList = new List<CrmAttributeData>();

            AttributeCollection allFields = contextEntity.Attributes;
            ICollection<string> keys = allFields.Keys;
            ICollection<object> values = allFields.Values;

            foreach (string field in allFields.Keys)
            {
                object val = new object();
                bool isval = allFields.TryGetValue(field, out val);
                string attrValue = string.Empty;
                CrmAttributeData info = new CrmAttributeData();

                Type attrType = val.GetType();
                string typeName = attrType.ToString();

                if (typeName.Contains("."))
                {
                    string[] fullTypename = typeName.Split(new char[] { '.' });
                    int count = fullTypename.Length - 1;
                    info.AttributeType = fullTypename[count];
                }
                else
                {
                    info.AttributeType = typeName;
                }

                if (attrType == typeof(OptionSetValue))
                {
                    attrValue = ((OptionSetValue)val).Value.ToString();
                    if (contextEntity.FormattedValues.ContainsKey(field))
                    {
                        info.AttributeText = contextEntity.FormattedValues[field];
                    }

                }
                else if (attrType == typeof(EntityReference))
                {
                    attrValue = ((EntityReference)val).Id.ToString();
                    info.AttributeLogicalName = ((EntityReference)val).LogicalName.ToString();
                    info.AttributeText = ((EntityReference)val).Name;
                }
                else if (attrType == typeof(Money))
                {
                    attrValue = ((Money)val).Value.ToString();
                }
                else
                {
                    attrValue = val.ToString();
                }

                info.AttributeValue = attrValue;
                info.AttributeName = field;

                keyValueList.Add(info);
            }

            return keyValueList;
        }

        private static List<string> GetConditionalColumns(List<string> schemaNames)
        {
            //filter column names for data retrieve
            List<string> columnsToGet = new List<string>();

            foreach (string cName in schemaNames)
            {
                string schemaName1 = string.Empty;

                if (cName.Contains("!"))
                {
                    schemaName1 = cName;

                    if (!columnsToGet.Contains(schemaName1))
                    {
                        columnsToGet.Add(schemaName1);
                    }
                }
            }

            return columnsToGet;
        }

        private static List<string> ColumnsToGet(List<string> schemaNames)
        {
            //filter column names for data retrieve
            List<string> columnsToGet = new List<string>();

            foreach (string cName in schemaNames)
            {
                string schemaName1 = string.Empty;
                string schemaName2 = string.Empty;

                if (cName.Contains("."))
                {
                    schemaName1 = cName.Split(new char[] { '.' })[0];
                }
                else if (cName.Contains("!"))
                {
                    schemaName1 = cName.Split(new char[] { '!' })[0];
                    schemaName2 = cName.Split(new char[] { '!' })[1];
                }
                else
                {
                    schemaName1 = cName;
                }

                if (!columnsToGet.Contains(schemaName1))
                {
                    columnsToGet.Add(schemaName1);
                }

                if (!string.IsNullOrEmpty(schemaName2))
                {
                    if (!columnsToGet.Contains(schemaName2))
                    {
                        columnsToGet.Add(schemaName2);
                    }
                }
            }

            return columnsToGet;
        }

        /// <summary>
        /// Gets the attributes used in templates
        /// </summary>
        /// <param name="templateContent">templateContent</param>
        /// <returns>List string</returns>
        private static List<string> GetAttributesUsedInTemplate(string templateContent)
        {
            List<string> schemaNames = new List<string>();

            ICollection<string> matches =
                    Regex.Matches(templateContent.Replace(Environment.NewLine, ""), @"\[([^]]*)\]")
                    .Cast<Match>()
                    .Select(x => x.Groups[1].Value)
                    .ToList();

            if (matches.Count > 0)
            {
                foreach (string match in matches)
                {
                    if (match != "ADHOCTEXT" && match != "currentdatetime")
                    {
                        schemaNames.Add(match);
                    }
                }
            }

            return schemaNames;
        }

        /// <summary>
        /// GetContextEntityData.
        /// </summary>
        /// <param name="contextEntity">contextEntity</param>
        /// <param name="toEmailAddress">toEmailAddress</param>
        /// <returns>List of CrmAttributeData</returns>
        private static List<CrmAttributeData> GetContextEntityData(Entity contextEntity, out string toEmailAddress)
        {
            List<CrmAttributeData> keyValueList = new List<CrmAttributeData>();
            toEmailAddress = string.Empty;

            if (contextEntity.LogicalName == "account" || contextEntity.LogicalName == "contact" || contextEntity.LogicalName == "lead")
            {
                toEmailAddress = contextEntity.GetAttributeValue<string>("emailaddress1");
            }

            AttributeCollection allFields = contextEntity.Attributes;
            ICollection<string> keys = allFields.Keys;
            ICollection<object> values = allFields.Values;

            foreach (string field in allFields.Keys)
            {
                object val = new object();
                bool isval = allFields.TryGetValue(field, out val);
                string attrValue = string.Empty;
                CrmAttributeData info = new CrmAttributeData();

                Type attrType = val.GetType();
                string typeName = attrType.ToString();

                if (typeName.Contains("."))
                {
                    string[] fullTypename = typeName.Split(new char[] { '.' });
                    int count = fullTypename.Length - 1;
                    info.AttributeType = fullTypename[count];
                }
                else
                {
                    info.AttributeType = typeName;
                }

                if (attrType == typeof(OptionSetValue))
                {
                    attrValue = ((OptionSetValue)val).Value.ToString();
                    if (contextEntity.FormattedValues.ContainsKey(field))
                    {
                        info.AttributeText = contextEntity.FormattedValues[field];
                    }

                }
                else if (attrType == typeof(EntityReference))
                {
                    attrValue = ((EntityReference)val).Id.ToString();
                    info.AttributeLogicalName = ((EntityReference)val).LogicalName.ToString();
                    info.AttributeText = ((EntityReference)val).Name;
                }
                else if (attrType == typeof(Money))
                {
                    attrValue = ((Money)val).Value.ToString();
                }
                else
                {
                    attrValue = val.ToString();
                }

                info.AttributeValue = attrValue;
                info.AttributeName = field;

                keyValueList.Add(info);
            }

            return keyValueList;
        }

        /// <summary>
        /// GetTemplateContentRateTrackerCorpNotification.
        /// </summary>
        /// <returns>string</returns>
        public static string GetTemplateContentRateTrackerCorpNotification()
        {
            return @"<p style=""text-align:right;"">Client ID: [customerid.mcorp_fedsid]&nbsp;</p><p style=""text-align:left;"">Dear [mcorp_notificationcontactid.name],</p><p>The [mcorp_opportunitysellcurrencyid.name] to [mcorp_opportunitybuycurrencyid.name] exchange rate you requested us to track is fast approaching the&nbsp;[mcorp_stoprate!mcorp_limitrate] mark. </p><p>To find out the current rate and speak to your personal dealer about your foreign exchange requirements call +44 (0)20 7823 7800. </p><p>Alternatively,&nbsp;please <a href=""https://business.moneycorp.com/ratetracker/"" target=""_blank""><span style=""color:#ed1944;"">click here</span></a>&nbsp;if you would like to set up another rate alert with the moneycorp rate tracker.</p><p>Kind regards,</p><p>The moneycorp team</p>";
            //return @"<p style=""text-align:right;"">Client ID: [customerid.mcorp_fedsid]&nbsp;</p><p style=""text-align:left;"">Dear [mcorp_notificationcontactid.name],</p><p>The [mcorp_opportunitysellcurrencyid.name] to [mcorp_opportunitybuycurrencyid.name] exchange rate you requested us to track at [mcorp_stoprate!mcorp_limitrate] mark, has triggered on {CURRENTDATETIME}. </p><p>To find out the current rate and speak to your personal dealer about your foreign exchange requirements call +44 (0)20 7823 7800. </p><p>Alternatively,&nbsp;please <a href=""https://business.moneycorp.com/ratetracker/"" target=""_blank""><span style=""color:#ed1944;"">click here</span></a>&nbsp;if you would like to set up another rate alert with the moneycorp rate tracker.</p><p>Kind regards,</p><p>The moneycorp team</p>";
        }

        public static string GetRateTrackerSetupTemplate()
        {
            return @"<p sizcache01326407398199722=""829 65 7"" sizcache049320288781281285=""829 65 7"" sizset=""false"">Client Number: [customerid.mcorp_fedsid]</p><p sizcache004523589881340895=""829 65 9"" sizcache012370831929455739=""829 65 9"" sizcache01326407398199722=""829 65 7"" sizcache035224569108617703=""829 65 9"" sizcache04418306430231622=""829 65 9"" sizcache049320288781281285=""829 65 7"" sizcache05622169721543202=""829 65 9"" sizcache06236315321190639=""829 65 6"" sizcache06748497069197479=""829 65 8"" sizcache0790918396908722=""829 65 6"" sizcache08848859003475222=""829 65 9"" sizset=""false""><strong>Dear [mcorp_notificationcontactid.name] </strong><br /><br />Your rate tracker has been setup. We will automatically send you an email when the <strong>[mcorp_opportunitysellcurrencyid.name]</strong> to<strong> [mcorp_opportunitybuycurrencyid.name] </strong>rate reaches [mcorp_stoprate!mcorp_limitrate].<br /><br />If you would like to track another rate please <a ?="""" Index?ReturnUrl=""%2fRateTracker%2fCreate"" Login="""" href=""https://business.moneycorp.com/ratetracker"" originaloriginaloriginalhref="""" target=""_blank"">click here</a>. </p><p sizcache01326407398199722=""829 65 7"" sizcache049320288781281285=""829 65 7"" sizset=""false"">Alternatively, if you would like to speak to an expert about your International Payments requirements please call us on +44 (0)20 7823 7800.</p>";
        }

        /// <summary>
        /// GetTemplateBackgroundRateTrackerCorpNotification.
        /// </summary>
        /// <returns>string</returns>
        private static string GetTemplateBackgroundRateTrackerCorpNotification()
        {
            return @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
            <html>
            <head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
	            <title>Moneycorp</title>
	            <style type=""text/css"">.ReadMsgBody {
                        width: 100%;
                    }

                    .ExternalClass {
                        width: 100%;
                    }

                        .ExternalClass * {
                            line-height: 130%;
                            margin-bottom: 0;
                        }

                        .ExternalClass .lh-outlook-fix {
                            /* background-color: yellow;
                  line-height: 9px; */
                        }

            

                    hr {
                        height: 1px;
                        color: #bab4b8;
                        background: #bab4b8;
                        font-size: 0;
                        border: 0;
                        margin: 10px 0px 10px 0px;
                    }

		            .brand {
			            color: #ed1944;
		            }

                    p {
                        margin-bottom: 0;
                    }

                    table {
                        border-collapse: separate;
                    }

                    body {
                        font-family: Arial, Verdana,sans-serif;
                        margin-top: 10px;
                        margin-bottom: 30px;
                        color: #333333;
                        margin-left: 0;
                        margin-right: 0;
                        padding-bottom: 0;
                        padding-left: 0;
                        padding-right: 0;
                        padding-top: 0;
			            font-size: 12px;
                    }

                    table {
                        border-spacing: 0;
                    }

                    span.yshortcuts {
                        color: #000;
                        background-color: none;
                        border: none;
                    }

                        span.yshortcuts:hover,
                        span.yshortcuts:active,
                        span.yshortcuts:focus {
                            color: #000;
                            background-color: none;
                            border: none;
                        }

                    a img {
                        border: none;
                    }

                    .heading {
                        background: #756871;
                    }

                    .content {
                        background: #c7c0c4;
                    }
	            </style>
            </head>
            <body style=""margin-top: 0px; margin-bottom: 30px; color: #303030; font-family: Arial, Verdana,sans-serif; margin-left: 0; margin-right: 0; padding-bottom: 0; padding-left: 0; padding-right: 0; padding-top: 0; background: #FFFFFF"">
            <div class=""ExternalClass""><!--wrapper table-->
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style="""" width=""100%"">
	            <tbody>
		            <tr>
			            <td>
			            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0; border: 0; background: #fffff; padding-bottom: 0; padding-left: 0; padding-right: 0; padding-top: 0;"" width=""640px"">
				            <tbody>
					            <tr style=""background: #FFFFFF"">
						            <td style=""background: white"" width=""20"">&nbsp;</td>
						            <td style=""background: white"" valign=""top"" width=""400""><!--- header -->
						            <table>
							            <tbody>
								            <tr>
									            <td colspan=""1"" style=""padding-top: 0px; padding-bottom: 0px;"" valign=""top"" width="""">
									            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0; text-align: left; font-size: 12px; padding: 0; margin: 0"" width=""100%"">
										            <tbody>
											            <tr>
												            <td style=""padding-top: 0px; padding-bottom: 0px""><img alt=""Moneycorp - Logo"" name=""Cont_0"" src=""http://www.moneycorp.com/Global/Images/emails/travelmoney/moneycorp/logo.png"" style=""display: block;"" /></td>
											            </tr>
										            </tbody>
									            </table>
									            </td>
								            </tr>
							            </tbody>
						            </table>
						            <!--- end of header--><!--- shared -->

						            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0; text-align: left; padding: 0; margin: 0; vertical-align: top;"" width=""100%"">
							            <tbody>
								            <tr>
									            <td colspan=""1"" style=""padding-top: 0px; padding-bottom: 0px;"" valign=""top"" width="""">
									            <table>
										            <tbody>
											            <tr class=""spacer"">
												            <td style=""padding-top: 20px; padding-bottom: 10px"">&nbsp;</td>
											            </tr>
											            <tr>
												            <td valign=""top"">
												            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0; text-align: left; font-size: 14px; padding: 0; margin: 0"" width=""100%"">
													            <tbody>
														            <tr>
															            <td colspan=""2"" valign=""top"">[CONTENT]</td>
														            </tr>
													            </tbody>
												            </table>
												            </td>
											            </tr>
										            </tbody>
									            </table>
									            </td>
								            </tr>
							            </tbody>
						            </table>
						            <!-- end shared -->

						            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0px; text-align: left; padding: 0px; margin: 0px; width: 100%;"">
							            <tbody>
								            <tr style=""background:#ffffff"">
									            <td style=""padding-top:15px;"" valign=""top"">
									            <hr />
									            <p><span style=""font-size: 20px;""></span><span style=""font-size: 20px;"">Any questions?</span></p>
									            </td>
								            </tr>
								            <tr>
									            <td style=""padding-top:15px;"" valign=""top"">
									            <p style=""margin: 0px; padding: 5px 0px; color: #41333a; font-family: &quot;Arial&quot;, sans-serif; font-size: 0px; font-weight: normal;""><span style=""font-size: 13px;""><img height=""20px"" name=""Cont_1"" src=""https://www.moneycorp.com/Global/Images/emails/login-avatar.png"" width=""20px"" />&nbsp;&nbsp;<a href=""https://online.moneycorp.com"" name=""www_moneycorp_com_uk_contact_us__in"" xt=""SPCLICK""><span style=""color:#ed1944;"">Login</span></a></span></p>

									            <p style=""margin: 0px; padding: 5px 0px; color: #41333a; font-family: &quot;Arial&quot;, sans-serif; font-size: 0px; font-weight: normal;""><span style=""font-size: 13px;""><br />
									            <img height=""20px"" name=""Cont_2"" src=""https://www.moneycorp.com/Global/Images/emails/phone-call.png"" width=""20px"" />&nbsp; +44 (0)20 7823 7800</span></p>

									            <p style=""margin: 0px; padding: 0px 0px 5px; color: #41333a; font-family: &quot;Arial&quot;, sans-serif; font-size: 0px; font-weight: normal;""><span style=""font-size: 13px;""><strong><br />
									            Lines open:</strong><br />
									            Mon-Fri 07:30 am to 07:30 pm<br />
									            <br />
									            <strong>Are you overseas?</strong><br />
									            We also have offices in USA, France, Spain, Romania and Ireland.&nbsp;<a href=""https://www.moneycorp.com/uk/contact-us/#international"" name=""www_moneycorp_com_uk_contact_us__in_2"" xt=""SPCLICK""><span style=""color:#ed1944;"">Click here</span></a> for their contact details.<br />
									            &nbsp; &nbsp; &nbsp; &nbsp;<br />
									            <a href=""https://itunes.apple.com/gb/app/moneycorp/id969455113"" name=""Hyperlink_20170301_111831309"" target=""_blank"" xt=""SPCLICK""><img border=""0"" height=""50.336"" name=""Cont_0"" src=""https://www.moneycorp.com/Global/Images/emails/personal/Campaigns2016/Download_on_the_App_Store_Badge.svg.png"" width=""170"" /></a>&nbsp; &nbsp; &nbsp;&nbsp;</span><a href=""https://play.google.com/store/apps/details?id=com.moneycorp.remo3"" name=""Hyperlink_20170301_111859597"" target=""_blank"" xt=""SPCLICK""><img border=""0"" height=""52"" name=""Cont_1"" src=""https://www.moneycorp.com/Global/Images/emails/personal/Campaigns2016/google-play-badge.png"" style=""line-height: 0px;"" width=""170"" /></a><span style=""font-size: 13px;""></span></p>
									            </td>
								            </tr>
							            </tbody>
						            </table>
						            <!-- the end of got any questions panel-->

						            <table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-spacing: 0; text-align: left; padding: 0; margin: 0"" width=""100%"">
							            <tbody>
								            <tr>
									            <td style=""padding-top: 10px; padding-bottom: 5px;"">
									            <hr /></td>
								            </tr>
							            </tbody>
						            </table>
						            <!-- footer -->

						            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""640"">
							            <tbody>
								            <tr>
									            <td>&nbsp;</td>
								            </tr>
								            <tr>
									            <td>
									            <p style=""font-family: Arial, Verdana,sans-serif; font-size: 11px; color: #41333a; padding: 0; margin: 0;"">Copyright &copy; | 2021 | TTT Moneycorp Limited | All rights reserved.</p>
									            </td>
								            </tr>
								            <tr>
								            </tr>
								            <tr>
									            <td>
									            <p style=""font-family: Arial, Verdana,sans-serif; font-size: 11px; color: #41333a; padding: 0; margin: 0;"">None of the information contained in this email constitutes, nor should be construed as financial advice.</p>
									            </td>
								            </tr>
								            <tr>
								            </tr>
								            <tr>
									            <td>
									            <p style=""font-family: Arial, Verdana,sans-serif; font-size: 11px; color: #41333a; padding: 0; margin: 0;"">TTT Moneycorp Limited is a company registered in England under registration number 738837. Its registered office address is Floor 5, Zig Zag Building, 70 Victoria Street, London, SW1E 6SQ and its VAT registration number is 897 3934 54.</p>
									            </td>
								            </tr>
								            <tr>
								            </tr>
								            <tr>
									            <td>
									            <p style=""font-family: Arial, Verdana,sans-serif; font-size: 11px; color: #41333a; padding: 0; margin: 0;"">DISCLAIMER: Moneycorp disclaims any responsibility or liability for viruses contained within this email. It is therefore recommended that all emails should be scanned for viruses.</p>
									            </td>
								            </tr>
								            <tr>
								            </tr>
							            </tbody>
						            </table>
						            <!--/end footer--></td>
					            </tr>
				            </tbody>
			            </table>
			            <!--/wrapper table--></td>
		            </tr>
	            </tbody>
            </table>
            </div>
            </body>
            </html>";
        }
    }
}
