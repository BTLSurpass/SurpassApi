using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Item;
using SurpassApiSdk.DataContracts.Media;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class MediaHelper : SurpassHelper
    {
        public MediaHelper(SurpassApiClient client) : base(client)
        {

        }

        public long? Post(string subjectReference, byte[] media, string name)
        {
            var controller = m_surpassApiClient.Media;
            MediaInputResource myResource = new MediaInputResource
            {
                Name = name,
                Data = media,
                Subject = new ItemSubjectResource { Reference = subjectReference }
            };
            try
            {
                var myResponse = controller.Post(myResource);
                return myResponse.Id;
            }
            catch (ResourceException ex)
            {
                return 0;
            }

        }
    }
}
