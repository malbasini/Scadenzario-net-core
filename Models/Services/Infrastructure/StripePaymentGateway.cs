
namespace Scadenzario.Models.Services.Infrastructure;


using System;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Options;

public class StripePaymentGateway : IPaymentGatewayStripe
{
    private readonly IOptionsMonitor<StripeOptions> options;

    public StripePaymentGateway(IOptionsMonitor<StripeOptions> options)
    {
        this.options = options;
    }
    
    public async Task<string> GetPaymentUrlAsyncStripe(ScadenzaPayInputModel inputModel)
    {
        SessionCreateOptions sessionCreateOptions = new()
        {
            ClientReferenceId = $"{inputModel.IdScadenza}/{inputModel.UserId}",
            LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions()
                    {
                        Name = inputModel.Description,
                        Amount = Convert.ToInt64(inputModel.Price * 100),
                        Currency = "EUR",
                        Quantity = 1
                    }
                },
            Mode = "payment",
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                CaptureMethod = "manual"
            },
            PaymentMethodTypes = new List<string>
                {
                    "card"
                },
            SuccessUrl = inputModel.ReturnUrl + "?token={CHECKOUT_SESSION_ID}",
            CancelUrl = inputModel.CancelUrl
        };

        RequestOptions requestOptions = new()
        {
            ApiKey = options.CurrentValue.PrivateKey
        };

        SessionService sessionService = new();
        Session session = await sessionService.CreateAsync(sessionCreateOptions, requestOptions);
        return session.Url;
    }

    public async Task<ScadenzaSubscribeInputModel> CapturePaymentAsyncStripe(string token)
    {
        try
        {
            RequestOptions requestOptions = new()
            {
                ApiKey = options.CurrentValue.PrivateKey
            };

            SessionService sessionService = new();
            Session session = await sessionService.GetAsync(token, requestOptions: requestOptions);

            PaymentIntentService paymentIntentService = new();
            PaymentIntent paymentIntent = await paymentIntentService.CaptureAsync(session.PaymentIntentId, requestOptions: requestOptions);

            string[] customIdParts = session.ClientReferenceId.Split('/');
            int IdScadenza = int.Parse(customIdParts[0]);
            string userId = customIdParts[1];

            return new ScadenzaSubscribeInputModel
            {
                IdScadenza = IdScadenza,
                UserId = userId,
                Paid = paymentIntent.Amount / 100m,
                TransactionId = paymentIntent.Id,
                PaymentDate = paymentIntent.Created,
                PaymentType = "Stripe"
            };
        }
        catch (Exception exc)
        {
            throw new PaymentGatewayException(exc);
        }
    }
}