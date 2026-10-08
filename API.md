# API Strategy

## Current state

No separate public API is required.

The application is server-rendered MVC.

## Future

If an API becomes necessary:

`/api/v1/`

within the same application.

Requirements:
- DTO-only public contracts
- authentication
- authorization
- rate limiting
- validation
- versioning
- OpenAPI
- auditability
- secure CORS only when needed

Do not build an API merely because modern projects often have one.
