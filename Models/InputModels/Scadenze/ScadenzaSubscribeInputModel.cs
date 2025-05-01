using System;
using System.ComponentModel.DataAnnotations;

namespace Scadenzario.Models.InputModels.Scadenze;

public class ScadenzaSubscribeInputModel
{
    [Required]
    public int IdScadenza { get; set; }
    [Required]
    public string? UserId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentType { get; set; }
    public decimal Paid { get; set; }
    public string TransactionId { get; set; }
}