using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Models.Dtos;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.ViewModels.Scadenze;

namespace Scadenzario.Controllers;

[Route("Scadenze")]
public class AnaliticheSpeseController : Controller
{
    private readonly AnaliticheSpeseService _svc;

    public AnaliticheSpeseController(AnaliticheSpeseService svc) => _svc = svc;

    // Pagina HTML con grafico
    [HttpGet("GraficoCategorie")]
    public async Task<IActionResult> GraficoCategorie(
        DateTime? dal, DateTime? al, string? filter, CancellationToken ct)
    {
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
        var data = await _svc.GetTotaliPerCategoriaAsync(dal, al, denominazione, dataScadenza,ct);

        var vm = new GraficoCategorieViewModel
        {
            Labels = data.Select(d => d.Categoria).ToList(),
            Values = data.Select(d => d.Totale).ToList(),
            Dal = dal,
            Al  = al
        };

        return View("GraficoCategorie", vm);
    }
}