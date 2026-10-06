using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum AuthMethodImap
    {
        //
        // Resumo:
        //     Automatically selects the most-secure authentication mechanism supported by the
        //     server.
        Auto = 0,
        //
        // Resumo:
        //     Login using plaintext password authentication; This is supported by most servers.
        Login = 1,
        //
        // Resumo:
        //     Login using the SASL PLAIN authentication mechanism.
        Plain = 2,
        //
        // Resumo:
        //     Login using the CRAM-MD5 authentication mechanism.
        CramMd5 = 3,
        //
        // Resumo:
        //     Login using the DIGEST-MD5 authentication mechanism.
        DigestMd5 = 4,
        //
        // Resumo:
        //     Login using OAuth via the SASL XOAuth mechanism.
        OAuth = 5,
        //
        // Resumo:
        //     Login using OAuth 2.0 via the SASL XOAUTH2 mechanism.
        OAuth2 = 6,
        //
        // Resumo:
        //     Login using the NTLM authentication mechanism.
        Ntlm = 7,
        //
        // Resumo:
        //     Login using the NTLMv2 authentication mechanism.
        Ntlmv2 = 8,
        //
        // Resumo:
        //     Login using the NTLM/NTLMv2 authentication mechanism via Microsoft's Security
        //     Support Provider Interface (SSPI).
        NtlmOverSspi = 9,
        //
        // Resumo:
        //     Login using Kerberos authentication via the SASL GSSAPI mechanism.
        Gssapi = 10,
        //
        // Resumo:
        //     Login using the SCRAM-SHA-1 authentication mechanism.
        ScramSha1 = 11,
        //
        // Resumo:
        //     Login using the Secure Remote Password (SRP) authentication mechanism.
        //
        // Comentários:
        //     The SRP mechanism is only available when targeting .NET 4.0 or newer.
        Srp = 12
    }
}
