Okay, let's look at the flow of adding a Function and running Aspire from the Slice foundation, this is the first main feature of Scaffolder.

First you need to install Scaffolder latest approved version, at the moment approved version is 0.5.8.

---

There are 3 steps to follow before installing.

1. 1st is you must need Enkiveda Azure DeVos access, Lishan will give you an invitation for that.
2. Then you need to confirm that the .NET SDK version is 10.0.302. - dotnet --version
3. Last step is, you need to install the tool for Azure Artifacts credential provider, you can sign in with your Microsoft account.

If the all 3 steps complete you able to install ENKIVEDA.Scaffolder

### Note them:

* enkiveda-scaffold → Scaffolder CLI
* --json → Give the output in JSON format
* --non-interactive → Run the command without asking for user input.

First you need to assign Release NuGet feed URL into PowerShell variable. Then you can call that variable anytime using $feed.

```powershell
$feed = "https://pkgs.dev.azure.com/ENKIVEDA/enkiveda-tools/_packaging/enkiveda-tools@Release/nuget/v3/index.json"
```

Then run the tool install command

```powershell
dotnet tool install ENKIVEDA.Scaffolder `
  --global `
  --version 0.5.8 `
  --source $feed `
  --interactive
```

After installation complete verify the installation

```powershell
enkiveda-scaffold --version
```

`enkiveda-scaffold packs --json --non-interactive` (Its showing 2 packes, In Gear Guard pack version is 0.6.6. The packs command does not generate code. It only to inspect what architecture packs are available for the scaffolder.)

`enkiveda-scaffold --pack gg --help` (The command shows the commands available for the Gear Guard pack and how to use them. you can check them when you are using the tool)

Okay, Scaffolder installation and verification part is done.

---

Next you need to ready your GG repositories in one workspace and make sure to clean the repositories

```text
C:\Users\Enkiveda\Desktop\Gear Guard\GG\
├── gg-microservices\
└── gg-local-dev\
```

Then I will assign my workspace path into powershell veriable

```powershell
$workspace = "C:\Users\Enkiveda\Desktop\Gear Guard\GG"
```

Next we need to export samples for GG pack. Samples means that, official examples + templates + schemas that you should use for GG work.

I'm going to export samples into my workspace.

```powershell
enkiveda-scaffold --pack gg samples --out "$workspace\gg-samples"
```

Then check the generated samples:

```powershell
Get-ChildItem "$workspace\gg-samples"
```

Your folder now looks roughly like this:

```text
GG
└── gg-samples
    ├── manifests
    ├── asset-index.json
    └── README.md
```

#### Manifest?

The manifest is the JSON instruction file that tells Scaffolder "what operation I want to perform, where/how I want to perform it."

In the manifest folder you can see schemas, examples and templates.

#### asset-index.json?

asset-index.json is a list of assets files in the bundle and classification information. We don't need to worry about that.

#### README.md?

README.md is a task guide on how to use this exported bundle.

When you are trying the Scaffolder after export the gg-samples make sure to read the README.md

