using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rydo.Domain.Entities;

namespace Rydo.Application.Common.Helpers;

public class EncryptedDetailConverter(CryptoHelper crypto)
    : ValueConverter<Detail, string>(v => crypto.Encrypt(v), v => crypto.Decrypt<Detail>(v))
{

}