# Interface Contracts

Formal definitions for the system's major interfaces. Technology-agnostic — any implementation language can follow these contracts.

---

## Task Interface

The unified task structure that all source formats normalize to.

```typescript
interface Task {
  /** Unique identifier (line number for MD, index for YAML/JSON, issue number for GitHub) */
  id: string;
  
  /** Short task description */
  title: string;
  
  /** Full task details (from GitHub body, YAML description, etc.) */
  body?: string;
  
  /** Parallel group: same value = run concurrently. 0 or undefined = sequential */
  parallelGroup?: number;
  
  /** Whether the task is completed */
  completed: boolean;
  
  /** Which specialist skill to activate. Auto-detected if omitted */
  specialist?: string;
  
  /** Conditions that must ALL pass for the task to be considered complete */
  acceptance_criteria?: string[];
  
  /** Task IDs that must complete before this task starts */
  dependencies?: string[];
}
```

### Task Source Interface

```typescript
type TaskSourceType = "markdown" | "markdown-folder" | "yaml" | "json" | "github";

interface TaskSource {
  /** Which format this source reads from */
  type: TaskSourceType;
  
  /** Get all incomplete tasks */
  getAllTasks(): Promise<Task[]>;
  
  /** Get the next task to execute */
  getNextTask(): Promise<Task | null>;
  
  /** Mark a task as complete (updates source file in-place) */
  markComplete(id: string): Promise<void>;
  
  /** Count remaining tasks */
  countRemaining(): Promise<number>;
  
  /** Count completed tasks */
  countCompleted(): Promise<number>;
  
  /** Get all tasks in a parallel group */
  getTasksInGroup?(group: number): Promise<Task[]>;
  
  /** Flush any cached writes to disk */
  flush(): Promise<void>;
  
  /** Clean up resources */
  dispose(): void;
}
```

### Cached Task Source

Wraps any TaskSource to reduce file I/O:

```typescript
interface CachedTaskSource extends TaskSource {
  /** Inner source being cached */
  inner: TaskSource;
  
  /** Batch all completion writes and flush once */
  flush(): Promise<void>;
}
```

---

## Project Configuration

```typescript
interface ProjectConfig {
  /** Project metadata */
  project: {
    name: string;
    language: string;       // Auto-detected
    framework: string;      // Auto-detected
    description?: string;
  };
  
  /** Verification commands */
  commands: {
    test: string;           // e.g., "npm test"
    lint: string;           // e.g., "npm run lint"
    build: string;          // e.g., "npm run build"
  };
  
  /** Rules injected into every agent prompt */
  rules: string[];
  
  /** Files/patterns agents must never modify */
  boundaries: {
    never_touch: string[];
  };
  
  /** Execution settings */
  execution: {
    max_retries: number;          // default: 3
    retry_delay: number;          // seconds, base for exponential backoff
    max_parallel: number;         // concurrent agents
    default_isolation: "sandbox" | "worktree";
    skip_tests: boolean;
    skip_lint: boolean;
  };
  
  /** Webhook notifications */
  notifications: {
    discord_webhook?: string;
    slack_webhook?: string;
    custom_webhook?: string;
  };
}
```

### Auto-Detection Result

```typescript
interface DetectedProject {
  name: string;
  language: string;       // "TypeScript", "C#", "Python", "Go", "Rust"
  framework: string;      // "Next.js, React", "ASP.NET Core", etc.
  testCmd: string;        // Auto-detected from package.json scripts
  lintCmd: string;
  buildCmd: string;
}
```

---

## Context Budget Interface

Manages token allocation, utilization tracking, and overflow handling.

```typescript
interface ContextBudget {
  /** Unique identifier for this budget instance */
  budgetId: string;
  
  /** Total tokens available in the context window */
  totalTokens: number;
  
  /** Per-component token allocations */
  allocations: Map<ComponentId, BudgetAllocation>;
  
  /** Emergency reserve tokens */
  reserve: number;
  
  /** Current budget health status */
  status: "active" | "exhausted" | "emergency";
  
  /** Request tokens for a component */
  requestAllocation(
    componentId: ComponentId,
    requestedTokens: number,
    priority: Priority
  ): AllocationResult;
  
  /** Release a component's allocation */
  releaseAllocation(componentId: ComponentId): void;
  
  /** Initiate compression on a component */
  triggerCompression(
    componentId: ComponentId,
    strategy: CompressionStrategy
  ): CompressionResult;
  
  /** Activate emergency protocol system-wide */
  enterEmergencyMode(): EmergencyContext;
}

interface BudgetAllocation {
  componentId: ComponentId;
  allocatedTokens: number;
  utilizedTokens: number;
  pagingQuota: number;
  pagingUtilized: number;
  compressionStrategies: CompressionStrategy[];
}

type ComponentId = "orchestrator" | "active-task" | "review";
type Priority = "critical" | "high" | "normal" | "low";
type CompressionStrategy = "summarize" | "tier-demote" | "truncate";

interface AllocationResult {
  granted: boolean;
  allocatedTokens: number;
  reason?: string;
  remainingCapacity: number;
}

interface CompressionResult {
  success: boolean;
  tokensFreed: number;
  strategyUsed: CompressionStrategy;
  affectedContent: string[];
}

interface EmergencyContext {
  activatedAt: Date;
  preservedContext: string[];
  lostContext: string[];
  actionsTaken: string[];
  recoverable: boolean;
}
```

