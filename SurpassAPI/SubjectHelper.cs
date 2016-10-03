using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.DataContracts.Subject;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI
{

    public class SubjectHelper:SurpassHelper
    {
        public SubjectHelper(SurpassApiClient client):base(client)
        {

        }
        /// <summary>
        /// Wrapper method to create or update an existing centre - see SubjectCreateResource for required field
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public SubjectDetailedResource CreateOrUpdateSubject(SubjectCreateResource subject)
        {
            SubjectDetailedResource mySubject = GetSubject(subject);
            if (mySubject == null)
            {
                return CreateSubject(subject);
            }
            if (!CheckEquivalent(subject, mySubject))
            {
                return UpdateSubject(subject);
            }
            return mySubject;
        }
        internal Boolean CheckEquivalent(SubjectCreateResource subject1, SubjectDetailedResource subject2)
        {
            var nameCheck = (subject1.Name == subject2.Name);
            var centreCheck = (subject1.PrimaryCentre.Id == subject2.PrimaryCentre.Id);
            return nameCheck && centreCheck;
        }
        /// <summary>
        /// Create a subject - see SubjectCreateResource for required fields
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public SubjectDetailedResource CreateSubject(SubjectCreateResource subject)
        {
            var subjectController = m_surpassApiClient.Subject;
            TimeZonePostResponseModel myResponse = subjectController.Post(subject);
            if ((myResponse.Errors == null) || (!myResponse.Errors.Any()))
            {
                return GetSubject(subject);
            }
            return null;
        }
        public SubjectDetailedResource GetSubject(SubjectCreateResource subject)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            return GetSubject(subject.Reference);
        }
        /// <summary>
        /// Get Subject by subject reference
        /// </summary>
        /// <param name="subjectReference">a valid subject reference</param>
        /// <returns></returns>
        public SubjectDetailedResource GetSubject(String subjectReference)
        {
            try
            {
                var subjectController = m_surpassApiClient.Subject;
                TimeZonePageResponse<SubjectDetailedResource> mySubject = subjectController.GetByReference(subjectReference);
                return mySubject.Response.ToList().FirstOrDefault();
            }
            catch (ResourceException)
            {
                //Does not exist
                return null;
            }
        }
        public SubjectDetailedResource UpdateSubject(SubjectCreateResource subject)
        {
            var subjectController = m_surpassApiClient.Subject;
            SubjectUpdateResource myUpdate = new SubjectUpdateResource
            {
                Name = subject.Name,
                PrimaryCentre = subject.PrimaryCentre,
            };
            TimeZonePostResponseModel myResponse = subjectController.Put(subject.Reference, myUpdate);
            if (!myResponse.Errors.Any())
            {
                return GetSubject(subject);
            }
            return null;
        }

        public SubjectResource Convert(SubjectDetailedResource detailedResource)
        {
            SubjectResource mySubjectResource = new SubjectResource { Id = detailedResource.Id };
            return mySubjectResource;
        }
    }
}
