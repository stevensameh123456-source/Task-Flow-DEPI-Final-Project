using System.ComponentModel.DataAnnotations;
using TaskFlow.Web.Mock;

namespace TaskFlow.Web.ViewModels.Teams;

public class InviteMemberVM
{
    public int TeamId { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    public MockRole Role { get; set; } = MockRole.Employee;
}