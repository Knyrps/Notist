# Notist Production Build Script
# Creates a self-contained executable with all dependencies

param(
    [string]$OutputPath = "production-dist",
    [switch]$SkipClean = $false,
    [switch]$Force = $false
)

Write-Host "=== Notist Production Build ===" -ForegroundColor Green
Write-Host ""

# Function to safely remove directory
function Remove-DirectorySafely {
    param([string]$Path)
    
    if (Test-Path $Path) {
        Write-Host "Removing existing build directory..." -ForegroundColor Gray
        try {
            # Try to stop any running processes that might lock files
            $processes = Get-Process -Name "Notist" -ErrorAction SilentlyContinue
            if ($processes) {
                Write-Host "Found running Notist processes. Stopping them..." -ForegroundColor Yellow
                $processes | Stop-Process -Force -ErrorAction SilentlyContinue
                Start-Sleep -Seconds 2
            }
            
            Remove-Item -Recurse -Force $Path -ErrorAction Stop
            Write-Host "Previous build cleaned successfully." -ForegroundColor Green
        } catch {
            if ($Force) {
                Write-Host "Force flag set. Creating new output directory..." -ForegroundColor Yellow
                $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
                $script:OutputPath = "$OutputPath-$timestamp"
                Write-Host "Using output path: $OutputPath" -ForegroundColor Cyan
            } else {
                Write-Host "Warning: Could not fully clean previous build directory." -ForegroundColor Yellow
                Write-Host "You may need to close running applications or use -Force flag." -ForegroundColor Yellow
                $continue = Read-Host "Continue anyway? (y/N)"
                if ($continue -ne "y" -and $continue -ne "Y") {
                    exit 1
                }
            }
        }
    }
}

# Function to build frontend
function Build-Frontend {
    Write-Host "1. Building frontend..." -ForegroundColor Yellow
    
    if (-not (Test-Path "Notist.Frontend/package.json")) {
        Write-Host "Error: Frontend package.json not found!" -ForegroundColor Red
        exit 1
    }
    
    Set-Location "Notist.Frontend"
    
    # Check if node_modules exists, install if needed
    if (-not (Test-Path "node_modules")) {
        Write-Host "Installing frontend dependencies..." -ForegroundColor Gray
        npm install
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Frontend dependency installation failed!" -ForegroundColor Red
            Set-Location ".."
            exit 1
        }
    }
    
    # Build frontend
    npm run build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Frontend build failed!" -ForegroundColor Red
        Set-Location ".."
        exit 1
    }
    
    Set-Location ".."
    Write-Host "✓ Frontend build completed." -ForegroundColor Green
}

# Function to build backend
function Build-Backend {
    Write-Host "2. Building backend..." -ForegroundColor Yellow
    
    # Clean first
    dotnet clean "Notist/Notist.csproj" --configuration Release --verbosity quiet
    
    # Build
    dotnet build "Notist/Notist.csproj" --configuration Release --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Backend build failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✓ Backend build completed." -ForegroundColor Green
}

# Function to publish self-contained
function Publish-SelfContained {
    param([string]$OutputPath)
    
    Write-Host "3. Publishing self-contained executable..." -ForegroundColor Yellow
    
    dotnet publish "Notist/Notist.csproj" `
        --configuration Release `
        --runtime win-x64 `
        --self-contained true `
        --output $OutputPath `
        --verbosity minimal `
        --property:PublishReadyToRun=true
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Publish failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✓ Self-contained publish completed." -ForegroundColor Green
}

