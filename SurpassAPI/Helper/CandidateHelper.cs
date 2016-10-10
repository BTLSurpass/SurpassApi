using System;
using System.Linq;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Candidate;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class CandidateHelper: SurpassHelper
    {
        public CandidateHelper(SurpassApiClient client) : base(client)
        {
            
        }
        /// <summary>
        /// Create a candidate
        /// </summary>
        /// <param name="aCandidate"></param>
        /// <returns></returns>
        public CandidateDetailedResource CreateCandidate(CandidateCreateResource aCandidate)
        {
            try
            {
                var candidateController = m_surpassApiClient.Candidate;
                PostResponseModel myResponse = candidateController.Post(aCandidate);
                if (myResponse.Id != null)
                {
                    return GetCandidate((int)myResponse.Id);
                }
            }
            catch (ResourceException)
            {

                return null;

            }

            return null;
        }
        /// <summary>
        /// Wrapper method to create or update candidate accordingly - see CandidateCreateResource for required fields
        /// </summary>
        /// <param name="aCandidate"></param>
        /// <returns></returns>
        public CandidateDetailedResource CreateOrUpdateCandidate(CandidateCreateResource aCandidate)
        {
            var myCandidate = GetCandidate(aCandidate.Reference);

            if (myCandidate == null)
            {
                //create
                return CreateCandidate(aCandidate);
            }
            else
            {
                //update
                CandidateUpdateResource myUpdateCandiate = convertCandidateCreateResourceToUpdateResource(aCandidate);
                return UpdateCandidate(aCandidate.Reference, myUpdateCandiate);
            }
        }
        private CandidateUpdateResource convertCandidateCreateResourceToUpdateResource(CandidateCreateResource aCandidate)
        {
            CandidateUpdateResource myUpdateCandiate = new CandidateUpdateResource
            {
                Centres = aCandidate.Centres,
                DateOfBirth = aCandidate.DateOfBirth,
                Email = aCandidate.Email,
                FirstName = aCandidate.FirstName,
                LastName = aCandidate.LastName,
                Retired = aCandidate.Retired,
                Tel = aCandidate.Tel,
                SpecialRequirements = aCandidate.SpecialRequirements,
                Subjects = aCandidate.Subjects
            };
            return myUpdateCandiate;
        }
        /// <summary>
        /// Update Candidate Resource - see CandidateUpdateResource for required fields
        /// </summary>
        /// <param name="candiateReference"></param>
        /// <param name="aCandidate"></param>
        /// <returns></returns>
        public CandidateDetailedResource UpdateCandidate(String candiateReference, CandidateUpdateResource aCandidate)
        {
            var candidateController = m_surpassApiClient.Candidate;
            TimeZonePostResponseModel myResponse = candidateController.Put(candiateReference, aCandidate);
            if ((myResponse.Errors == null) || (!myResponse.Errors.Any()))
            {
                return GetCandidate(candiateReference);
            }
            return null;
        }
        /// <summary>
        /// Get candidate by candidate reference
        /// </summary>
        /// <param name="candidateReference"></param>
        /// <returns></returns>
        public CandidateDetailedResource GetCandidate(String candidateReference)
        {
            try
            {
                var candidateController = m_surpassApiClient.Candidate;
                TimeZonePageResponse<CandidateDetailedResource> myCandidateResponse = candidateController.GetByReference(candidateReference);
                return myCandidateResponse.Response.ToList().FirstOrDefault();
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }
        /// <summary>
        /// Get a candidate by Surpass internal Id
        /// </summary>
        /// <param name="candidateId"></param>
        /// <returns></returns>
        public CandidateDetailedResource GetCandidate(int candidateId)
        {
            try
            {
                var candidateController = m_surpassApiClient.Candidate;
                TimeZonePageResponse<CandidateDetailedResource> myCandidateResponse = candidateController.Get(candidateId);
                return myCandidateResponse.Response.ToList().FirstOrDefault();
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }
    }
}
