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
        DateTime? dal, DateTime? al, CancellationToken ct)
    {
        var data = await _svc.GetTotaliPerCategoriaAsync(dal, al, ct);

        var vm = new GraficoCategorieViewModel
        {
            Labels = data.Select(d => d.Categoria).ToList(),
            Values = data.Select(d => d.Totale).ToList(),
            Dal = dal,
            Al  = al
        };

        return View("GraficoCategorie", vm);
    }

    // API JSON, utile per fetch lato client
    [HttpGet("/api/scadenze/spese-per-categoria")]
    public async Task<IActionResult> SpesePerCategoriaApi(
        DateTime? dal, DateTime? al, CancellationToken ct)
    {
        List<CategoriaTotaleDto> data = await _svc.GetTotaliPerCategoriaAsync(dal, al, ct);
        return Ok(data);
    }
}