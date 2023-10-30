using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(Appuser user);
}
