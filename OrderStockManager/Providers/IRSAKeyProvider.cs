using System.Threading.Tasks;

namespace OrderStockManager.Providers
{
    public interface IRSAKeyProvider
    {
        string CreateKey(bool includePrivateParameters = false);
        void DeleteKeys();
        string GetPrivateKey();
        string GetPublicKey();
    }
}
