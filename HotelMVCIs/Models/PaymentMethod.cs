namespace HotelMVCIs.Models
{
    public enum PaymentMethod
    {
        Hotovost, // Cash
        Karta,    // Card
        Prevod,   // Bank Transfer
        Online,   // Online Payment (e.g., PayPal, Stripe)
        Faktura,
        Jine      // Other
    }
}