using System.Threading.Tasks;
using Scadenzario.Models.InputModels.Scadenze;
namespace Scadenzario.Models.Services.Infrastructure
{
    public interface IPaymentGatewayStripe
    {
        Task<string> GetPaymentUrlAsyncStripe(ScadenzaPayInputModel inputModel);
        Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncStripe(string token);
    }
}