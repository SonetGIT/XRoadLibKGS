using xRoadWeb.Contracts;

namespace xRoadWeb.Services
{
    public class SumOfIntegersWebService : ISumOfIntegers
    {
        public int Sum(AddRequest request)
        {
            return request.X + request.Y;
        }
    }
}