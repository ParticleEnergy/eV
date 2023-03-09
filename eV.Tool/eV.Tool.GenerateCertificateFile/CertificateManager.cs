// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace eV.Tool.GenerateCertificateFile;

public static class CertificateManager
{
    public static void GenerateSelfSignedCertificate(string targetHost, string password, string path = "./")
    {
        // Generate a new RSA key pair
        using RSA rsa = RSA.Create();

        // Create a CertificateRequest with the subject name and the RSA key
        CertificateRequest request = new($"CN={targetHost}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Create a self-signed X.509 certificate
        DateTimeOffset startDate = DateTimeOffset.UtcNow;
        DateTimeOffset endDate = startDate.AddYears(1);
        X509Certificate2 certificate = request.CreateSelfSigned(startDate, endDate);

        // Export the certificate to a PFX file with a password
        byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, password);

        string filePath = Path.Combine(path, "certificate.pfx");
        // Save the PFX file to disk
        File.WriteAllBytes(filePath, pfxBytes);
        Console.WriteLine($"Certificate saved to {filePath}");
    }
}
