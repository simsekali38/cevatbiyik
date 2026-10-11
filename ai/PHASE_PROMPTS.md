# Phase Prompt Usage

Use each file in `/phases` as a separate implementation prompt.

Recommended workflow:

1. Paste the Master Prompt once.
2. Let the agent complete Phase 00.
3. Review the resulting architecture/docs.
4. Give the Phase 01 prompt.
5. Continue sequentially.
6. Do not skip a phase merely because the agent says it is obvious.
7. At each phase require actual project/tests/build output.

This avoids a single huge prompt causing uncontrolled implementation.