---

## Paging Tool Interface

The four tools available to agents for autonomous memory retrieval.

```typescript
interface PagingToolkit {
  // Programmatic API — agents invoke these as tools named page_search, page_retrieve, etc.
  /** Semantic search across Structured Storage and Deep Archive. Returns metadata only. */
  search(query: string, options: SearchOptions): Promise<SearchResult[]>;
  
  /** Fetch full content into Working Memory. Deducted from paging quota. */
  retrieve(memoryIds: string[], options: RetrieveOptions): Promise<RetrievedMemory[]>;
  
  /** Follow relationship chains between memories */
  recurse(
    startId: string,
    relationshipType: RelationshipType,
    depth: number
  ): Promise<RecursionResult>;
  
  /** Save content to deeper tiers. Frees Working Memory budget. */
  store(
    content: string,
    targetTier: MemoryTier,
    options: StoreOptions
  ): Promise<string>;
  
  /** Check remaining paging quota */
  getQuotaStatus(): QuotaStatus;
  
  /** Request additional quota with justification */
  requestQuotaIncrease(
    justification: string,
    requestedAmount: number
  ): Promise<QuotaAdjustmentResult>;
}

interface SearchOptions {
  resultLimit?: number;           // default: 5
  relevanceThreshold?: number;    // 0-1, default: 0.7
  tiers?: MemoryTier[];
  contentTypes?: string[];
  timeRange?: { start?: Date; end?: Date };
  scopes?: string[];              // Skill-specific memory scopes
}

interface SearchResult {
  memoryId: string;
  tier: MemoryTier;
  contentType: string;
  summary: string;
  relevanceScore: number;
  estimatedTokens: number;
  timestamp: Date;
  relationships: string[];
}

interface RetrieveOptions {
  includeRelationships?: boolean;
  maxTokens?: number;
}

interface RetrievedMemory {
  memoryId: string;
  content: string;
  metadata: { type: string; tier: MemoryTier; timestamp: Date; tags: string[] };
  relatedIds: string[];
  tokensUsed: number;
}

type MemoryTier = "hot" | "warm" | "cold";
type RelationshipType = "resolved_by" | "depends_on" | "related_to" | "supersedes" | "referenced_by";

interface RecursionResult {
  nodes: { memoryId: string; summary: string; tier: MemoryTier; content?: string }[];
  edges: { source: string; target: string; type: RelationshipType }[];
  depthReached: number;
  tokensUsed: number;
}

interface StoreOptions {
  contentType?: string;
  tags?: string[];
  relationships?: { targetId: string; type: RelationshipType }[];
  retentionPolicy?: "permanent" | "standard" | "temporary";
}

interface QuotaStatus {
  totalQuota: number;
  utilized: number;
  remaining: number;
  operationCount: number;
  maxOperations: number;
}

interface QuotaAdjustmentResult {
  granted: boolean;
  additionalQuota: number;
  newTotal: number;
  reason?: string;
}
```

---

## Verification Interface

```typescript
interface VerificationEngine {
  /** Run the full verification pipeline for a task */
  verify(task: Task, options: VerificationOptions): Promise<VerificationResult>;
}

interface VerificationOptions {
  /** Commands from project config */
  commands: {
    test: string;
    lint: string;
    build: string;
  };
  
  /** Working directory to run commands in */
  workDir: string;
  
  /** Whether to run specialist review */
  runSpecialistReview: boolean;
  
  /** Budget remaining for review phase */
  reviewBudget: number;
}

interface VerificationResult {
  /** Overall pass/fail */
  passed: boolean;
  
  /** Per-criterion results */
  checks: CheckResult[];
  
  /** Parsed test output */
  testSummary: {
    total: number;
    passed: number;
    failed: number;
    skipped: number;
    coverage?: number;
  };
  
  /** Lint error count */
  lintErrors: number;
  
  /** Whether build succeeded */
  buildSuccess: boolean;
  
  /** Specialist review (if run) */
  review?: ReviewReport;
}

interface CheckResult {
  /** The original criterion text */
  criterion: string;
  
  /** How it was verified */
  method: "automated" | "review";
  
  /** Whether it passed */
  passed: boolean;
  
  /** Evidence (e.g., "exit code 0", "coverage 85.3%", review notes) */
  evidence: string;
}

interface ReviewReport {
  verdict: "pass" | "fail" | "blocked";
  dimensions: {
    correctness: DimensionResult;
    completeness: DimensionResult;
    patterns: DimensionResult;
    security: DimensionResult;
    performance: DimensionResult;
    testQuality: DimensionResult;
  };
  feedback: string[];
  suggestions: string[];
}

interface DimensionResult {
  passed: boolean;
  notes: string;
}
```

