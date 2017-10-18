using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.DataContracts.TagValue;

namespace SurpassAPI.Helper
{
    public class TagValueHelper : SurpassHelper
    {
        public TagValueHelper(SurpassApiClient client) : base(client)
        {
           
        }
        public TagValueResource CreateTag(long tagGroupId, string value)
        {
            var controller = m_surpassApiClient.TagValue;
            var tagValue = new TagValueInputResource {
                TagValue = value,
                TagGroup = new TagValueGroupResource { Id = tagGroupId }
            };
            var response = controller.Post(tagValue);
            
            TimeZonePageResponse<TagValueResource> myTagResponse = controller.Get(response.Id.Value);
            return myTagResponse.Response.ToList().FirstOrDefault();
        }

        public TagValueResource Get(int tagGroupId, string name)
        {
            var controller = m_surpassApiClient.TagValue;
            var myTagId = checkTagAlreadyExists(name, tagGroupId);
            if (myTagId != null)
            {
                TimeZonePageResponse<TagValueResource> myResponse = controller.Get(myTagId.Value);
                return myResponse.Response.ToList().FirstOrDefault();
            }
            return null;
        }

        private long? checkTagAlreadyExists(string tagName, long group)
        {
            var controller = m_surpassApiClient.TagValue;
            //There is a defect in the TagValue filter when the name contains ' and ', so if this is the case it has to loop through all values for the group
            //and get the one with the required name.
            if (tagName.ToLower().Contains(" and "))
            {
                int page = 0;
                int pageSize = 10;
                while (true)
                {
                    var response = controller.Get($"$filter=TagGroup.id eq {group}&$top={pageSize}&$skip={page * pageSize}");
                    if (response.Response.Any(tagValue => tagValue.TagValue == tagName))
                    {
                        return response.Response.Single(tagValue => tagValue.TagValue == tagName).Id;
                    }
                    if (response.NextPageLink == null)
                    {
                        return null;
                    }
                    page++;
                }
            }
            else
            {
                var response = controller.Get($"$filter=TagGroup.id eq {group} and TagValue eq '{tagName}'");
                if (response.Response.Any())
                {
                    return response.Response.First().Id;
                }
            }
            return null;
        }
    }
}
