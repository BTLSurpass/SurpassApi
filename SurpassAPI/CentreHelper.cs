using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Centre;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI
{
    public class CentreHelper
    {
        private readonly SurpassApiClient m_surpassApiClient;
        public CentreHelper(SurpassApiClient client)
        {
            m_surpassApiClient = client;
        }
        public CentreDetailedResource GetCentre(string centreReference)
        {
            var centreController = m_surpassApiClient.Centre;
            TimeZonePageResponse<CentreDetailedResource> myCentre = centreController.GetByReference(centreReference);
            return myCentre.Response.ToList().FirstOrDefault();
        }

        public CentreDetailedResource GetCentre(CentreCreateUpdateResource centre)
        {
            return GetCentre(centre.Reference);
        }
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
    }
}
