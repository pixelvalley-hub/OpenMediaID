using OpenMediaID.Crypto;
using OpenMediaID.Models;
using OpenMediaID.Packaging;
using OpenMediaID.UnitTest.Helper;

namespace OpenMediaID.UnitTest;

public class PackagingTest
{
    [RunnableInDebugOnly]
    public void Test1()
    {
        var entry = new MediaEntry
        {
            Filename = "Major_Update.jpg",
            FilePath = "C:\\temp\\Major_Update.jpg"
        };

        var collection = new MediaCollection
        {
            Name = "My Images",
            Publisher = "Example Corp",
            Created = DateTime.UtcNow,
            Entries = [entry]
        };

        var medidFile = new MedidFile
        {
            Collection = collection
        };

        var keyPair = KeyPairGenerator.Generate();

        SaveOptions options = new SaveOptions
        {
            PublicKey = keyPair.PublicKey,
            PrivateKey = keyPair.PrivateKey,
            SignerName = "Your Name"
        };

        MedidPackage.Save(medidFile, "C:\\temp\\package.medid", options);
        bool result = MedidVerifier.VerifyPackage("C:\\temp\\package.medid");
    }
}