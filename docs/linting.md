# Linting y formato (ms1)

## Setup individual
1. Instalar extensiones de VS Code (VS Code las sugiere por `.vscode/extensions.json`):
   - `SonarSource.sonarlint-vscode` (SonarLint / SonarQube for IDE)
   - `ms-dotnettools.csharp` + `ms-dotnettools.csdevkit`
   - `EditorConfig.EditorConfig`
2. Activar el hook de git (una vez por clon):
   ```
   git config core.hooksPath .githooks
   ```

## Reglas
- `.editorconfig`: **tabs** (no espacios) para todo menos YAML/JSON/XML/MD. C# usa CRLF (igual que el repo).
- Linter: analizadores Roslyn (`Directory.Build.props` -> `EnforceCodeStyleInBuild`) + `dotnet format`. SonarLint corre en el editor.
- Hook `pre-commit`: ejecuta `dotnet format --verify-no-changes` solo sobre los `.cs` en staging. Si falla, el commit se bloquea.

## Comandos
```
dotnet format GestionClinicaNutricionalService.sln --verify-no-changes   # solo verificar
dotnet format GestionClinicaNutricionalService.sln --include <archivo>   # corregir archivo
```

## Nota
El codigo existente usa 4 espacios. El hook solo valida archivos tocados; migrar todo a tabs es un commit aparte.
