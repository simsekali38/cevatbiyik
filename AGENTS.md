# AGENTS.md

## Project identity

Av. Cevat Bıyık — Turkish personal legal website.

## Mandatory rules

1. Read relevant files under `/skills` before implementing a related feature.
2. Do not invent lawyer information.
3. Do not expose secrets.
4. Do not add unnecessary dependencies.
5. Prefer server-side rendered Razor.
6. Keep public site and admin in one ASP.NET Core application.
7. Keep the database on MySQL 8.
8. Keep files on hosting filesystem unless requirements change.
9. Do not introduce a subdomain without a documented reason.
10. Do not create an API unless needed.
11. Update documentation when architecture changes.
12. Update `DECISIONS.md` for meaningful architectural decisions.
13. Test before claiming completion.

## Content placeholders

Use `CONTENT_REQUIRED` for unknown facts.

## Production rule

Never deploy untested migrations, secrets, debug settings, fake content, or unreviewed legal claims.

## Phase workflow (owner-confirmed)

- Root guides, project-manifest.json, docs/, phases/ and skills/ are the project contract.
- For a phase-number request, first review Phase 00, current ADRs, accepted risks, dependency direction and technology/hosting constraints.
- Resolve the guide using project-manifest.json and phases/PHASE_ORDER.md; a legacy guide with the same number is not automatically completed.
- Inspect relevant skills/acceptance criteria, implement modular code/tests and update CHANGELOG.md and meaningful ADRs/docs.
- Run build/test and applicable format/migration checks; report commands/results and environment-dependent skips explicitly.
- Finish the active requested scope; wait for the owner's instruction before starting another phase.