---

## Agent Output Interface

```typescript
interface AgentOutput {
  taskId: string;
  status: "completed" | "failed" | "blocked";
  deliverables: Artifact[];
  testResults: TestReport;
  blockers?: Blocker[];
  metrics: ExecutionMetrics;
  pagingLog: PagingOperation[];
  memoryUpdates: MemoryFragment[];
  budgetUtilization: {
    executionTokens: number;
    pagingTokens: number;
    compressionEvents: number;
    withinBudget: boolean;
  };
}

interface Artifact {
  path: string;
  content: string;
  type: "code" | "test" | "documentation" | "configuration" | "other";
}

interface TestReport {
  total: number;
  passed: number;
  failed: number;
  skipped: number;
  coverage?: number;
  results: { name: string; status: "passed" | "failed" | "skipped"; error?: string; duration: number }[];
}

interface Blocker {
  blockerId: string;
  description: string;
  category: "missing_knowledge" | "access_denied" | "dependency" | "ambiguity" | "technical";
  escalationLevel: number;
  attemptedResolution: string;
}

interface ExecutionMetrics {
  durationMs: number;
  apiCalls: number;
  toolInvocations: number;
  filesRead: number;
  filesWritten: number;
}

interface PagingOperation {
  tool: "page_search" | "page_retrieve" | "page_recurse" | "page_store";
  input: Record<string, unknown>;
  resultSummary: string;
  tokenCost: number;
  timestamp: Date;
  effective: boolean;
}

interface MemoryFragment {
  memoryId: string;
  tier: MemoryTier;
  contentType: string;
  summary: string;
}
```

---

## Skill Manifest Interface

```typescript
interface SkillManifest {
  skillId: string;
  name: string;
  version: string;
  description: string;
  
  /** Technology expertise to inject into the agent prompt */
  expertisePrompt: string;
  
  /** Technologies this specialist covers */
  technologies: string[];
  
  /** Patterns the specialist follows and enforces */
  patterns: string[];
  
  /** Quality standards for this specialist */
  standards: string[];
  
  /** Auto-detection rules */
  detection: {
    files?: string[];          // e.g., ["package.json"]
    dependencies?: string[];   // e.g., ["react", "next"]
    frameworks?: string[];     // e.g., ["React", "Next.js"]
  };
  
  /** Context requirements */
  contextProfile: {
    baselineTokens: number;
    optimalTokens: number;
    pagingTriggers: string[];
  };
  
  /** Which paging operations this specialist uses */
  pagingCapabilities: {
    searchable: boolean;
    retrievable: boolean;
    recursive: boolean;
    storable: boolean;
  };
  
  /** Automatic paging behaviors */
  autoPagingRules: {
    onBlocker: string[];
    onAmbiguity: string[];
    maxAutoRetrieve: number;
  };
  
  /** Budget requirements */
  budgetRequest: {
    minimum: number;
    recommended: number;
    pagingAllowance: number;   // percentage
  };
  
  /** Verification commands (override project defaults) */
  verification?: {
    testCommand?: string;
    lintCommand?: string;
    buildCommand?: string;
  };
}
```

---

## Telemetry Interface

```typescript
interface Session {
  sessionId: string;
  timestamp: number;
  engine: string;           // Which AI engine was used
  mode: "sequential" | "parallel" | "single";
  
  totalTokensIn: number;
  totalTokensOut: number;
  totalDurationMs: number;
  taskCount: number;
  successCount: number;
  failedCount: number;
  
  toolCalls: {
    toolName: string;
    callCount: number;
    successCount: number;
    failedCount: number;
    avgDurationMs: number;
  }[];
  
  tags?: string[];
}
```

---

## Next Steps

- [Context Budgeting](../context-budgeting/README.md) — How budgets are managed
- [Memory & Paging](../memory/README.md) — How paging tools work in practice
- [Skills](../skills/README.md) — The specialist skill catalog
- [Workflow](../workflow/README.md) — Where these interfaces are used
