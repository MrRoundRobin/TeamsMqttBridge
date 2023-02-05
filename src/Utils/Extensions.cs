using System.Security.Cryptography;
using System.Text;

namespace Ro.Teams.MqttBridge.Utils;

internal static class Extensions
{
    private static readonly byte[] optionalEntropy = { 5, 2, 23, 18, 10, 93, 9, 31, 12, 91, 14 };

    public static string Encrypt(this string data)
        => Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(data), optionalEntropy, DataProtectionScope.CurrentUser));

    public static string Decrypt(this string data)
        => Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(data), optionalEntropy, DataProtectionScope.CurrentUser));
}
