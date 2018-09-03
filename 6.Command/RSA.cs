using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Pkcs;
using System.IO;

namespace FakeSSL
{
    class RSA
    {

        static AsymmetricCipherKeyPair ReadKeyfromFile(string filename)
        {
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                StreamReader sr = new StreamReader(file);
                String PemKey = sr.ReadToEnd();
                AsymmetricCipherKeyPair keys = ReadKeyAsPem(PemKey);

                return keys;
                sr.Close();
                file.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        static void WriteKeytoFile(string filename, string pemContent, int type)
        {
            try
            {
                if (type == 0)
                {
                    filename += "_public.key";
                }
                else
                {
                    filename += "_pirvate.key";
                }
                FileStream file = new FileStream(filename, FileMode.Create);
                StreamWriter sw = new StreamWriter(file);
                sw.Write(pemContent);

                sw.Close();
                file.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());

            }
        }

        static void ReadCertificatefromFile(string filename)
        {

        }

        static void WriteCertificatetoFile(string filename, string pemContent)
        {
            try
            {
                filename += ".cer";
                FileStream file = new FileStream(filename, FileMode.Create);
                StreamWriter sw = new StreamWriter(file);
                sw.Write(pemContent);

                sw.Close();
                file.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());

            }
        }


        static string WriteCertificateAsPem(X509Certificate2 cer)
        {
            var Cer = Convert.ToBase64String(cer.GetRawCertData());


            string pemCer = "-----BEGIN CERTIFICATE-----";


            for (int i = 0; i < Cer.Length; ++i)
            {

                if ((i) % 64 != 0)
                {
                    pemCer += Cer[i];

                }
                else
                {
                    pemCer += "\r\n" + Cer[i]; ;
                }

            }
            pemCer += "\r\n-----END CERTIFICATE-----";




            return pemCer;
        }






        static string WriteKeyAsPem(AsymmetricCipherKeyPair pair, int type)
        {
            using (TextWriter textWriter = new StringWriter())
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(textWriter);
                if (type == 0)
                {
                    pemWriter.WriteObject(pair.Public);
                }
                else
                {
                    pemWriter.WriteObject(pair.Private);

                }
                return textWriter.ToString();
            }
        }



        static AsymmetricCipherKeyPair ReadKeyAsPem(string pem)
        {
            using (TextReader reader = new StringReader(pem))
            {
                var obj = new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                return obj as AsymmetricCipherKeyPair;
            }
        }


        public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, string issuerName, AsymmetricKeyParameter issuerPrivKey, int keyStrength = 2048)
        {
            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Signature Algorithm
            const string signatureAlgorithm = "SHA1WithRSA";
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);
            var issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();



            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Generating the Certificate
            var issuerKeyPair = subjectKeyPair;

            // selfsign certificate
            var certificate = certificateGenerator.Generate(issuerPrivKey, random);

            // correcponding private key
            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);


            // merge into X509Certificate2
            var x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());




            var seq = (Asn1Sequence)Asn1Object.FromByteArray(info.PrivateKey.GetDerEncoded());
            if (seq.Count != 9)
                //throw new PemException("malformed sequence in RSA private key");
                return null;

            var rsa = new RsaPrivateKeyStructure(seq);
            RsaPrivateCrtKeyParameters rsaparams = new RsaPrivateCrtKeyParameters(
            rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

            x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);

            WriteKeytoFile(subjectName.Substring(3), WriteKeyAsPem(subjectKeyPair, 0), 0);

            WriteKeytoFile(subjectName.Substring(3), WriteKeyAsPem(subjectKeyPair, 1), 1);

            WriteCertificatetoFile(subjectName.Substring(3), WriteCertificateAsPem(x509));

            Console.WriteLine(WriteKeyAsPem(subjectKeyPair, 0));

            Console.WriteLine(WriteKeyAsPem(subjectKeyPair, 1));

            Console.WriteLine(WriteCertificateAsPem(x509));

            return x509;

        }


        public static AsymmetricKeyParameter GenerateCACertificate(string subjectName, int keyStrength = 2048)
        {
            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Signature Algorithm
            const string signatureAlgorithm = "SHA256WithRSA";
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);
            var issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Generating the Certificate
            var issuerKeyPair = subjectKeyPair;

            // selfsign certificate
            var certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
            var x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());


            // Add CA certificate to Root store
            addCertToStore(x509, StoreName.Root, StoreLocation.CurrentUser);

            return issuerKeyPair.Private;

        }
        public static bool addCertToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
            }
            catch
            {

            }

            return bRet;
        }

        public static void CreateRSA()
        {
            var caPrivKey = ReadKeyfromFile("rooy.key");

            GenerateSelfSignedCertificate("CN=ts.cf", "ST=TS", caPrivKey.Private);






        }
    }
}