# Function to create documentation
function Create-Documentation {
    param([string]$OutputPath)
    
    Write-Host "4. Creating documentation..." -ForegroundColor Yellow
    
    $readmePath = "$OutputPath/README.md"
    $buildDate = Get-Date -Format "MMMM d, yyyy"
    $buildTime = Get-Date -Format "HH:mm:ss"
    
    $readmeContent = @"
# Notist - Production Distribution

## Build Information
- **Build Date**: $buildDate at $buildTime
- **Version**: Self-contained with CORS fixes
- **Framework**: .NET 8.0
- **Frontend**: Vue 3.5.18 + TypeScript
- **Architecture**: win-x64 self-contained

## What's Included
✅ **Complete Application**: WPF desktop app with embedded Vue.js frontend  
✅ **All Dependencies**: .NET runtime, WebView2, and all required libraries  
✅ **Frontend Assets**: Built and optimized Vue 3 application  
✅ **CORS Fix**: Virtual host mapping prevents browser security errors  
✅ **No Installation Required**: Run directly without dependencies  

## Quick Start
1. Run ``Notist.exe`` directly - no installation needed!
2. Configure settings via system tray or global hotkeys
3. Start taking notes with the overlay interface

## Features
- **Global Hotkeys**: Customizable key combinations with live recording
- **Settings System**: Real-time configuration with restart warnings
- **Modern UI**: Vue 3 responsive interface with organized components
- **System Integration**: System tray, notifications, overlay mode

## Configuration
Settings stored in: ``%APPDATA%\Notist\settings.json``

## Technical Details
- **CORS Resolution**: WebView2 virtual host mapping (https://notist.local/)
- **Self-contained**: ~200MB including full .NET runtime
- **Production Optimized**: Compressed assets, ReadyToRun compilation

---
*Generated by Notist Production Build System*
"@
    
    Set-Content -Path $readmePath -Value $readmeContent
    Write-Host "✓ Documentation created." -ForegroundColor Green
}

# Function to verify and summarize build
function Verify-Build {
    param([string]$OutputPath)
    
    Write-Host "5. Verifying build..." -ForegroundColor Yellow
    
    $exePath = "$OutputPath/Notist.exe"
    if (Test-Path $exePath) {
        $fileInfo = Get-Item $exePath
        $totalSize = (Get-ChildItem -Recurse $OutputPath | Measure-Object -Property Length -Sum).Sum
        
        Write-Host "✓ Build verification successful!" -ForegroundColor Green
        Write-Host "  Executable: $($fileInfo.FullName)" -ForegroundColor Cyan
        Write-Host "  Main exe size: $([math]::Round($fileInfo.Length / 1MB, 2)) MB" -ForegroundColor Cyan
        Write-Host "  Total distribution: $([math]::Round($totalSize / 1MB, 1)) MB" -ForegroundColor Cyan
        
        # Check for critical files
        $criticalFiles = @(
            "$OutputPath/Resources/Frontend/index.html",
            "$OutputPath/Resources/Frontend/assets"
        )
        
        Write-Host ""
        Write-Host "Critical components:" -ForegroundColor Cyan
        foreach ($file in $criticalFiles) {
            if (Test-Path $file) {
                Write-Host "  ✓ $([System.IO.Path]::GetFileName($file))" -ForegroundColor Green
            } else {
                Write-Host "  ✗ $([System.IO.Path]::GetFileName($file)) - MISSING!" -ForegroundColor Red
            }
        }
        
        Write-Host ""
        Write-Host "✓ Production build complete!" -ForegroundColor Green
        Write-Host "Run with: .\$OutputPath\Notist.exe" -ForegroundColor Yellow
        
        return $true
    } else {
        Write-Host "✗ Build verification failed - executable not found!" -ForegroundColor Red
        return $false
    }
}

# Main execution flow
try {
    # Step 0: Prepare output directory
    if (-not $SkipClean) {
        Remove-DirectorySafely -Path $OutputPath
    }
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    
    # Step 1: Build frontend
    Build-Frontend
    
    # Step 2: Build backend
    Build-Backend
    
    # Step 3: Publish
    Publish-SelfContained -OutputPath $OutputPath
    
    # Step 4: Create documentation
    Create-Documentation -OutputPath $OutputPath
    
    # Step 5: Verify
    $success = Verify-Build -OutputPath $OutputPath
    
    if ($success) {
        Write-Host ""
        Write-Host "🚀 Build completed successfully!" -ForegroundColor Green
        Write-Host "Distribution ready at: $OutputPath" -ForegroundColor Cyan
    } else {
        exit 1
    }
    
} catch {
    Write-Host ""
    Write-Host "✗ Build failed with error:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
