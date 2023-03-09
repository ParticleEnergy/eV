// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eV.Tool.GenerateCertificateFile;

public static class CertificateManager
{
    public static void GenerateSelfSignedCertificate(string targetHost, string password, string path = "./")
    {
        RSA rsa = RSA.Create(2048);
        // 创建证书请求
        CertificateRequest csr = new($"CN={targetHost}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // 生成证书
        X509Certificate2 cert = csr.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));

        // 导出证书到PFX文件
        byte[] pfxBytes = cert.Export(X509ContentType.Pfx, password);

        string filePath = Path.Combine(path, "cert.pfx");
        File.WriteAllBytes(filePath, pfxBytes);
    }
}
