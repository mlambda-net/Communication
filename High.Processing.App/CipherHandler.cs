using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using High.Processing.Domain.Events;

namespace High.Processing.App;

public class CipherHandler
{


    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CipherHandler()
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        _key = aes.Key;
        _iv = aes.IV;
    }

    public Task Create(CreateProduct msg)
    {
        var json = JsonSerializer.Serialize(msg);
        Encrypt(json);
        return Task.CompletedTask;
    }


    private string Encrypt(string plainText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = _key;
        aesAlg.IV = _iv;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

}