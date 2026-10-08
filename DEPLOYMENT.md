# Plesk Deployment

## Target

Windows Server + Plesk.

## Application

ASP.NET Core MVC published output.

## Runtime

Preferred .NET 10 LTS.
Fallback .NET 8 LTS if hosting requires it.

## Pre-deployment

1. Confirm runtime.
2. Confirm ASP.NET Core hosting bundle/module.
3. Confirm MySQL connectivity.
4. Configure production settings.
5. Configure connection string securely.
6. Configure writable upload directories.
7. Backup database.
8. Backup uploads.
9. Publish application.
10. Apply migrations using controlled deployment procedure.
11. Test HTTPS.
12. Test admin.
13. Test forms.
14. Test sitemap and robots.
15. Test error pages.

## Domain

Use one canonical host.

Redirect alternative host to canonical host.

## Files

`wwwroot/uploads` must be writable by the application identity.

## Production configuration

- Debug disabled
- detailed errors disabled
- HTTPS enabled
- secure cookies
- production logging
- secrets outside source control
