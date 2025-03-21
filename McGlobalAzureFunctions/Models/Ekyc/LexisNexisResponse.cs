using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc
{
    public class LexisNexisResponse
    {
        public Status? Status { get; set; }
        public List<Product>? Products { get; set; }

    }

    public class Item
    {
        public string? ItemName { get; set; }
        public string? ItemStatus { get; set; }
        public ItemReason? ItemReason { get; set; }
        public List<ItemInformationDetail>? ItemInformationDetails { get; set; }
    }

    public class ItemInformationDetail
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public class ItemReason
    {
        public string? Code { get; set; }
    }

    public class Product
    {
        public string? ProductType { get; set; }
        public string? ExecutedStepName { get; set; }
        public string? ProductConfigurationName { get; set; }
        public string? ProductStatus { get; set; }
        public List<Item>? Items { get; set; }
    }
    public class Status
    {
        public string? ConversationId { get; set; }
        public string? RequestId { get; set; }
        public string? TransactionStatus { get; set; }
    }

    public class ScreeningApiResult
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
    }

}
