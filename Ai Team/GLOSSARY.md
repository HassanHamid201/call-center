# Glossary

Definitions for all domain-specific terms used throughout this project's documentation.

---

## A

**ACP (Agent Communication Protocol)**
A standardized messaging format that all agent workers use to communicate with the orchestrator and with each other. Ensures interoperability regardless of agent implementation.

**Acceptance Criteria**
Explicit conditions defined per task that must all pass for the task to be considered complete. Verified independently by the Verification Engine — agents cannot self-certify. Defined in YAML/JSON task definitions or extracted from Markdown task descriptions.

**Active Task Budget**
The portion of the total context window (typically 60%) allocated to the primary agent's execution context, including working memory and skill documentation.

**Allocation Result**
The return value from a budget request — indicates whether the allocation was granted, partially granted, or denied, along with the actual token amount reserved.

---

## B

**Blocker**
An obstacle that prevents an agent from completing its assigned task. Blockers trigger an escalation protocol: the agent first attempts autonomous resolution via paging, then escalates to the orchestrator, and finally to a human if necessary.

**Budget Allocation**
A reserved token quantity assigned to a specific component (orchestrator, agent, review phase). Includes the allocated amount, current utilization, paging sub-allocation, and configured compression strategies.

**Budget Compliance**
Verification that an agent's execution stayed within its allocated token budget. Checked after every execution phase before results are accepted.

---

## C

**Compression**
Automatic reduction of context size when a component approaches or exceeds its token budget. Strategies include summarization (condensing detailed history), tier demotion (moving content to deeper storage), and truncation (removing oldest low-priority context).

**Context Budget**
The total token allocation for a single task execution, divided among the orchestrator, active task, and review phases. Managed by the Context Budgeter component.

**Context Budgeter**
A dedicated subsystem of the Orchestrator Core responsible for allocating token budgets, monitoring utilization, and enforcing overflow protocols.

**Context Window**
The maximum number of tokens an LLM can process in a single request/response cycle. The system treats this as a finite resource that must be budgeted and managed.

---

## D

**Deep Archive (Tier 3 / Cold)**
The cold memory tier containing full project history, execution logs, resolved blockers, domain heuristics, and semantic embeddings. Accessed exclusively through paging operations. Content here has no budget impact until explicitly retrieved.

**Decision Log**
A structured record in Tier 2 (Structured Storage) capturing architectural or implementation decisions, their rationale, and the context in which they were made.

**Delta-Prompt**
A reduced prompt generated after verification failure, containing only the failed requirements and specific feedback (not the entire original task). Each retry uses a delta-prompt, making successive retries progressively cheaper. See [Workflow](docs/workflow/README.md).

---

## E

**Emergency Protocol**
Activated when a component exceeds 110% of its budget. Non-essential operations halt, only task-critical context is preserved, and the orchestrator attempts reallocation or escalates to human intervention.

**Execution Engine**
The subsystem responsible for running agent workers. Manages the lifecycle of agent sessions, provides them with paging toolkits, and reports results back to the orchestrator.

---

## I

**Intent Parsing**
The process by which the orchestrator interprets an incoming task prompt and extracts structured requirements, constraints, and success criteria.

---

## L

**LRU (Least Recently Used)**
The eviction policy used for paged content in Working Memory. When the paging budget is pressured, the least recently accessed memories are removed first.

---

## M

**MemGPT**
A memory-augmented LLM architecture that provides virtual context management through paging operations. The inspiration for this system's autonomous retrieval approach.

**Memory Fragment**
A discrete unit of stored content in any memory tier, identified by a unique memory ID and associated with metadata (tier, creation time, relevance scores, relationships).

**Memory Tier**
One of three storage layers (Working Memory, Structured Storage, Deep Archive) distinguished by access latency, lifetime, and budget impact. See [Memory & Paging](docs/memory/README.md) for details.

---

## O

**Orchestrator Core**
The central coordinating component. Responsible for intent parsing, context budgeting, strategy selection, resource allocation, state management, and workflow control.

**Overflow Handling**
The multi-phase protocol triggered when a component approaches or exceeds its token budget. Progresses from soft warning (80%) through hard limit (100%) to emergency protocol (110%).

---

## P

**Paging**
The process by which agents autonomously retrieve historical context from deeper memory tiers into Working Memory. Analogous to virtual memory paging in operating systems.

**Paging Budget**
A sub-allocation of the Active Task Budget (default 30%) reserved for paging operations. Prevents agents from spending their entire budget on retrieval.

**Paging Log**
A record of all paging operations performed during an agent session, including search queries, retrieved memory IDs, token costs, and effectiveness metrics.

**Paging Toolkit**
The set of tools available to agents for memory retrieval: `page_search`, `page_retrieve`, `page_recurse`, and `page_store`.

**Priority**
A classification used during budget allocation requests (e.g., critical, high, normal, low) that influences whether requests are granted when budgets are constrained.

---

## R

**RALM (Retrieval-Augmented Language Model)**
A pattern where LLMs augment their responses by retrieving relevant information from external sources. This system applies RALM principles to the orchestrator's iterative refinement loop.

**Ralph Loop**
The iterative refinement cycle: implement → test → review → resolve. Named after the "Wreck-It Ralph" fix-it pattern. Tasks cycle through this loop until they pass review or are escalated.

---

## S

**Skill**
An encapsulated capability that can be assigned to an agent. Includes prompts, tools, validation rules, context requirements, and paging behaviors. Defined by a Skill Manifest.

**Skill Manifest**
A structured declaration describing a skill's identity, context requirements, paging capabilities, auto-paging rules, and budget negotiation parameters.

**Structured Storage (Tier 2 / Warm)**
The warm memory tier containing project-scoped structured data: task hierarchies, decision logs, blockers, requirements, and paging indexes. Persistent across sessions with relational query support.

---

## T

**Tier Demotion**
The process of moving content from a higher (hotter) tier to a lower (colder) tier to free Working Memory budget. Typically triggered during overflow handling.

**Tier Promotion**
The process of moving content from a lower tier to a higher tier (via paging) when it becomes relevant to the current task.

**Token**
The basic unit of context measurement. Used by the Context Budgeter to allocate, track, and enforce resource limits across all system components.

---

## W

**Working Memory (Tier 1 / Hot)**
The hot memory tier holding the current task's conversation threads, intermediate calculations, temporary variables, and recently paged data. Zero-latency access, deducted from the Active Task Budget.

---

## V

**Verification Engine**
The subsystem that independently validates agent output against acceptance criteria and project standards (test/lint/build commands). Runs after each agent completes — agents cannot self-certify. See [Verification Engine](docs/verification/README.md).

---

## S

**Specialist Agent**
An agent worker loaded with a specific technology skill (e.g., React, .NET, PostgreSQL) from the Skill Registry. Specialist expertise is injected into the agent's prompt at execution time. See [Skill Registry](docs/skills/README.md).

---

## Related Documentation

- [Architecture Overview](docs/architecture/OVERVIEW.md) — Where these components fit in the system
- [Memory & Paging](docs/memory/README.md) — Detailed tier and paging documentation
- [Context Budgeting](docs/context-budgeting/README.md) — How tokens are managed
