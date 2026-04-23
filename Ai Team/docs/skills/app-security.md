# App Security Specialist — Best Practices

## Overview

The App Security Specialist evaluates and enforces security across all technology layers — frontend, backend, database, and infrastructure. It is **always active** in the verification pipeline for every task, regardless of the assigned implementation specialist.

**Skill ID:** `app-security`
**Always active:** Included in every Scrum Team's reviewer set
**Activated by:** Automatically; always participates in verification
**Coordinates with:** All implementation specialists

---

## OWASP Top 10 — Mandatory Checklist

Every task's output must be evaluated against the current OWASP Top 10:

| # | Risk | Check |
|---|---|---|
| A01 | Broken Access Control | Is every endpoint/action protected with explicit authorization? |
| A02 | Cryptographic Failures | Is sensitive data encrypted in transit and at rest? |
| A03 | Injection | Are all external inputs parameterized/escaped? |
| A04 | Insecure Design | Are security requirements baked in at design, not bolted on? |
| A05 | Security Misconfiguration | Are defaults hardened? Are unnecessary features disabled? |
| A06 | Vulnerable Components | Are dependencies audited? Are known CVEs addressed? |
| A07 | Auth & Session Failures | Is authentication strong? Are sessions managed securely? |
| A08 | Software Integrity Failures | Are build artifacts and deployments verified? |
| A09 | Logging & Monitoring Failures | Are security events logged? Are alerts defined? |
| A10 | SSRF | Are outbound requests validated and restricted? |

---

## Best Practices by Layer

### Frontend Security

- **XSS prevention**: never insert user-supplied content via `innerHTML`, `dangerouslySetInnerHTML`, or `v-html` without DOMPurify sanitization.
- **CSP (Content Security Policy)**: configure restrictive CSP headers. Disallow `unsafe-inline` scripts. Use nonces for legitimate inline scripts.
- **Auth token storage**: JWTs in memory (React state / Pinia store) + refresh token in `httpOnly`, `Secure`, `SameSite=Strict` cookie. Never `localStorage`.
- **CSRF protection**: use `SameSite=Strict` cookies. For legacy setups, use synchronizer token pattern with anti-forgery tokens.
- **Subresource Integrity (SRI)**: add `integrity` and `crossorigin` attributes to externally loaded scripts and stylesheets.
- **Open redirect prevention**: never use user-supplied input as a redirect URL without validating it against an allowlist.
- **Clickjacking**: set `X-Frame-Options: DENY` or CSP `frame-ancestors 'none'`.

### Backend Security

