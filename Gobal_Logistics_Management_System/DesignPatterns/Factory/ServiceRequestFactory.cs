namespace Global_Logistics_Management_System.DesignPatterns.Factory
{
    public abstract class ServiceRequestBase
    {
        public int ContractId { get; set; }
        public string? Description { get; set; }
        public decimal? CostUSD { get; set; }
        public abstract string RequestType { get; }
    }

    public class StandardRequest : ServiceRequestBase
    {
        public override string RequestType => "Standard";
        public int EstimatedDays { get; set; } = 7;
    }

    public class PriorityRequest : ServiceRequestBase
    {
        public override string RequestType => "Priority";
        public bool RequiresImmediateAttention => true;
        public decimal PriorityFee { get; set; } = 500m;
    }

    public static class ServiceRequestFactory
    {
        public static ServiceRequestBase CreateRequest(string type, int contractId, string description, decimal? costUSD)
        {
            ServiceRequestBase request = type switch
            {
                "Standard" => new StandardRequest(),
                "Priority" => new PriorityRequest(),
                _ => throw new ArgumentException("Invalid request type")
            };

            request.ContractId = contractId;
            request.Description = description;
            request.CostUSD = costUSD;
            return request;
        }

        internal static object CreateRequest(object requestType, int contractId, object description, object costUSD)
        {
            throw new NotImplementedException();
        }
    }
}
