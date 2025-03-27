# OpenMediaID

**OpenMediaID** is an open-source .NET library and container format for signing, verifying, and describing media collections in a secure and portable way.

It allows developers, publishers, and teams to:

- Create signed metadata packages for collections of media files
- Verify the integrity and origin of media using embedded public key signatures
- Embed previews, thumbnails, and structured metadata

---

## ✅ Features

- ✍️ Digital signing using RSA private keys
- 🔍 Signature verification using public keys
- 📦 ZIP-based `.medid` container format
- 📄 Metadata stored as human-readable JSON
- 🖼 Supports thumbnails and preview media
- 🛠 .NET 6+ library and NuGet package

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package OpenMediaID
```

Or via Visual Studio NuGet Manager: **OpenMediaID**

---

## 📂 The .medid Container Format

A `.medid` file is a ZIP archive containing:

```
my-collection.medid
├── medid.json         # core metadata file (signed)
├── public.key         # optional: embedded public key
├── info.md            # optional: human-readable info
├── thumbnails/        # optional preview images
└── media/             # optional preview media
```

---

## 🧰 Example Usage

### 🔐 Signing a media package

```csharp
var keyPair = KeyPairGenerator.Generate();
var signed = MedidSigner.Sign(medidFile, keyPair.PrivateKey, "Your Name", "pubkey-2025");
MedidPackage.Save(signed, "package.medid", keyPair.PublicKey);
```

### ✅ Verifying a signed package

```csharp
var medid = MedidPackage.Load("package.medid", out var pubkey);
bool isValid = MedidVerifier.Verify(medid, pubkey);
```

### 🧾 Creating a metadata file

```csharp
var entry = new MediaEntry
{
    Filename = "image.jpg",
    Hash = "sha256:...",
    LengthInBytes = 102400,
    MimeType = "image/jpeg",
    Metadata = new MediaMetadata { Width = 800, Height = 600 },
    ThumbnailPath = "thumbnails/image.jpg"
};

var collection = new MediaCollection
{
    Name = "My Images",
    Publisher = "Example Corp",
    Created = DateTime.UtcNow,
    Entries = new() { entry }
};

var medidFile = new MedidFile
{
    Collection = collection
};
```

---

## 🔒 Key Generation

```csharp
var (priv, pub) = KeyPairGenerator.Generate();
File.WriteAllBytes("private.key", priv);
File.WriteAllBytes("public.key", pub);
```

To securely store private keys:

```csharp
var encrypted = EncryptedKeyManager.EncryptPrivateKey(priv, "MyPassword");
var decrypted = EncryptedKeyManager.DecryptPrivateKey(encrypted, "MyPassword");
```

---

## 📄 License

MIT License © 2025 OpenMedid Project