* `*.schema.json` files — rules (You don't usually edit these. These are the rules that define whether the manifest is valid.)

`C:\Users\Enkiveda\Desktop\Gear Guard\GG\gg-samples\manifests\public-v2\examples` (These are the example references)

* `slice-create-*.json` — Create a new slice
* `slice-adopt-*.json` — Adopt an existing slice into the Scaffolder lifecycle
* `function-add-*.json` — Expose an existing business handler with an HTTP Function
* `capability-add-*.json` — connect the business capability to the technical spine

---

Okay, Now I'm going to do is, Generate complete function slice called ProductBrand:

### These are the steps I'm following

1. ProductBrand slice foundation
2. Canonical business sources
3. capability add
4. Application business behavior
5. function add
6. Run the Aspire

Just remember that, I will explain them while processing.

---

## 01 - Generate ProductBrand slice foundation

### Step 01 -

So, first check repositories clean or not, I will use these commands:

```powershell
git -C "$workspace\gg-microservices" status --short
git -C "$workspace\gg-local-dev" status --short
```

The outputs should be empty.

### Step 02 -

Now we need to create a slice-create manifest for ProductBrand, because we are going to create new function slice called ProductBrand.

In Here, we already exported samples. I'm select the example that capability-wise closest for ProductBrand. I'm select this slice-create-tenders.json file.

Then I will create manifest folder under my workspace using this command:

```powershell
$manifestDir = "$workspace\manifests"
```

```powershell
New-Item -ItemType Directory -Force -Path $manifestDir | Out-Null
```

Now I'm going to create product brand manifest file under the manifest folder:

```powershell
$manifest = "$manifestDir\product-brand-slice.json"
```

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\slice-create-tenders.json" `
  $manifest
```

Product-brand-slice.json was generated.

Now we need to changes these tenders values according to Product-brand

Done:

```json
{
  "$schema": "../slice-create.schema.json",
  "schemaVersion": 1,
  "kind": "enkiveda.public-v2.gg.slice.create",
  "id": "product-brand",
  "version": "1.0.0",
  "slice": {
    "name": "ProductBrand",
    "routeBase": "v1/product-brands",
    "errorPrefix": "PBR"
  },
  "capabilities": {
    "persistence": {
      "kind": "sql-server",
      "schema": "ProductBrands"
    },
    "messaging": "azure-service-bus",
    "tenancy": "product-context"
  },
  "aspire": {
    "resourceName": "product-brands",
    "profiles": [ "data" ]
  }
}
```

### Step 03 -

Now we able to run this slice create dry run, this is the first actual Scaffolder operation.

A dry run is a preview of what changes will occur if the command is applied, without changing any files.

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

In here its check manifest is valid, check workspace is compatible, what are the files will change, check any conflicts or error and then give json result.

Output is Ok now.

In this output you should review 3 main things:

1. operation
2. dryRun
3. changes

For exactly check that I will extract the output using this command:

This is the veriable:

```powershell
$preview = enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive | ConvertFrom-Json
```

```powershell
$preview | Select-Object operation, dryRun, isNoOp
```

1. operation - This tells Scaffolder what operation is going to be performed now.
2. dryRun - Scaffolder didn't make these changes. It just previewed what was going to be done.
3. isNoOp - This indicates whether there is an actual change from this operation or not. False means - There are changes. If you apply, the files/wiring will change.
4. changes - This is the list of files and registrations that will be created/updated if Scaffolder is actually applied.

For check changes I'm using this command:

```powershell
$preview.changes | Format-Table repositoryId, kind, relativePath
```

After review done we can move to --apply command.

### Step 04 -

This is the command that use for apply changes, the only change is this --apply flag:

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

### Step 05 -

After apply process done, wen need to run exact same --apply command for check any changes after that.

This ensure Same manifest + same repository state will not generate unnecessary changes again.

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

The expected result for changed: false.

### Step 06 -

The run the doctor command to verify that the workspace is in a healthy state after the scaffolding operation is applied.

This is the command for doctor run:

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

### Step 07 -

Now the last step of ProductBrand slice foundation. Review the generated ProductBrand foundation and commit into the branch

We have generated ProductBrand function slice here, and generated foundation files. I'm not going to review files one by one.

I'm going to commit them into both microservice and local-dev branches.

* `feat(ProductBrand): add scaffolder-generated slice foundation`
* `feat(ProductBrand): add scaffolder-generated product-brand slice wiring`

after commit done, verify there is no changes in current branches:

```powershell
git -C "$workspace\gg-microservices" status
git -C "$workspace\gg-local-dev" status
```

Now we completed first main step of our process.

---

## 02 - Business logic implementation

In this second main step we are going to implement business logic that required for generate capability.

For perform that we have 4 canonical business sources, such as:

1. Entity
2. Repository Interface
3. Repository Implementation
4. EF Configuration

In our case we are going to create brand capability under the ProductBrand function slice.

We need to implement Brands.cs, IBrandRepository.cs, BrandRepository.cs, BrandConfiguration.cs and BrandErrors.cs files

These 4 files are developer-owned. Scaffolder does not generate them; they are validated at capability add time to ensure they are in the exact paths and follow the required contracts.

Also make sure to write test cases according to that, In this session I'm not going to write test cases because of the time.

I have already prepared business logic implementation for this session, I will use them now.

Now this is the required domain and infrastructure implementations with simple validation related to the brand.

```text
GG.Functions.ProductBrand
├── Domain
│   └── Brands
│       ├── Brand.cs
│       ├── IBrandRepository.cs
│       └── Errors
│           └── BrandErrors.cs
│
└── Infrastructure
    └── Persistence
        ├── BrandRepository.cs
        └── Configuration
            ├── BrandConfiguration.cs
            └── BrandSchema.cs
```

Now I'm going to commit those changes.

* `feat(ProductBrand): add implemented brand business logic`

after commit done, verify there is no changes in current branch:

```powershell
git -C "$workspace\gg-microservices" status
```

Now, business logic implementation part is done.

---

## 03 - capability add

3rd main step is, capability add. We need to add the capability to officially register the Brand business code you created manually to the Scaffolder lifecycle.

### Step 01 -

I'm are going to copy canonical spine example from exported sample and edit according it to Brands.

The canonical spine example is capability-add-customer-contact-spine.json.

This is the command for copy it into my manifests folder:

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\capability-add-customer-contact-spine.json" `
  "$workspace\manifests\product-brand-capability.json"
```

I have already edited product-brand-capability.json file. I will use it here.

```json
{
  "$schema": "../capability-add.schema.json",
  "schemaVersion": 2,
  "kind": "enkiveda.public-v2.gg.capability.add",
  "id": "brand",
  "version": "1.0.0",
  "slice": {
    "id": "product-brand",
    "name": "ProductBrand"
  },
  "capabilities": {
    "persistence": {
      "kind": "sql-server",
      "schema": "dbo"
    },
    "messaging": "azure-service-bus",
    "tenancy": "product-context"
  },
  "aggregate": {
    "name": "Brand",
    "pluralName": "Brands"
  },
  "mode": "spine-only",
  "spine": {
    "entity": {
      "path": "Functions/GG.Functions.ProductBrand/Domain/Brands/Brand.cs",
      "type": "GG.Functions.ProductBrand.Domain.Brands.Brand"
    },
    "repositoryContract": {
      "path": "Functions/GG.Functions.ProductBrand/Domain/Brands/IBrandRepository.cs",
      "type": "GG.Functions.ProductBrand.Domain.Brands.IBrandRepository"
    },
    "repositoryImplementation": {
      "path": "Functions/GG.Functions.ProductBrand/Infrastructure/Persistence/BrandRepository.cs",
      "type": "GG.Functions.ProductBrand.Infrastructure.Persistence.BrandRepository"
    },
    "configuration": {
      "path": "Functions/GG.Functions.ProductBrand/Infrastructure/Persistence/Configuration/BrandConfiguration.cs",
      "type": "GG.Functions.ProductBrand.Infrastructure.Persistence.Configuration.BrandConfiguration"
    },
    "dbSetProperty": "Brands"
  }
}
```

Okay, Now JSON is correct.

### Step 02 -

Next step is run the capability add dry run:

For that I'm using this variable for define manifest file:

```powershell
$manifest = "$workspace\manifests\product-brand-capability.json"
```

Then run capability add command:

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

Now dry-run is complete.

1. `"ok": true` ✅ — The manifest and workspace validation passed.
2. `"operation": "capability.add"` ✅ — This is the exact operation we intended to run.
3. `"manifestId": "brand"` ✅ — The capability manifest ID is `brand`.
4. `"slice": "ProductBrand"` ✅ — The capability is targeting the `ProductBrand` slice.
5. `"dryRun": true` ✅ — No files have been written yet.
6. `"isNoOp": false` ✅ — Since this is the first run, there are changes available to apply.
7. `"planDigest": "cebd..."` ✅ — This digest uniquely identifies the exact planned mutation.
8. `"function": null` ✅ — This is a capability step, so no HTTP Function is generated.

### Step 03 -

Next step is apply command:

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

Ops, look like I miss something. Let me check the what is the issue. I think its formatting issue in Brand.cs file. Line (21,22)

I will use dotnet format command and let see.

Ah okay, this is the problem. Lets commit it.

`fix(ProductBrand): fix GGS-PUBLIC-VERIFY-002 formatting issue`

And check branch is clear:

```powershell
git -C "$workspace\gg-microservices" status
```

### Step 04 -

Output is ok, now run the same apply command like previous

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

Make sure to check always "changed": false in second apply command.

### Step 05 -

Then run doctor:

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

Perfect. This `doctor` output confirms a healthy state. ✅

* `"installed": true` → The Brand capability is installed.
* `"receiptStatus": "valid"` → The Scaffolder receipt is valid.
* `"journalPending": false` → There is no interrupted or pending mutation.
* `"recoveryRequired": false` → No recovery action is required.
* `"issue": null` → Doctor did not detect any issues.

Expected changes should only be technical spine wiring related. If they are correct, commit them.

I'm going to commit them now.

`feat(ProductBrand): add ProductBrand brand capability wiring`

And check branch is clear:

```powershell
git -C "$workspace\gg-microservices" status
```

Now capability phase also completed.

---

## 04 - Application business behavior

Our next major phase is function add. Before that we need to implement Application handlers and Mapping behaviors. This also Developer owns implementations

In this phase I'm going to implement:

```text
GG.Functions.ProductBrand
├── Application
│   └── Brands
│       └── Commands
│           └── CreateBrand
│               ├── CreateBrandCommand.cs
│               ├── CreateBrandCommandHandler.cs
│               └── CreateBrandResult.cs
│
├── Functions
│   └── Brands
│       ├── BrandCommandExtensions.cs
│       └── BrandCallerContext.cs
│
└── Presentation
    └── Models
        └── Brands
            └── CreateBrandRequest.cs
```

I'm going to commit them now.

`feat(ProductBrand): add application handlers and mapping behaviors`

And check branch is clear:

```powershell
git -C "$workspace\gg-microservices" status
```

Now this phase also completed.

---

## 05 - Function add

In this phase we are doing author the Function manifest and run function add.

We need to get the function-add example matching your HTTP behavior from Fresh Samples.

The example is function-add-product-context.json.

### Step 01 -

Now I'm going to copy it into my manifests folder:

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\function-add-product-context.json" `
  "$workspace\manifests\brand-function.json"
```

Then open it, and then we need to edit according to ProductBrand function slice and Brand capability.

I already prepared the brand-function.json file and I'm going to use it here.

### Step 02 -

After that run the function add dry run command:

before that assign the brand-function.json path into variable:

```powershell
$functionManifest = "$workspace\manifests\brand-function.json"
```

then run dry-run:

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --json --non-interactive
```

### Step 03 -

Then run the apply command:

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

### Step 04 -

After success this run the apply command:

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

### Step 04 -

Then run the doctor:

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $functionManifest `
  --workspace $workspace `
  --json --non-interactive
```

After review generated changes we can commit them. I'm going to commit them

`feat(ProductBrand): add scaffolder-generated create brand function`

And check branch is clear:

```powershell
git -C "$workspace\gg-microservices" status
```

Now all the main steps are completed.

---

## 06 - Run the Aspire

Final step is run the aspire for check ProductBrand slice:

```powershell
$env:GG_APPHOST_PROFILE = "core"
$env:GG_APPHOST_SLICES = "product-brands"
```

```powershell
dotnet run --project `
  "$workspace\gg-local-dev\apphost\GG.Local.AppHost\GG.Local.AppHost.csproj" `
  --configuration Release
```
