using SelfCheckout.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfCheckout.Services.Payment
{
    public interface IPaymentService
    {
        Task<string> GeneratePPQrCode(object payload);

        Task<PromptPayResult> InquiryAsync(string refField);
    }
}
