using XRoadLib.Attributes;

namespace WebApplication.Contracts
{
    public interface ICalculate
    {
        [XRoadService("Calculate")]
        [XRoadTitle("en", "Calculation service")]
        [XRoadNotes("en", "Performs specified operation on two user provided integers and returns the result.")]
        int Calculate(CalculationRequest request);
    }
}