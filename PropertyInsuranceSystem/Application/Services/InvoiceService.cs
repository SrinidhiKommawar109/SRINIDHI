using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public Task<List<Invoice>> GetMyInvoicesAsync(int customerId) =>
        _invoiceRepository.GetByCustomerIdAsync(customerId);
}
