using Braintree;

namespace ShopApplication_Utility.BrainTree;

public interface IBrainTreeGate
{
    public IBraintreeGateway CreateGateway();
    
    public IBraintreeGateway GetGateWay();

    public string GetClientBrainTreeToken();
}