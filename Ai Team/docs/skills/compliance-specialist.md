# Compliance Specialist — Best Practices

## Overview

The Compliance Specialist ensures the system meets regulatory requirements across multiple frameworks: PDPL (Saudi Arabia), ISO 27001, CBAHI (healthcare accreditation), and HIPAA (US healthcare). It is activated for healthcare, financial, or government projects and for any task involving personal data handling.

**Skill ID:** `compliance-specialist`
**Activated by:** Explicit assignment, or detection of healthcare/government/finance project markers, or tasks mentioning "PDPL", "HIPAA", "ISO 27001", "audit", "data privacy", or "compliance"
**Coordinates with:** App Security Specialist, Data Architect

---

## Regulatory Frameworks

### PDPL — Personal Data Protection Law (Saudi Arabia)

The primary data protection law for projects operating in Saudi Arabia.

| Requirement | Implementation |
|---|---|
| **Lawful basis** | Document the legal basis for processing each category of personal data (consent, contract, legal obligation, vital interests, public task, legitimate interest) |
| **Consent management** | Explicit, informed, revocable consent for non-essential processing. Consent stored with timestamp and version |
| **Data minimization** | Collect only the data necessary for the stated purpose. Regularly audit for over-collection |
| **Purpose limitation** | Data collected for purpose A may not be used for purpose B without new consent |
| **Data subject rights** | Implement: right of access, right to correction, right to erasure (with 30-day SLA), right to data portability |
| **Data retention** | Define and enforce retention periods per data category. Delete/anonymize at end of retention |
| **Cross-border transfer** | Personal data may only be transferred outside Saudi Arabia if adequate protection is ensured |
| **Breach notification** | Notify NDMO (National Data Management Office) within 72 hours of discovering a personal data breach |
| **Privacy by design** | Privacy controls built into systems from the start, not added later |

### ISO 27001 — Information Security Management

| Domain | Key Controls |
|---|---|
| **Asset Management** | Inventory all information assets; classify by sensitivity |
| **Access Control** | Least privilege; MFA for privileged access; quarterly access reviews |
| **Cryptography** | Policy for encryption key management; approved algorithms list |
| **Physical & Environmental** | Data center access controls; clean desk policy; device encryption |
| **Operations Security** | Change management; capacity management; malware protection; backup and recovery |
| **Communications Security** | Network segmentation; TLS for all data in transit |
| **Supplier Relationships** | Third-party risk assessments; contractual security requirements |
| **Incident Management** | Incident response plan; escalation procedures; lessons-learned process |
| **Audit** | Internal audit schedule; management review; corrective action tracking |

### CBAHI — Central Board for Accreditation of Healthcare Institutions (Saudi Arabia)

| Standard | Requirement |
|---|---|
| **Patient identification** | Two-factor patient ID (name + national ID or DOB) before any clinical action |
| **Medical record confidentiality** | Access logs on all medical record views; minimum necessary access principle |
| **Consent documentation** | Informed consent records with version control |
| **Data integrity** | Audit trails on all changes to clinical records; no deletions, only corrections |
| **Availability** | System availability SLAs; defined RTO/RPO for clinical systems; disaster recovery plan |
| **Staff access** | Role-based access aligned to clinical role; revoked within 24 hours of staff departure |

### HIPAA — Health Insurance Portability and Accountability Act (US)

| Rule | Key Requirements |
|---|---|
| **Privacy Rule** | PHI (Protected Health Information) may only be used/disclosed for treatment, payment, or healthcare operations, or with authorization |
| **Security Rule** | Administrative, physical, and technical safeguards for electronic PHI (ePHI) |
| **Breach Notification Rule** | Notify affected individuals within 60 days of discovering a breach involving PHI |
| **Minimum Necessary** | Access and disclose only the minimum PHI necessary for the stated purpose |
| **BAA** | Business Associate Agreements required with all third parties that process PHI |

---

## Best Practices

### Privacy by Design (applies to all frameworks)

- **Data classification first**: before designing any feature that handles personal data, classify every field (PUBLIC / INTERNAL / CONFIDENTIAL / RESTRICTED / PHI).
- **Default to privacy**: features should default to the most privacy-preserving setting. Users should opt in to data sharing, not opt out.
- **Separation of concerns**: separate personal identifiers from health/behavioral data where possible (pseudonymization).
- **Consent management system**: a dedicated service that records consent with: user ID, purpose, version, timestamp, IP, status (active/withdrawn).

### Audit Trail Requirements

All access to and changes of sensitive/regulated data must be logged:

```sql
-- Audit log table structure
CREATE TABLE AuditLog (
    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EntityType   NVARCHAR(100) NOT NULL,     -- 'Patient', 'MedicalRecord'
    EntityId     NVARCHAR(255) NOT NULL,     -- Subject's ID
    Action       NVARCHAR(50)  NOT NULL,     -- 'CREATE', 'READ', 'UPDATE', 'DELETE'
    ChangedBy    NVARCHAR(255) NOT NULL,     -- User performing action
    ChangedAt    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    OldValues    NVARCHAR(MAX) NULL,         -- JSON of before state
    NewValues    NVARCHAR(MAX) NULL,         -- JSON of after state
    IpAddress    VARCHAR(45)   NULL,
    UserAgent    NVARCHAR(500) NULL
);
```

