# Migration Policy

- EF Core migrations are source-controlled.
- Review generated SQL for destructive changes.
- Production migrations are applied in controlled deployment.
- Backup before destructive schema changes.
- Never manually change production schema without recording the decision.
