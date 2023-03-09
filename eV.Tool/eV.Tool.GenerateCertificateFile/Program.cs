// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.


using eV.Tool.GenerateCertificateFile;

if (args.Length >= 3)
{
    CertificateManager.GenerateSelfSignedCertificate(args[0], args[1], args[2]);
}
else
{
    CertificateManager.GenerateSelfSignedCertificate(args[0], args[1]);
}
