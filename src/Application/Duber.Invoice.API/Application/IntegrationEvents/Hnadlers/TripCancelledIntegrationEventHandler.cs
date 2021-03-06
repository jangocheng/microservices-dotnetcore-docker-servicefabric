﻿using System;
using System.Threading.Tasks;
using Duber.Domain.Invoice.Repository;
using Duber.Domain.SharedKernel.Model;
using Duber.Infrastructure.EventBus.Abstractions;
using Duber.Invoice.API.Application.IntegrationEvents.Events;

namespace Duber.Invoice.API.Application.IntegrationEvents.Hnadlers
{
    public class TripCancelledIntegrationEventHandler : IIntegrationEventHandler<TripCancelledIntegrationEvent>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public TripCancelledIntegrationEventHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }

        public async Task Handle(TripCancelledIntegrationEvent @event)
        {
            var invoice = await _invoiceRepository.GetInvoiceByTripAsync(@event.TripId);
            if (invoice != null) return;

            invoice = new Domain.Invoice.Model.Invoice(
                @event.PaymentMethod.Id,
                @event.TripId,
                @event.Duration,
                0,
                TripStatus.Cancelled.Id);

            await _invoiceRepository.AddInvoiceAsync(invoice);

            // integration with external payment system.
            if (Equals(invoice.PaymentMethod, PaymentMethod.CreditCard) && invoice.Total > 0)
            {

            }
        }
    }
}
