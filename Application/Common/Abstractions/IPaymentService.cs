namespace Application.Common.Abstractions
{
    public  interface IPaymentService
    {
        Task<string> ProcessPayment(string userId, decimal amount, string paymentMethod);
    }
}
