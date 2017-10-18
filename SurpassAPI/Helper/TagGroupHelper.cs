using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.DataContracts.TagGroup;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class TagGroupHelper : SurpassHelper
    {
        public TagGroupHelper(SurpassApiClient client) : base(client)
        {

        }

        public TagGroupDetailedResource CreateTagGroup(TagGroupInputResource tagGroup)
        {
            try
            {
                var controller = m_surpassApiClient.TagGroup;
                var myResponse = controller.Post(tagGroup);
                if (myResponse.Id != null)
                {
                    TimeZonePageResponse<TagGroupDetailedResource> myGetResponse = controller.Get((int)myResponse.Id);
                    return myGetResponse.Response.ToList().FirstOrDefault();
                }
                return null;
            }
            catch (ResourceException ex)
            {
                return null;
            }
        }

        public TagGroupDetailedResource GetTagGroup(string name, string subjectRef)
        {
            try
            {
                var controller = m_surpassApiClient.TagGroup;
                var myQuery = $"$filter=subject/reference eq '{subjectRef}' and contains(name,'{name}')";
                TimeZonePageResponse<TagGroupResource> myResponse = controller.Get(myQuery);
                var myTagGroup =  myResponse.Response.ToList().Where(t => t.Name == name).ToList().FirstOrDefault();
                if (myTagGroup == null)
                {
                    return null;
                }
                var myDetailedTagGroup = controller.Get(myTagGroup.Id.Value);
                return myDetailedTagGroup.Response.FirstOrDefault();
            }
            catch (ResourceException ex)
            {
                return null;
            }
        }
    }
}
