﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Folder;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class FolderHelper : SurpassHelper
    {
        public FolderHelper(SurpassApiClient client) : base(client)
        {

        }

        public FolderResource GetFolderByName(FolderInputResource folder, String subjectReference)
        {
            try
            {
                var foldercontroller = m_surpassApiClient.Folder;
                var oDataQuery = string.Format("$filter=name eq '{0}' and Subject/Reference eq '{1}'", folder.Name, subjectReference);
                TimeZonePageResponse<FolderResource> myGetResponse = foldercontroller.Get(oDataQuery);
                return myGetResponse.Response.ToList().Where(t => t.Name == folder.Name).ToList().FirstOrDefault();
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }

        public FolderResource CreateFolder(FolderInputResource folder)
        {
            try
            {
                var foldercontroller = m_surpassApiClient.Folder;
                PostResponseModel myResponse = foldercontroller.Post(folder);
                if (myResponse.Id != null)
                {
                    TimeZonePageResponse<FolderResource> myGetResponse = foldercontroller.Get((int)myResponse.Id);
                    return myGetResponse.Response.ToList().FirstOrDefault();
                }
                return null;
            }
            catch (ResourceException ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the folder if it exists or creates one
        /// Wrapper function for CreateFolder & GetFolder
        /// </summary>
        /// <param name="folderInputResource">The folder input resource.</param>
        /// <returns></returns>
        public FolderResource GetOrCreateFolder(FolderInputResource folderInputResource, String subjectReference)
        {
            FolderResource myFolder = GetFolderByName(folderInputResource, subjectReference);
            if (myFolder == null)
            {
                myFolder = CreateFolder(folderInputResource);
            }
            return myFolder;
        }
    }
}