- **Input validation**: validate ALL inputs at the API boundary (FluentValidation, class-validator, Zod). Apply `whitelist: true` / `stripUnknown: true` to reject unexpected fields.
- **Parameterized queries**: all database queries must use parameterized inputs. No string-concatenated SQL.
- **Authentication**: JWT with RS256 (asymmetric signing). Short-lived access tokens (15 min). Refresh token rotation. Revoke on logout.
- **Authorization**: policy-based (claim/permission-based). Never trust client-supplied role data. Re-validate permissions server-side on every request.
- **Rate limiting**: apply rate limits on all authentication endpoints (login, password reset, OTP). Apply global rate limiting on all public endpoints.
- **Security headers**: configure Helmet (Node.js) or security middleware (.NET) to set:
  - `Strict-Transport-Security` (HSTS)
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY`
  - `Content-Security-Policy`
  - `Referrer-Policy: no-referrer`
- **Error responses**: never return stack traces, internal error messages, or system details in production responses. Return opaque error IDs.
- **SSRF prevention**: validate all user-supplied URLs against an allowlist of domains before making outbound HTTP requests.
- **Secrets management**: no secrets in code or config files. Use environment variables, Azure Key Vault, AWS Secrets Manager, or Vault.

### Database Security

- **Least privilege**: application DB user has only `SELECT, INSERT, UPDATE, DELETE` on required tables.
- **Parameterized queries**: enforced by ORM (EF Core, Prisma). Verify no raw SQL with string interpolation.
- **Sensitive data**: PII/PHI fields encrypted at rest (column-level encryption or disk-level TDE).
- **Connection security**: TLS required for all database connections. Reject plaintext.
- **Row-level security**: enable RLS on multi-tenant tables.
- **Audit logging**: enable `pgaudit` (PostgreSQL) or SQL Server Audit for all DML on sensitive tables.

### Infrastructure Security

- **TLS everywhere**: all HTTP traffic is HTTPS. Redirect HTTP → HTTPS. Minimum TLS 1.2 (prefer 1.3).
- **Dependency scanning**: run `npm audit`, `dotnet list package --vulnerable`, or `pip audit` in CI. Block on critical vulnerabilities.
- **Container security**: run containers as non-root. Scan images with Trivy or Snyk in CI.
- **Network segmentation**: database servers are not publicly reachable. API servers communicate with DB via internal network only.

---

## Security Code Review Process

When reviewing any implementation task, evaluate:

### 1. Authentication & Authorization
- [ ] All protected endpoints require authentication
- [ ] Authorization checks are server-side, not client-side
- [ ] JWT signing key is asymmetric (RS256) and stored securely
- [ ] Token expiry is short (< 30 min for access tokens)

### 2. Input Handling
- [ ] All inputs validated at the API boundary
- [ ] All database queries parameterized (no raw SQL with user input)
- [ ] File uploads: type, size, and content validated; stored outside web root
- [ ] User-supplied redirect URLs validated against allowlist

### 3. Output Encoding
- [ ] HTML output encoded (Razor auto-encodes; React/Vue auto-encode JSX)
- [ ] JSON responses do not include sensitive fields (passwords, secrets, internal IDs)
- [ ] Error responses are sanitized — no stack traces in production

### 4. Session & Auth Token Management
- [ ] Auth tokens stored securely (not `localStorage`)
- [ ] Session invalidated on logout
- [ ] Refresh token rotated on use

### 5. Security Headers
- [ ] HSTS configured
- [ ] CSP configured
- [ ] `X-Frame-Options` set
- [ ] `X-Content-Type-Options` set

### 6. Dependencies
- [ ] `npm audit` / `dotnet list package --vulnerable` passes with no critical/high vulnerabilities
- [ ] No packages with known CVEs pulled in transitively

---

## Anti-Patterns

| Anti-Pattern | Risk | Correct Approach |
|---|---|---|
| `localStorage` JWT storage | XSS steals token → account takeover | In-memory + `httpOnly` cookie |
| Secrets in `appsettings.json` | Credential exposure in source control | Environment variables / Key Vault |
| Role-based auth with client-supplied role | Privilege escalation | Server-side role/claim validation |
| Generic error messages showing stack traces | Information disclosure | Opaque error IDs, structured logging |
| `SELECT *` with no field filtering in API response | Over-exposure of sensitive fields | Explicit DTO projections |
| CORS `AllowAnyOrigin` | Cross-origin request from malicious sites | Explicit allowlist of trusted origins |
| Missing rate limiting on auth | Brute force, credential stuffing | Rate limit per IP per endpoint |
| Logging PII/PHI | Compliance violation, data exposure | Mask/omit sensitive fields in logs |

---

## Security Testing

- **Static analysis (SAST)**: run ESLint security plugins, `dotnet-security-guard`, or Semgrep in CI.
- **Dependency audit**: `npm audit --audit-level=high` or equivalent in CI pipeline. Block merges on critical/high.
- **Secret scanning**: run `trufflehog` or GitHub's native secret scanning on every push.
- **Dynamic testing (DAST)**: run OWASP ZAP or Burp Suite scan against the staging environment before each release.
- **Penetration testing**: schedule manual pentests for major features (authentication, payment flows, file uploads).

---

## Scrum Team Collaboration

The App Security Specialist is a **reviewer**, not an implementation agent. It runs in the verification phase after implementation completes.

### Review Output Format

```yaml
security_review:
  task_id: "reg-002"
  verdict: "PASS"        # PASS | FAIL | CONDITIONAL_PASS
  owasp_checks:
    A01_access_control: "PASS"
    A03_injection: "PASS"
    A07_auth_failures: "PASS"
  findings:
    - severity: "HIGH"    # CRITICAL | HIGH | MEDIUM | LOW | INFO
      category: "A02_cryptographic_failures"
      description: "JWT signing uses HS256 (symmetric) — change to RS256"
      recommendation: "Replace symmetric key with RS256 keypair; store private key in Key Vault"
    - severity: "MEDIUM"
      category: "A05_security_misconfiguration"
      description: "HSTS header not configured"
      recommendation: "Add Strict-Transport-Security: max-age=31536000; includeSubDomains"
  blocking_findings: 1    # Number of HIGH/CRITICAL findings (block release if > 0)
```

### Coordinating with Implementation Specialists

- CRITICAL and HIGH findings block the task — the implementation specialist must fix before the sprint can advance.
- MEDIUM findings are tracked but do not block the current sprint — create a follow-up task.
- LOW/INFO findings are logged in the Retrospective Record as improvement suggestions.

---

## Verification Checklist

- [ ] OWASP Top 10 checklist completed for the task scope
- [ ] No CRITICAL or HIGH findings unresolved
- [ ] Auth tokens not in `localStorage`
- [ ] No parameterized query violations
- [ ] Security headers configured
- [ ] Dependency audit passes (no critical/high CVEs)
- [ ] No secrets in source code
- [ ] Error responses sanitized (no stack traces)
- [ ] Security review verdict recorded in sprint standup
