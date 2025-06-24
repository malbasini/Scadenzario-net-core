using System.Threading.Tasks;
using Scadenzario.Models.InputModels.Scadenze;

namespace Scadenzario.Models.Services.Infrastructure;

public interface IPaymentGatewayPayPal
{
    Task<string> GetPaymentUrlAsyncPayPal(ScadenzaPayInputModel inputModel);
    Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncPayPal(string token);
}