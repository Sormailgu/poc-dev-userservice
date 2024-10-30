namespace UserService.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public UserWithBalance UserWithBalance { get; set; }
    public UserAccountType UserAccountType { get; set; }
}
