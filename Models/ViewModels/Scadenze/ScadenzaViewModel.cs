using System.ComponentModel.DataAnnotations;
using Scadenzario.Models.Entities;
using Scadenzario.Models.Entity;
using Scadenzario.Models.ViewModels.Beneficiari;

namespace Scadenzario.Models.ViewModels.Scadenze;

public class ScadenzaViewModel
{
    public int IDScadenza { get; set; }
    public String IDUser { get; set; }
    public int IDBeneficiario { get; set; }
    public string Denominazione { get; set; }
    [DataType(DataType.Date)]
    public DateTime DataScadenza { get; set; }
    public Decimal Importo { get; set; }
    public bool Sollecito { get; set; }
    public int? GiorniRitardo { get; set; }
    [DataType(DataType.Date)]
    public DateTime? DataPagamento { get; set; }
    public string? Status { get; set; }

    public static ScadenzaViewModel FromEntity(Scadenza scadenza)
    {
        return new ScadenzaViewModel
        {
            IDScadenza = scadenza.IDScadenza,
            IDBeneficiario = scadenza.IDBeneficiario,
            IDUser = scadenza.IDUser,
            Denominazione = scadenza.Denominazione,
            DataScadenza = scadenza.DataScadenza,
            DataPagamento = scadenza.DataPagamento,
            Importo = scadenza.Importo,
            GiorniRitardo = scadenza.GiorniRitardo,
            Sollecito = scadenza.Sollecito,
            Status =scadenza.Status
        };
    }
}