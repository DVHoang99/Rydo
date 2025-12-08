using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace Rydo.Application.Common.Helpers;

public class CryptoHelper
{
    private readonly IDataProtector _protector;

    public CryptoHelper(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("PaymentDetail.Detail");
    }

    public string Encrypt(object input)
    {
        var json = JsonSerializer.Serialize(input);
        return _protector.Protect(json);
    }

    public T Decrypt<T>(string encrypted)
    {
        var json = _protector.Unprotect(encrypted);
        return JsonSerializer.Deserialize<T>(json);
    }
}