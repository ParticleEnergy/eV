// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See LICENSE file in the project root for full license information.

using System.Text;
namespace eV.Routing
{
    public static class Encoder
    {
        private static readonly Encoding s_encoding = Encoding.UTF8;
        public static Encoding GetEncoding()
        {
            return s_encoding;
        }
    }
}
