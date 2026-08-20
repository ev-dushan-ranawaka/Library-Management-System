# The second main feature of Scaffolder is Enroll an existing handwritten slice.

That's means registering a function slice that has already manually created by developers into the managed lifecycle of ENKIVEDA Scaffolder.

Now we are going to this process. In here we already have GG.Functions.Product. Under this slice we are going to create Brand capability.

---

## First of all

Make sure to verify scaffolder version

```powershell
enkiveda-scaffold --version

enkiveda-scaffold packs --json --non-interactive
```

```powershell
$workspace = "C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\GG"
```

Then check the repos are clean or not:

```powershell
git -C "$workspace\\gg-microservices" status --short

git -C "$workspace\\gg-local-dev" status --short
```

---

## Next we need export samples:

```powershell
$samples = "C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\scaffolder-samples\\gg-0.5.8"
```

```powershell
enkiveda-scaffold --pack gg samples `
  --out $samples `
  --json --non-interactive
```

This the same gg-sample folder like previous example, only change in this version name.

---

In this feature, first, we need to find exported registered adoption template JSON file. To adopt a slice, we need to copy the exported template and bind the exact paths / hashes of the Product slice.

In samples files this is the registered template file for slice adopt.

```text
C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\scaffolder-samples\\gg-0.5.8\\manifests\\public-v2\\examples\\slice-adopt-registered-template.json
```

Then we need make copy from that template:

```powershell
$workspace = "C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\GG"

$samples = "C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\scaffolder-samples\\gg-0.5.8"

$template = "$samples\\manifests\\public-v2\\examples\\slice-adopt-registered-template.json"

$productAdopt = "$samples\\manifests\\public-v2\\examples\\slice-adopt-product.json"

Copy-Item $template $productAdopt
```

---

Now Copy process done. Then we need to change this according to Product slice

```text
customer          -> product
Customer          -> Product
GG.Functions.Customer -> GG.Functions.Product
```

After that we need to bind this six Product anchor paths, before bind them we need to verify these files are present or not, for that:

```powershell
$anchorChecks = @(
  "$workspace\\gg-microservices\\Functions\\GG.Functions.Product\\GG.Functions.Product.csproj",
  "$workspace\\gg-microservices\\Functions\\GG.Functions.Product\\Program.cs",
  "$workspace\\gg-microservices\\Tests\\GG.Functions.Product.Tests\\GG.Functions.Product.Tests.csproj",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\GG.Local.AppHost.csproj",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\SliceDefinitions.cs",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\Program.cs"
)

$anchorChecks | ForEach-Object {
  [PSCustomObject]@{
      Exists = Test-Path $_
      Path   = $_
  }
}
```

Now we can see all anchor paths are present.

---

Then we get the exact SHA-256 hashes to confirm that the reviewed baseline files have not changed.

- The SHA-256 hash is a fingerprint of the file.
- When adopting a slice, Scaffolder is told, "These existing files are the baseline that we reviewed and approved."

I'm using these commands for get hashes from anchor files:

```powershell
$workspace = "C:\\Users\\Enkiveda\\Desktop\\Gear Guard\\GG"

$anchors = @(
  "$workspace\\gg-microservices\\Functions\\GG.Functions.Product\\GG.Functions.Product.csproj",
  "$workspace\\gg-microservices\\Functions\\GG.Functions.Product\\Program.cs",
  "$workspace\\gg-microservices\\Tests\\GG.Functions.Product.Tests\\GG.Functions.Product.Tests.csproj",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\GG.Local.AppHost.csproj",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\SliceDefinitions.cs",
  "$workspace\\gg-local-dev\\apphost\\GG.Local.AppHost\\Program.cs"
)

$anchors | ForEach-Object {
  $hash = Get-FileHash $_ -Algorithm SHA256

  [PSCustomObject]@{
      Path = $hash.Path
      Hash = $hash.Hash.ToLowerInvariant()
  }
}
```

Now all set up.

---

## Now we able to Dry-run adoption

```powershell
enkiveda-scaffold --pack gg slice adopt `
  --manifest $productAdopt `
  --workspace $workspace `
  --json --non-interactive
```

dry-run operation is success.

---

## Next step is run apply command

```powershell
enkiveda-scaffold --pack gg slice adopt `
  --manifest $productAdopt `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

---

If successful, run doctor next and see if the adoption receipt is valid:

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $productAdopt `
  --workspace $workspace `
  --json --non-interactive
```

We are looking at roughly:

```json
"installed": true,
"receiptStatus": "valid",
"journalPending": false,
"recoveryRequired": false,
"issue": null
```

Doctor also healthy. Product slice adoption stage is officially complete.

Then we can commit this change.

---

After this step, you can work as usual, Like as we did before. That means:

Write business logic and commit

Capability add and rest of the operations.
