# Future Work

- Automate environment parity checks so every `config/appsettings.{Environment}.json` file is validated for required keys during CI builds.
- Integrate a secure secrets provider (Azure Key Vault, AWS Secrets Manager, etc.) and wire it into `ConfigurationExtensions` once deployment infrastructure is ready.
- Provide a developer shortcut (PowerShell/Bash script or `just` recipe) that toggles the ASP.NET Core environment for API, UI, and Sentinel in one command.
