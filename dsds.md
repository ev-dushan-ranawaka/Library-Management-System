# ENKIVEDA Scaffolder — ProductBrand Command Runbook

## 1. Prerequisites and Scaffolder Installation

```powershell
dotnet --version
```

```powershell
$feed = "https://pkgs.dev.azure.com/ENKIVEDA/enkiveda-tools/_packaging/enkiveda-tools@Release/nuget/v3/index.json"
```

```powershell
dotnet tool install ENKIVEDA.Scaffolder `
  --global `
  --version 0.5.8 `
  --source $feed `
  --interactive
```

```powershell
enkiveda-scaffold --version
```

```powershell
enkiveda-scaffold packs --json --non-interactive
```

```powershell
enkiveda-scaffold --pack gg --help
```

---

## 2. Workspace Setup

```text
C:\Users\Enkiveda\Desktop\Gear Guard\GG\
├── gg-microservices\
└── gg-local-dev\
```

```powershell
$workspace = "C:\Users\Enkiveda\Desktop\Gear Guard\GG"
```

```powershell
git -C "$workspace\gg-microservices" status --short
git -C "$workspace\gg-local-dev" status --short
```

---

## 3. Export GG Samples

```powershell
enkiveda-scaffold --pack gg samples --out "$workspace\gg-samples"
```

```powershell
Get-ChildItem "$workspace\gg-samples"
```

```text
GG
└── gg-samples
    ├── manifests
    ├── asset-index.json
    └── README.md
```

---

# 01 — ProductBrand Slice Foundation

## Step 01 — Verify Repositories

```powershell
git -C "$workspace\gg-microservices" status --short
git -C "$workspace\gg-local-dev" status --short
```

## Step 02 — Create Slice Manifest

```powershell
$manifestDir = "$workspace\manifests"
New-Item -ItemType Directory -Force -Path $manifestDir | Out-Null
```

```powershell
$manifest = "$manifestDir\product-brand-slice.json"
```

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\slice-create-tenders.json" `
  $manifest
```

### product-brand-slice.json

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

## Step 03 — Slice Create Dry Run

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

```powershell
$preview = enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive | ConvertFrom-Json
```

```powershell
$preview | Select-Object operation, dryRun, isNoOp
```

```powershell
$preview.changes | Format-Table repositoryId, kind, relativePath
```

## Step 04 — Apply Slice Create

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 05 — Re-run Apply

```powershell
enkiveda-scaffold --pack gg slice create `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 06 — Doctor

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

## Step 07 — Commit Generated Foundation

```text
feat(ProductBrand): add scaffolder-generated slice foundation
```

```text
feat(ProductBrand): add scaffolder-generated product-brand slice wiring
```

```powershell
git -C "$workspace\gg-microservices" status
git -C "$workspace\gg-local-dev" status
```

---

# 02 — Business Logic Implementation

## Required Files

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

## Commit

```text
feat(ProductBrand): add implemented brand business logic
```

```powershell
git -C "$workspace\gg-microservices" status
```

---

# 03 — Capability Add

## Step 01 — Create Capability Manifest

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\capability-add-customer-contact-spine.json" `
  "$workspace\manifests\product-brand-capability.json"
```

### product-brand-capability.json

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

## Step 02 — Capability Add Dry Run

```powershell
$manifest = "$workspace\manifests\product-brand-capability.json"
```

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

## Step 03 — Apply Capability Add

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 04 — Re-run Apply

```powershell
enkiveda-scaffold --pack gg capability add `
  --manifest $manifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 05 — Doctor

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $manifest `
  --workspace $workspace `
  --json --non-interactive
```

## Commit

```text
feat(ProductBrand): add ProductBrand brand capability wiring
```

```powershell
git -C "$workspace\gg-microservices" status
```

---

# 04 — Application Business Behavior

## Required Files

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

## Commit

```text
feat(ProductBrand): add application handlers and mapping behaviors
```

```powershell
git -C "$workspace\gg-microservices" status
```

---

# 05 — Function Add

## Step 01 — Create Function Manifest

```powershell
Copy-Item `
  "$workspace\gg-samples\manifests\public-v2\examples\function-add-product-context.json" `
  "$workspace\manifests\brand-function.json"
```

## Step 02 — Function Add Dry Run

```powershell
$functionManifest = "$workspace\manifests\brand-function.json"
```

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --json --non-interactive
```

## Step 03 — Apply Function Add

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 04 — Re-run Apply

```powershell
enkiveda-scaffold --pack gg function add `
  --manifest $functionManifest `
  --workspace $workspace `
  --apply `
  --json --non-interactive
```

## Step 05 — Doctor

```powershell
enkiveda-scaffold --pack gg doctor `
  --manifest $functionManifest `
  --workspace $workspace `
  --json --non-interactive
```

## Commit

```text
feat(ProductBrand): add scaffolder-generated create brand function
```

```powershell
git -C "$workspace\gg-microservices" status
```

---

# 06 — Run Aspire

```powershell
$env:GG_APPHOST_PROFILE = "core"
$env:GG_APPHOST_SLICES = "product-brands"
```

```powershell
dotnet run --project `
  "$workspace\gg-local-dev\apphost\GG.Local.AppHost\GG.Local.AppHost.csproj" `
  --configuration Release
```
