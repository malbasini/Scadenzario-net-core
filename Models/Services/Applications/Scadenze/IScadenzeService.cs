using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Models.Services.Applications.Scadenze;

public interface IScadenzeService
{
    Task<ListViewModel<ScadenzaViewModel>?> GetScadenzeAsync(ScadenzaListInputModel model, int anno, CancellationToken ct);
    Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id);
    
    Task<ScadenzaDetailViewModel> GetScadenzaAsync(int id);
    Task<ScadenzaDetailViewModelInfo> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel);
    Task<ScadenzaDetailViewModelInfo> EditScadenzaAsync(ScadenzaEditInputModel inputModel);
    Task<string> DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel);
    
    List<SelectListItem> GetBeneficiari();
    
    string GetBeneficiarioById(int IdBeneficiario);
    
    int DateDiff(DateTime inizio, DateTime fine);
    bool IsDate(string date);
    Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncPayPal(int id, string token);
    Task SubscribeScadenzaAsync(ScadenzaSubscribeInputModel inputModel);
    Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncStripe(int id, string token);
    Task<string> GetPaymentUrlAsyncPayPal(int id);
    Task<string> GetPaymentUrlAsyncStripe(int id);
}