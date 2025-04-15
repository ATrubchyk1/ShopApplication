using Braintree;
using Microsoft.Extensions.Options;

namespace ShopApplication_Utility.BrainTree;

public class BrainTreeGate : IBrainTreeGate
{
    private BrainTreeSettings Options { get; set; }
    private IBraintreeGateway BrainTreeGateWay { get;set; }

    public BrainTreeGate(IOptions<BrainTreeSettings> options)
    {
        Options = options.Value;
    }
    
    public IBraintreeGateway CreateGateway()
    {
        return new BraintreeGateway(Options.Environment, Options.MerchantId, Options.PublicKey, Options.PrivateKey);
    }

    public IBraintreeGateway GetGateWay()
    {
        if (BrainTreeGateWay == null)
        {
            BrainTreeGateWay = CreateGateway();
        }
        return BrainTreeGateWay;
    }

    public string GetClientBrainTreeToken()
    {
        var gateway = GetGateWay();
        return gateway.ClientToken.Generate();
    }
}