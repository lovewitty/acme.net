using System.IO;
using System.Linq;
using System.Text;
using Oocx.Asn1PKCS.Asn1BaseTypes;
using Oocx.Asn1PKCS.Parser;

namespace Oocx.Asn1PKCS.PKCS1
{
    public class PrivateKeyParser
    {
        private readonly Asn1Parser parser;

        public PrivateKeyParser(Asn1Parser parser)
        {
            this.parser = parser;
        }

        public PrivateKeyParser()
            : this(new Asn1Parser()) { }

        public RSAPrivateKey ParsePem(string pem)
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(pem)))
            {
                return ParsePem(stream);
            }
        }
        
        // This function supports both PKCS#1 & PKCS#8 encodings

        public RSAPrivateKey ParsePem(Stream input)
        {
            var der = PEM.Decode(input, PEM.PrivateKey);

            using (var derStream = new MemoryStream(der))
            {
                // TODO add more validation, ensure that the algorithm used is RSA

                var asn1 = (Sequence)parser.Parse(derStream).First();
                var octet = (OctetString)asn1.Children.Last();
                using (var octetStream = new MemoryStream(octet.UnencodedValue))
                {
                    var rsaParser = new RSAPrivateKeyParser(parser);
                    return rsaParser.ParseDer(octetStream);
                }
            }
        }
    }
}