# Security Policy

## Supported Versions

| Version | .NET      | Supported          |
| ------- | --------- | ------------------ |
| 3.0.x   | .NET 10.0 | :white_check_mark: |
| 2.0.x   | .NET 8.0  | :x:                |
| 1.0.x   | .NET 7.0  | :x:                |
| < 1.0   | .NET 5.0  | :x:                |

## Reporting a Vulnerability

If you discover a security vulnerability, please **do not** open a public issue.

Instead, please report it by emailing the maintainer directly. You can find contact information on the [GitHub profile](https://github.com/osisdie).

### What to include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

### Response Timeline
- **Acknowledgment**: Within 48 hours
- **Initial assessment**: Within 1 week
- **Fix timeline**: Depends on severity

## Security Considerations

This boilerplate includes sensitive components:
- **JWT Authentication**: Ensure `JwtSettings:Secret` is stored securely (use Secret Manager or environment variables in production)
- **Email Handler**: SMTP credentials (`COREFX_SMTP_PWD`) should never be committed to source control
- **HTTPS**: Always use HTTPS in production environments
