using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurpassApiSdk;
using SurpassApiSdk.DataContracts.Item;
using SurpassApiSdk.DataContracts.Response;
using SurpassApiSdk.Exceptions;

namespace SurpassAPI.Helper
{
    public class ItemHelper: SurpassHelper
    {
        public ItemHelper(SurpassApiClient client) : base(client)
        {

        }
        public ItemResource CreateItem(ItemInputResource item)
        {
            try
            {
                var itemController = m_surpassApiClient.Item;
                PostResponseModel myItem = itemController.Post(item);
                Console.WriteLine("Created " + myItem.Id);
                if (myItem.Id != null)
                {
                    TimeZonePageResponse<ItemResource> myItemResponse = itemController.Get((int)myItem.Id);
                    return myItemResponse.Response.ToList().FirstOrDefault();
                }
                return null;
            }
            catch (ResourceException ex)
            {
                //Does not exist
                return null;
            }
        }
    }
}
