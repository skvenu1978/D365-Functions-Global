// <copyright file="SpFileHandler.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.SharePoint
{
    using Microsoft.Graph;
    using Microsoft.Graph.Models;

    using Microsoft.Graph.Models.ODataErrors;
    using Microsoft.Graph.Drives.Item.Items.Item.CreateUploadSession;

    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Const;
    using System.IO;

    /// <summary>
    /// Class to Handle File uploads
    /// to SharePoint
    /// </summary>
    public class SpFileHandler
    {
        public static DynTaskResponse UploadAttachmentsToSharePointOMNIFolder(string spSiteName, SpCredentials creds, string entityFolderName, string spRecordFolder, AttachmentRequest attachmentData)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            string spSiteId = SpConverter.GetSpSiteIdBasedOnSiteName(spSiteName);

            try
            {
                GraphServiceClient graphServiceClient = SpAuthProvider.GetGraphService(creds);

                // get the list id linked to the entity level folder
                string spListID = GetEntityFolderListId(graphServiceClient, entityFolderName, spSiteId);

                if (!string.IsNullOrEmpty(spListID))
                {
                    string spDriveId = GetDriveIdLinkedtoEntity(spSiteId, spListID, graphServiceClient);

                    if (!string.IsNullOrEmpty(spDriveId))
                    {
                        // create Record Level Folder in SharePoint
                        string? recordFolderId = EnsureRecordLevelSpFolder(spDriveId, spRecordFolder, graphServiceClient);

                        if (!string.IsNullOrEmpty(recordFolderId))
                        {
                            string? uploadFolderId = EnsureOmniDocumentsSpFolder(spDriveId, spRecordFolder, recordFolderId, graphServiceClient);

                            if (!string.IsNullOrEmpty(uploadFolderId))
                            {
                                UploadFileToFolder(spDriveId, uploadFolderId, graphServiceClient, attachmentData);
                                response.IsSuccess = true;
                            }
                        }
                    }
                }
            }
            catch (ODataError ex)
            {
                if (ex.Error != null)
                {
                    MainError err = ex.Error;

                    if (!string.IsNullOrEmpty(err.Message))
                        response.ErrorMessage = err.Message;
                }
            }

            return response;
        }

        /// <summary>
        /// Upload file to the folder
        /// location linked to the specific
        /// record in D365
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderId"></param>
        /// <param name="graphClient"></param>
        /// <param name="attachmentData"></param>
        private static void UploadFileToFolder(string driveId, string folderId, GraphServiceClient graphClient, AttachmentRequest attachmentData)
        {
            using (Stream fileStream = new MemoryStream(attachmentData.Attachment))
            {
                CreateUploadSessionPostRequestBody uploadSessionRequestBody = new CreateUploadSessionPostRequestBody
                {
                    Item = new DriveItemUploadableProperties
                    {
                        AdditionalData = new Dictionary<string, object>
                                        {
                                            { "@microsoft.graph.conflictBehavior", "rename" }, // fail, replace, or rename
                                        },
                    },
                };

                var uploadSessionTask = graphClient.Drives[driveId]
                                                                .Items[folderId]
                                                                .ItemWithPath(attachmentData.Filename)
                                                                .CreateUploadSession
                                                                .PostAsync(uploadSessionRequestBody);
                uploadSessionTask.Wait();

                UploadSession? uploadSession = uploadSessionTask.Result;

                int maxSliceSize = 320 * 1024;
                LargeFileUploadTask<DriveItem> fileUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, fileStream, maxSliceSize, graphClient.RequestAdapter);

                long totalLength = fileStream.Length;
                IProgress<long> progress = new Progress<long>(prog => Console.WriteLine($"Uploaded {prog} bytes of {totalLength} bytes"));

                var uploadResultTask = fileUploadTask.UploadAsync(progress);
                uploadResultTask.Wait();

                UploadResult<DriveItem> uploadResult = uploadResultTask.Result;
            }
        }

        /// <summary>
        /// Gets the list Id
        /// linked to the Entity Name
        /// For ex: Account,Contact or Lead
        /// </summary>
        /// <param name="graphServiceClient"></param>
        /// <param name="listName"></param>
        /// <param name="spSiteId"></param>
        /// <returns></returns>
        private static string GetEntityFolderListId(GraphServiceClient graphServiceClient, string listName, string spSiteId)
        {
            string listId = string.Empty;

            var siteFolderTask = graphServiceClient.Sites[spSiteId].Lists[listName].GetAsync();
            siteFolderTask.Wait();

            List? res = siteFolderTask.Result;

            if (res != null)
            {
                string? siteFolderId = res.Id;

                if (!string.IsNullOrEmpty(siteFolderId))
                {
                    listId = siteFolderId;
                }
            }

            return listId;
        }

        /// <summary>
        /// get the drive id linked to the 
        /// SharePoint folder
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="listId"></param>
        /// <param name="graphServiceClient"></param>
        /// <returns></returns>
        private static string GetDriveIdLinkedtoEntity(string siteId, string listId, GraphServiceClient graphServiceClient)
        {
            string strListDriveId = string.Empty;
            string? listDriveId = string.Empty;

            var listDriveTask = graphServiceClient.Sites[siteId]
                                                .Lists[listId]
                                                .Drive
                                                .GetAsync();
            listDriveTask.Wait();

            Drive? res = listDriveTask.Result;

            if (res != null)
            {
                listDriveId = res.Id;

                if (!string.IsNullOrEmpty(listDriveId))
                {
                    strListDriveId = listDriveId;
                }
            }

            return strListDriveId;
        }

        /// <summary>
        /// Gets the folder Id
        /// linked to the record in D365
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderName"></param>
        /// <param name="graphServiceClient"></param>
        /// <returns></returns>
        private static string? GetRecordLinkedFolderId(string driveId, string folderName, GraphServiceClient graphServiceClient)
        {
            string? folderId = string.Empty;

            try
            {
                DriveItem? folder = new DriveItem();
                string path = "/" + folderName;
                //DriveItemCollectionResponse? folderSearch;
                //var folderSearchTask = graphServiceClient.Drives[driveId].Items.GetAsync(requestConfig => requestConfig.QueryParameters.Filter = $"Name eq '{folderName}'");

                var folderSearchTask = graphServiceClient.Drives[driveId].Root.ItemWithPath(path).GetAsync();
                folderSearchTask.Wait();

                if (folderSearchTask != null)
                {
                    folder = folderSearchTask.Result;

                    if (folder != null)
                    {
                        folderId = folder.Id;
                    }
                }
            }
            catch (ODataError ex)
            {
                if (ex.Error != null)
                {
                    MainError err = ex.Error;
                }
            }

            return folderId;
        }

        /// <summary>
        /// checks if OMNI Documents
        /// folder exists and creates
        /// if it does not exist
        /// </summary>
        /// <param name="driveId"></param>
        /// <param name="folderName"></param>
        /// <param name="graphServiceClient"></param>
        /// <returns></returns>
        private static string? EnsureOmniDocumentsSpFolder(string driveId, string folderName, string recordFolderId, GraphServiceClient graphServiceClient)
        {
            string? omniFolderId = string.Empty;
            DriveItem? folder = new DriveItem();
            string path = "/" + folderName;

            var folderSearchTask = graphServiceClient.Drives[driveId].Root.ItemWithPath(path).Children.GetAsync(requestConfig => requestConfig.QueryParameters.Select = new string[] { "Name", "Id" });

            folderSearchTask.Wait();
            DriveItemCollectionResponse? folderSearch = folderSearchTask.Result;

            if (folderSearch != null)
            {
                if (folderSearch != null && folderSearch.Value != null && folderSearch.Value.Count > 0)
                {
                    foreach (var item in folderSearch.Value)
                    {
                        DriveItem? spItem = item;

                        string? itemName = spItem.Name;

                        if (Equals(itemName, SharePointConstants.SpOMNIFolder))
                        {
                            if (!string.IsNullOrEmpty(spItem.Id))
                            {
                                omniFolderId = spItem.Id;
                            }

                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(omniFolderId))
            {
                var requestBody = new DriveItem
                {
                    Name = SharePointConstants.SpOMNIFolder,
                    Folder = new Folder
                    {
                    },
                    AdditionalData = new Dictionary<string, object>
                    {
                        {
                            "@microsoft.graph.conflictBehavior" , "rename"
                        },
                    },
                };

                var createOmniFolderTask = graphServiceClient.Drives[driveId].Items[recordFolderId].Children.PostAsync(requestBody);
                createOmniFolderTask.Wait();

                DriveItem? omniFolderTaskResult = createOmniFolderTask.Result;

                if (omniFolderTaskResult != null)
                {
                    omniFolderId = omniFolderTaskResult.Id;
                }
            }

            return omniFolderId;
        }

        private static string? EnsureRecordLevelSpFolder(string driveId, string folderName, GraphServiceClient graphServiceClient)
        {
            string? recordFolderId = GetRecordLinkedFolderId(driveId, folderName, graphServiceClient);

            if (string.IsNullOrEmpty(recordFolderId))
            {
                var requestBody = new DriveItem
                {
                    Name = folderName,
                    Folder = new Folder
                    {
                    },
                    AdditionalData = new Dictionary<string, object>
                    {
                        {
                            "@microsoft.graph.conflictBehavior" , "rename"
                        },
                    },
                };

                var createRecordFolderTask = graphServiceClient.Drives[driveId].Items.PostAsync(requestBody);
                createRecordFolderTask.Wait();

                DriveItem? recordFolderTaskResult = createRecordFolderTask.Result;

                if (recordFolderTaskResult != null)
                {
                    recordFolderId = recordFolderTaskResult.Id;
                }
            }

            return recordFolderId;
        }
    }
}