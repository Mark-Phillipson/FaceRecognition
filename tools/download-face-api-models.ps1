<#
  PowerShell helper: download face-api.js model files into wwwroot/models
  Usage: run from repository root (FaceRecognition)
    .\tools\download-face-api-models.ps1

  This script fetches the three common models used by this app:
   - ssd_mobilenetv1
   - face_landmark_68
   - face_recognition

  It downloads the manifests and the referenced weight shard bin files from the face-api.js repo.
#>

$modelsDir = "FaceRecognition.Client\wwwroot\models"
if (-not (Test-Path $modelsDir)) { New-Item -ItemType Directory -Path $modelsDir -Force | Out-Null }

$base = 'https://raw.githubusercontent.com/justadudewhohacks/face-api.js/master/weights'
$manifests = @( 'ssd_mobilenetv1_model-weights_manifest.json', 'face_landmark_68_model-weights_manifest.json', 'face_recognition_model-weights_manifest.json' )

Write-Host "Downloading face-api models to $modelsDir from $base"
foreach ($m in $manifests) {
  $url = "$base/$m"
  $out = Join-Path $modelsDir $m
  Write-Host "Fetching $url -> $out"
  try {
    Invoke-WebRequest -Uri $url -OutFile $out -UseBasicParsing -ErrorAction Stop
  } catch {
    Write-Warning "Failed to download manifest $url: $_"
    continue
  }

  # parse manifest and download referenced weight shards
  try {
    $json = Get-Content -Raw -Path $out | ConvertFrom-Json
    foreach ($entry in $json) {
      if ($entry.paths) {
        foreach ($p in $entry.paths) {
          $fileUrl = "$base/$p"
          $dest = Join-Path $modelsDir $p
          if (-not (Test-Path $dest)) {
            Write-Host "  -> fetching $fileUrl"
            try {
              Invoke-WebRequest -Uri $fileUrl -OutFile $dest -UseBasicParsing -ErrorAction Stop
            } catch {
              Write-Warning "    Failed to download $fileUrl: $_"
            }
          } else {
            Write-Host "  -> already exists: $p"
          }
        }
      }
    }
  } catch {
    Write-Warning "Failed to parse manifest $out: $_"
  }
}

Write-Host "Done. Models are in $modelsDir (if downloads succeeded)."