namespace UserService.Models;

public class UserWithBalance
{
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Status { get; set; }
}