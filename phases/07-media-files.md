# Phase 07: Implement media library, secure uploads, image processing and document storage.

## Objective

Implement media library, secure uploads, image processing and document storage.

## Mandatory process

1. Read `AGENTS.md`.
2. Read the relevant skill files.
3. Inspect current implementation.
4. Produce an implementation plan.
5. Implement only the scope of this phase.
6. Run relevant tests.
7. Run build.
8. Review security.
9. Review SEO where applicable.
10. Update documentation.
11. Update `CHANGELOG.md`.
12. Report files changed, tests run, risks and remaining work.

## Constraints

- Do not invent lawyer information.
- Do not introduce unnecessary dependencies.
- Do not break existing features.
- Do not claim success without verification.

## Deliverables

Document concrete files, code, tests and configuration produced during this phase.

## Exit Criteria

The phase is complete only when its implementation and verification criteria are satisfied.

## Specific tasks
- Reuse Phase 06 MediaAsset/IMediaService/FileImageStorage/UI; do not rebuild the image pipeline (ADR-019).
- Complete retention/recovery/orphan reconciliation, backup/restore and Plesk ACL/native-assets/body-limit checks.
- implement secure filesystem storage
- validate file content/type/size
- sanitize filenames
- create media library
- add image dimensions and alt text
- support documents without exposing unsafe executables

## Phase 07 implementation (2026-10-10)

Preserves ADR-019. ADR-020 records private PDF storage, fail-closed antivirus, retained soft deletion,
bounded reconciliation/temporary-file quarantine and combined backups. See docs/media/MEDIA_FILES.md,
docs/deployment/PLESK_MEDIA_RUNBOOK.md and docs/qa/PHASE_07_REVIEW.md for implementation/validation.
Owner approved closure and commit on 2026-10-10; code acceptance does not imply real MySQL/Plesk/Defender/ACL/restore drill completion.
