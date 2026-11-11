using System.Security.Cryptography;
using System.Text;

namespace TaskFlow.Services.Features.Invitations.Services;

public class InvitationTokenGenerator
{
    public string GenerateToken()
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public string HashToken(string token)
    {
        byte[] hashBytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hashBytes);
    }
}