Rules:
- Audit log is **append-only** — no updates or deletes.
- Audit log is stored in a **separate schema or database** from the application data, with restricted write access (application can only INSERT, not UPDATE/DELETE).
- Include `OldValues` and `NewValues` for all UPDATE operations on sensitive tables.

### Data Retention & Deletion

- Document retention periods per data category in the Data Dictionary.
- Implement automated deletion/anonymization jobs that run on schedule.
- For PDPL right-to-erasure requests: anonymize (replace PII with pseudonym) rather than hard-delete when referential integrity must be maintained.
- Log all erasure operations in the Audit Log.

### Access Control

- Role-based access with the **minimum necessary principle** per framework.
- Access reviews on a defined schedule (quarterly for ISO 27001).
- Privileged access requires MFA.
- Access revoked within 24 hours of staff departure (CBAHI requirement).
- Log every access to PHI/PII (not just changes — reads too).

---

## Compliance Review Process

When reviewing a task, check:

### PDPL Checklist

- [ ] Legal basis for processing documented (if new data category)
- [ ] Consent mechanism implemented (if required)
- [ ] Data subject rights endpoints exist (access, correction, erasure)
- [ ] Retention policy defined for new data
- [ ] Cross-border transfer assessed (if applicable)

### ISO 27001 Checklist

- [ ] Data classified by sensitivity
- [ ] Access controls aligned to least privilege
- [ ] Change management documented (migration runbook, rollback plan)
- [ ] Encryption in transit and at rest for classified data

### CBAHI Checklist (healthcare projects)

- [ ] Patient identification is two-factor before clinical data access
- [ ] Medical record audit trail is complete and append-only
- [ ] Consent records stored with version and timestamp
- [ ] RTO/RPO documented for clinical data systems

### HIPAA Checklist (US healthcare projects)

- [ ] PHI access logged (who, when, what)
- [ ] Minimum necessary access enforced
- [ ] BAA in place with all third-party processors
- [ ] PHI encrypted in transit (TLS 1.2+) and at rest

---

## Anti-Patterns

| Anti-Pattern | Compliance Risk | Correct Approach |
|---|---|---|
| Logging PII in application logs | PDPL breach, ISO 27001 violation | Mask/omit PII fields in logs |
| Audit log in same table as data | Log can be tampered with | Separate append-only audit store |
| Storing consent without timestamp | Cannot prove consent | Consent records must have timestamp + version |
| Hardcoded retention ("delete after 5 years") | May conflict with changing regulations | Data retention policy catalog; configurable retention |
| No breach response procedure | Breach notification SLA violation | Defined incident response plan with 72h notification |
| PHI in URLs or query parameters | Logged in server/proxy logs | Use POST body or path segments for PHI identifiers |
| Third-party integrations without data processing agreement | PDPL/HIPAA violation | BAA/DPA required for all PHI processors |

---

## Scrum Team Collaboration

The Compliance Specialist is a **reviewer**, active in the verification phase. It also consults during sprint planning when tasks involve new data handling.

### Review Output Format

```yaml
compliance_review:
  task_id: "reg-002"
  verdict: "CONDITIONAL_PASS"   # PASS | FAIL | CONDITIONAL_PASS
  frameworks_checked: ["PDPL", "CBAHI"]
  findings:
    - severity: "HIGH"
      framework: "PDPL"
      requirement: "Audit trail for patient data access"
      description: "GET /api/patients/{id} does not log the access event"
      recommendation: "Add audit log INSERT on every patient record READ"
    - severity: "MEDIUM"
      framework: "PDPL"
      requirement: "Data minimization"
      description: "API response returns national_id in list endpoint — not required for the list view"
      recommendation: "Remove national_id from GET /api/patients response; include only in detail endpoint"
  conditions:
    - "HIGH finding must be resolved before patient data is live"
  blocking_findings: 1
```

### When to Involve Compliance During Planning

Flag tasks to the Scrum Master for compliance review during planning (not just at verification) when they:
- Introduce new data collection
- Change access control rules
- Modify audit logging
- Add third-party integrations that process PII/PHI
- Implement data export or reporting features

---

## Verification Checklist

- [ ] OWASP Top 10 compliance (delegated to App Security Specialist)
- [ ] All applicable frameworks' checklists completed
- [ ] No CRITICAL or HIGH findings unresolved
- [ ] Audit trail covers all PHI/PII writes and (for HIPAA/CBAHI) reads
- [ ] Data retention policies documented in Data Dictionary
- [ ] Consent mechanism implemented if required
- [ ] Data subject rights endpoints tested
- [ ] Breach notification procedure documented
- [ ] Compliance review verdict recorded in sprint standup
