using System.Globalization;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Scadenzario.Customizations.Authorization;
using Scadenzario.Models.Enums;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.ViewModels;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Controllers
{
    [Authorize]
    public class ScadenzeController : Controller
    {
        private readonly IScadenzeService _service;
        private readonly IRicevuteService _ricevute;
        private readonly AnaliticheSpeseService _svc;
        public ScadenzeController(ICachedScadenzeService service, IRicevuteService ricevute, AnaliticheSpeseService svc)
        {
            _service = service;
            _ricevute = ricevute;
            _svc = svc;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(ScadenzaListInputModel input, [FromQuery] int? anno, CancellationToken ct)
        {
            ViewData["Title"] = "Lista Scadenze";
            ListViewModel<ScadenzaViewModel>? scadenze = await _service.GetScadenzeAsync(input,anno ?? DateTime.UtcNow.Year, ct);
            
            var anni = await _svc.GetAnniDisponibiliAsync(ct);
            var selected = anno ?? DateTime.UtcNow.Year;

            ScadenzaListViewModel viewModel = new ScadenzaListViewModel
            {
                Scadenze = scadenze,
                Input = input,
                Anni = anni,
                AnnoSelezionato = selected,
            };

            return View(viewModel);
        }
        // GET /Scadenze/GraficoCategorie
        [HttpGet("GraficoCategorie")]
        public async Task<IActionResult> GraficoCategorie([FromQuery] int? anno, DateTime? dal, DateTime? al, string? chart, string? filter,CancellationToken ct)
        {
            TempData["dal"] = dal;
            TempData["al"] = al;
            string? denominazione = filter;
            DateTime? dataScadenza = null;

            if (string.IsNullOrWhiteSpace(filter))
            {
                denominazione = null;
                dataScadenza = null;
            }
            else
            {
                try
                {
                    dataScadenza = DateTime.ParseExact(filter, "dd/MM/yyyy", CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.None);
                    denominazione = null;
                }
                catch (Exception e)
                {
                    dataScadenza = null;
                    denominazione = filter;
                }
            }
            
            // Normalizza/valida il tipo
            static string Normalize(string? t) => t?.ToLowerInvariant() switch
            {
                "bar"        => "bar",
                "line"       => "line",
                "pie"        => "pie",
                "doughnut"   => "doughnut",
                "polararea"  => "polarArea",   // accetto anche “polararea”
                "polarArea"  => "polarArea",
                "radar"      => "radar",
                _            => "bar"
            };
            var chartType = Normalize(chart);
            var anni = await _svc.GetAnniDisponibiliAsync(ct);
            var selected = anno ?? DateTime.UtcNow.Year;

            var data = await _svc.GetTotaliPerCategoriaAnnoAsync(selected,dal,al,denominazione, ct,dataScadenza);
            var vm = new GraficoCategorieViewModel
            {
                Anni = anni,
                AnnoSelezionato = selected,
                Labels = data.Select(d => d.Categoria).ToList(),
                Values = data.Select(d => d.Totale).ToList(),
                Dal = dal,
                Al = al,
                Chart = chartType
            };
            return View("GraficoCategorie", vm);
        }

        // API JSON (opzionale) /api/scadenze/spese-per-categoria?anno=2025
        [HttpGet("/api/scadenze/spese-per-categoria")]
        public async Task<IActionResult> SpesePerCategoriaApi([FromQuery] int? anno, DateTime? dal, DateTime? al, string? filter,CancellationToken ct)
        {
            var selected = anno ?? DateTime.UtcNow.Year;
            var data = await _svc.GetTotaliPerCategoriaAnnoAsync(selected,dal,al,filter, ct);
            return Ok(data);
        }
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Dettaglio Scadenza";
            ScadenzaDetailViewModel viewModel = await _service.GetScadenzaAsync(id);
            if(viewModel.DataPagamento==null)
                viewModel.DataPagamento = DateTime.Now;
            viewModel.Ricevute = _ricevute.GetRicevute(id);
            return View(viewModel);
        }
        [HttpGet]
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuova Scadenza";
            ScadenzaCreateInputModel inputModel = new ScadenzaCreateInputModel();
            inputModel.DataScadenza = DateTime.Now;
            inputModel.Beneficiari = _service.GetBeneficiari();
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [ValidateReCaptcha]
        [HttpPost]
        public async Task<IActionResult> Create(ScadenzaCreateInputModel inputModel)
        {
            inputModel.Beneficiari = _service.GetBeneficiari();
            if(ModelState.IsValid)
            {
                ScadenzaDetailViewModelInfo detail = await _service.CreateScadenzaAsync(inputModel);
                TempData["ConfirmationMessage"] = "OK! la scadenza è stata creata, ora perché non inserisci gli altri dati!";
                return RedirectToAction(nameof(Edit), new { id = detail.IDScadenza });
            }
            else
            {
                
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(allErrors);
                ViewData["Title"] = "Nuova Scadenza";
                return View(inputModel); 
            }
              
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            TempData["IDScadenza"]=id; 
            ViewData["Title"] = "Aggiorna Scadenza";
            ScadenzaEditInputModel inputModel = new ScadenzaEditInputModel();
            inputModel.DataPagamento = DateTime.Now;
            inputModel = await _service.GetScadenzaForEditingAsync(id);
            inputModel.Denominazione=_service.GetBeneficiarioById(inputModel.IdBeneficiario);
            inputModel.Beneficiari = _service.GetBeneficiari();
            return View(inputModel);
        }
        [AuthorizeRole(Role.Administrator,Role.Editor)]
        [HttpPost]
        public async Task<IActionResult> Edit(ScadenzaEditInputModel inputModel)
        {
            inputModel.Denominazione=_service.GetBeneficiarioById(inputModel.IdBeneficiario);
            if(inputModel.DataPagamento == null)
                inputModel.DataPagamento = DateTime.Now;
            if (ModelState.IsValid)
            {
                inputModel.GiorniRitardo = _service.DateDiff(inputModel.DataScadenza, inputModel.DataPagamento.Value);
                await _service.EditScadenzaAsync(inputModel);
                TempData["Message"] = "Aggiornamento effettuato correttamente".ToUpper();
                return RedirectToAction(nameof(Index),"Scadenze");
            }
            else
            {
                ViewData["Title"] = "Aggiorna Scadenza".ToUpper();
                inputModel.Beneficiari = _service.GetBeneficiari();
                return View(inputModel);
            }

        }
        [AuthorizeRole(Role.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ScadenzaDeleteInputModel inputModel = new ScadenzaDeleteInputModel();
            inputModel.IdScadenza=id;
            if(ModelState.IsValid)
            {
                String message = await _service.DeleteScadenzaAsync(inputModel);
                if (message != string.Empty)
                {
                    TempData["Message"] = message;
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    TempData["Message"] = "Scadenza eliminato con successo ed email all'amministratore inviata correttamente!";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ViewData["Title"] = "Elimina scadenza";
                return View(inputModel); 
            }
              
        }
        [HttpGet]
        public async Task<IActionResult> SubscribePayPal(int id, string token)
        {
            ScadenzaSubscribeInputModel inputModel = await _service.CapturePaymentAsyncPayPal(id, token);
            await _service.SubscribeScadenzaAsync(inputModel);
            TempData["Message"] = "IMPORTO PAGATO CORRETTAMENTE!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> SubscribeStripe(int id, string token)
        {
            ScadenzaSubscribeInputModel inputModel = await _service.CapturePaymentAsyncStripe(id, token);
            await _service.SubscribeScadenzaAsync(inputModel);
            TempData["Message"] = "IMPORTO PAGATO CORRETTAMENTE!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> PayPayPal(int id)
        {
            
             string paymentUrl = await _service.GetPaymentUrlAsyncPayPal(id);
             return Redirect(paymentUrl);
            
        }

        [HttpGet]
        public async Task<IActionResult> PayStripe(int id)
        {
            string paymentUrl = await _service.GetPaymentUrlAsyncStripe(id);
            return Redirect(paymentUrl);
        }















    }
}