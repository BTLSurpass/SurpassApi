using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.ItemTagValue;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class ItemTagValueHelper : SurpassHelper
    {
        public ItemTagValueHelper(SurpassApiClient client) : base(client)
        {

        }

        public long? Post(ItemTagValueInputResource tagResource)
        {
            try
            {
                var controller = m_surpassApiClient.ItemTagValue;
                var myResponse = controller.Post(tagResource);
                return myResponse.Id;
            }
            catch (ResourceException)
            {
                return null;
            }

        }
    }
}
