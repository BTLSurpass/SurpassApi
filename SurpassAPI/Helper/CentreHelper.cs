using System;
using System.Collections.Generic;
using System.Linq;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Centre;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class CentreHelper: SurpassHelper
    {
        public CentreHelper(SurpassApiClient client):base(client)
        {
        }
        /// <summary>
        /// Wrapper method to create or update an existing centre - see CentreCreateUpdateResource for required fields
        /// </summary>
        /// <param name="centre"></param>
        /// <returns></returns>
        public CentreDetailedResource CreateOrUpdateCentre(CentreCreateUpdateResource centre)
        {
            CentreDetailedResource myCentreResource = GetCentre(centre);
            if (myCentreResource != null)
            {
                if (!CheckEquivalent(centre, myCentreResource))
                {
                    UpdateCentre(centre);
                }
                return myCentreResource;
            }
            TimeZonePostResponseModel myCentreResponse = CreateCentre(centre);
            //Sanity check
            if (myCentreResponse.Reference == centre.Reference)
            {
                myCentreResource = GetCentre(centre);
            }
            return myCentreResource;
        }
        internal Boolean CheckEquivalent(CentreCreateUpdateResource centre1, CentreDetailedResource centre2)
        {

            var nameCheck = (centre1.Name == centre2.Name);
            var hideSubjectFromSubjectGroupsCheck = (centre1.HideSubjectFromSubjectGroups == centre2.HideSubjectFromSubjectGroups) || (centre1.HideSubjectFromSubjectGroups == null && centre2.HideSubjectFromSubjectGroups == false);
            var randomiseTestFormsCheck = (centre1.RandomiseTestForms == centre2.RandomiseTestForms) || (centre1.RandomiseTestForms == null && centre2.RandomiseTestForms == true);
            return nameCheck && hideSubjectFromSubjectGroupsCheck && randomiseTestFormsCheck;
        }
        /// <summary>
        /// Get a centre by the centre reference
        /// </summary>
        /// <param name="centreReference">valid centre reference</param>
        /// <returns></returns>
        public CentreDetailedResource GetCentre(string centreReference)
        {
            var centreController = m_surpassApiClient.Centre;
            TimeZonePageResponse<CentreDetailedResource> myCentre = centreController.GetByReference(centreReference);
            return myCentre.Response.ToList().FirstOrDefault();
        }
        /// <summary>
        /// Get a Centre
        /// </summary>
        /// <param name="centre"></param>
        /// <returns></returns>
        public CentreDetailedResource GetCentre(CentreCreateUpdateResource centre)
        {
            return GetCentre(centre.Reference);
        }
        /// <summary>
        /// Create a centre
        /// See CentreCreateUpdateResource for details on the required fields
        /// </summary>
        /// <param name="centre"></param>
        /// <returns></returns>
        public TimeZonePostResponseModel CreateCentre(CentreCreateUpdateResource centre)
        {
            var centreController = m_surpassApiClient.Centre;
            TimeZonePostResponseModel myCentreResponse = centreController.Post(centre);
            return myCentreResponse;
        }
        /// <summary>
        /// Update an existing centre
        /// </summary>
        /// <param name="centre"></param>
        /// <returns></returns>
        public TimeZonePostResponseModel UpdateCentre(CentreCreateUpdateResource centre)
        {
            var centreController = m_surpassApiClient.Centre;
            TimeZonePostResponseModel myCentreResponse = centreController.Put(centre.Reference, centre);
            return myCentreResponse;
        }
        /// <summary>
        /// Get all centres that you have access to
        /// </summary>
        /// <returns></returns>
        public List<CentreLinkedResource> GetAllCentres()
        {
            try
            {
                var centreController = m_surpassApiClient.Centre;
                TimeZonePageResponse<CentreLinkedResource> centres = centreController.Get();
                List<CentreLinkedResource> allCentres = saveRest(centres.Response.GetEnumerator());
                return allCentres;
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }
        private static List<T> saveRest<T>(IEnumerator<T> e)
        {
            var list = new List<T>();
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list;
        }

        public CentreResource Convert(CentreDetailedResource detailedResource)
        {
            CentreResource myResource = new CentreResource { Id = detailedResource.Id };
            return myResource;
        }
    }
}
