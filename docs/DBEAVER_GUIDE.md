# DBeaver Database Management Guide

A comprehensive guide for using DBeaver to analyze SQL query execution strategies, interpret execution plans, and optimize database performance.

## Table of Contents

- [Getting Started with DBeaver](#getting-started-with-dbeaver)
- [Connecting to Databases](#connecting-to-databases)
- [SQL Editor Essentials](#sql-editor-essentials)
- [Execution Plans Explained](#execution-plans-explained)
- [Analyzing Query Performance](#analyzing-query-performance)
- [Common Execution Plan Operations](#common-execution-plan-operations)
- [Performance Optimization Strategies](#performance-optimization-strategies)
- [DBeaver Productivity Tips](#dbeaver-productivity-tips)
- [Database-Specific Features](#database-specific-features)
- [Troubleshooting](#troubleshooting)

---

## Getting Started with DBeaver

### Installation

1. Download from [dbeaver.io](https://dbeaver.io/download/)
2. Choose **DBeaver Community** (free) or **DBeaver Pro** (paid, more features)
3. Install and launch

### Initial Setup

1. **Window → Preferences → General → Appearance** - Choose theme
2. **Window → Preferences → Editors → SQL Editor** - Configure formatting
3. **Window → Preferences → Database → SQL Editor** - Set auto-complete options

---

## Connecting to Databases

### SQLite Connection (Current Project)

1. **Database → New Database Connection** (or Ctrl+Shift+N)
2. Select **SQLite**
3. Click **Browse** and navigate to `src/BookLibrary.Api/booklibrary.db`
4. Click **Test Connection** → **Finish**

### PostgreSQL Connection (Docker)

1. **Database → New Database Connection**
2. Select **PostgreSQL**
3. Configure:
   - **Host**: `localhost`
   - **Port**: `5432`
   - **Database**: `booklibrary`
   - **Username**: `booklibrary`
   - **Password**: `YourSecurePassword123!`
4. Click **Test Connection** → **Finish**

### Connection Settings Best Practices

| Setting | Recommended Value | Purpose |
|---------|-------------------|---------|
| Keep-Alive | 30 seconds | Prevents timeout |
| Auto-commit | OFF (for development) | Control transactions |
| Fetch Size | 200 | Balance memory/speed |
| Read-only | ON (for production) | Prevent accidental changes |

---

## SQL Editor Essentials

### Opening SQL Editor

- **Right-click database → SQL Editor → New SQL Script**
- Or press **F3** with database selected
- Or **Ctrl+]** to open new script

### Keyboard Shortcuts

| Action | Shortcut | Description |
|--------|----------|-------------|
| Execute Statement | **Ctrl+Enter** | Run statement at cursor |
| Execute Script | **Ctrl+Alt+X** | Run entire script |
| Execute with Plan | **Ctrl+Shift+E** | Show execution plan |
| Format SQL | **Ctrl+Shift+F** | Auto-format query |
| Auto-complete | **Ctrl+Space** | Show suggestions |
| Comment Line | **Ctrl+/** | Toggle comment |
| Find/Replace | **Ctrl+F / Ctrl+H** | Search in editor |
| Go to Line | **Ctrl+G** | Jump to line number |
| Duplicate Line | **Ctrl+D** | Copy current line |
| Save Script | **Ctrl+S** | Save SQL file |

### Editor Features

**Auto-complete:**
- Table names, column names, functions
- Configure in Preferences → SQL Editor → Code Completion

**Syntax Highlighting:**
- Keywords, strings, numbers, comments
- Customize colors in Preferences → SQL Editor → Syntax Coloring

**Templates:**
- Type abbreviation + Tab (e.g., `sel` + Tab → `SELECT * FROM`)
- Manage in Preferences → SQL Editor → Templates

---

## Execution Plans Explained

### What is an Execution Plan?

An execution plan shows how the database engine will execute your query:
- Which indexes are used
- Join order and methods
- Row estimates vs actual rows
- Cost estimates for each operation

### Viewing Execution Plan in DBeaver

**Method 1: Explain Button**
1. Write your query
2. Click **Explain Execution Plan** button (or **Ctrl+Shift+E**)
3. View plan in **Execution Plan** tab below results

**Method 2: Manual EXPLAIN**

```sql
-- SQLite
EXPLAIN QUERY PLAN SELECT * FROM Books WHERE AuthorId = 1;

-- PostgreSQL
EXPLAIN SELECT * FROM "Books" WHERE "AuthorId" = 1;

-- PostgreSQL with actual execution statistics
EXPLAIN ANALYZE SELECT * FROM "Books" WHERE "AuthorId" = 1;

-- PostgreSQL verbose with buffers
EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT) 
SELECT * FROM "Books" WHERE "AuthorId" = 1;
```

### Understanding Plan Output

**SQLite EXPLAIN QUERY PLAN:**

```
QUERY PLAN
|--SCAN Books
```

vs.

```
QUERY PLAN
|--SEARCH Books USING INDEX idx_books_authorid (AuthorId=?)
```

**PostgreSQL EXPLAIN ANALYZE:**

```
Seq Scan on "Books"  (cost=0.00..1.15 rows=1 width=100) (actual time=0.015..0.018 rows=2 loops=1)
  Filter: ("AuthorId" = 1)
  Rows Removed by Filter: 10
Planning Time: 0.085 ms
Execution Time: 0.035 ms
```

### Key Metrics to Watch

| Metric | What It Means | Good Values |
|--------|---------------|-------------|
| **Cost** | Arbitrary units of work | Lower is better |
| **Rows** | Estimated row count | Should match actual |
| **Width** | Bytes per row | Smaller is better |
| **Actual Time** | Real execution time (ms) | Lower is better |
| **Loops** | Times operation executed | 1 is ideal |
| **Buffers** | Pages read from disk/cache | More shared hits = better |

---

## Analyzing Query Performance

### Step-by-Step Analysis Process

#### 1. Baseline the Query

```sql
-- PostgreSQL: Get timing and stats
EXPLAIN (ANALYZE, BUFFERS, TIMING) 
SELECT b.*, a."Name" as AuthorName
FROM "Books" b
JOIN "Authors" a ON b."AuthorId" = a."Id"
WHERE b."PublishedYear" > 2000;
```

#### 2. Identify Bottlenecks

Look for:
- **Seq Scan** on large tables → Need index
- **High cost** operations → Optimize or restructure
- **Large row estimates** mismatches → Update statistics
- **Nested Loop** with many rows → Consider different join

#### 3. Check Index Usage

```sql
-- PostgreSQL: See available indexes
SELECT indexname, indexdef 
FROM pg_indexes 
WHERE tablename = 'Books';

-- SQLite: See indexes
SELECT name, sql FROM sqlite_master 
WHERE type = 'index' AND tbl_name = 'Books';
```

#### 4. Compare Before/After

Run EXPLAIN ANALYZE before and after changes to measure improvement.

### Reading Execution Plans Visually

DBeaver displays plans as:
- **Tree view**: Hierarchical operation breakdown
- **Text view**: Raw database output
- **Graphical view** (Pro): Visual diagram

**Tree View Interpretation:**
```
→ Hash Join (cost=1.05..2.30)              ← Outer operation
   → Seq Scan on Authors (cost=0.00..1.05) ← Inner operation 1
   → Hash (cost=1.12..1.12)                ← Inner operation 2
      → Seq Scan on Books                  ← Leaf operation
```

Operations execute from **inside out** (leaves first).

---

## Common Execution Plan Operations

### Scan Operations

| Operation | Description | Performance |
|-----------|-------------|-------------|
| **Seq Scan** | Full table scan, reads every row | Slow for large tables |
| **Index Scan** | Uses index, then fetches rows | Good for selective queries |
| **Index Only Scan** | Uses index only, no table access | Excellent |
| **Bitmap Index Scan** | Builds bitmap from index | Good for multiple conditions |

**When Seq Scan is OK:**
- Small tables (< 1000 rows)
- Selecting most of the table
- No suitable index exists

**When Seq Scan is Bad:**
- Large tables with selective WHERE
- Frequently executed queries

### Join Operations

| Operation | Description | Best For |
|-----------|-------------|----------|
| **Nested Loop** | For each outer row, scan inner | Small datasets, indexed inner |
| **Hash Join** | Build hash table, probe with other | Large datasets, equality joins |
| **Merge Join** | Merge sorted inputs | Pre-sorted data, large datasets |

**Join Performance Tips:**
- Ensure join columns are indexed
- Smaller table should be the inner/build side
- Consider join order hints if needed

### Other Operations

| Operation | Description |
|-----------|-------------|
| **Sort** | Orders rows (uses memory/disk) |
| **Hash** | Builds hash table for joins/aggregates |
| **Aggregate** | GROUP BY, COUNT, SUM, etc. |
| **Limit** | Stops after N rows |
| **Filter** | Applies WHERE conditions |
| **Materialize** | Stores intermediate results |

---

## Performance Optimization Strategies

### 1. Index Optimization

**Identify Missing Indexes:**

```sql
-- PostgreSQL: Find slow queries
SELECT query, calls, mean_time, total_time
FROM pg_stat_statements
ORDER BY mean_time DESC
LIMIT 10;

-- Check if index would help
EXPLAIN ANALYZE SELECT * FROM "Books" WHERE "Isbn" = '978-0-123456-78-9';
```

**Create Targeted Indexes:**

```sql
-- Single column index
CREATE INDEX idx_books_isbn ON "Books" ("Isbn");

-- Composite index (column order matters!)
CREATE INDEX idx_books_author_year ON "Books" ("AuthorId", "PublishedYear");

-- Partial index (PostgreSQL)
CREATE INDEX idx_books_recent ON "Books" ("PublishedYear") 
WHERE "PublishedYear" > 2020;

-- Covering index (includes all needed columns)
CREATE INDEX idx_books_author_covering ON "Books" ("AuthorId") 
INCLUDE ("Title", "Isbn");
```

### 2. Query Optimization

**Avoid SELECT \*:**

```sql
-- Bad
SELECT * FROM "Books" WHERE "AuthorId" = 1;

-- Good
SELECT "Id", "Title", "Isbn" FROM "Books" WHERE "AuthorId" = 1;
```

**Use EXISTS Instead of IN for Large Sets:**

```sql
-- Slower for large subqueries
SELECT * FROM "Authors" 
WHERE "Id" IN (SELECT "AuthorId" FROM "Books");

-- Faster
SELECT * FROM "Authors" a
WHERE EXISTS (SELECT 1 FROM "Books" b WHERE b."AuthorId" = a."Id");
```

**Avoid Functions on Indexed Columns:**

```sql
-- Won't use index
SELECT * FROM "Books" WHERE LOWER("Title") = 'the great gatsby';

-- Will use index (with proper index)
SELECT * FROM "Books" WHERE "Title" ILIKE 'the great gatsby';

-- Or create functional index
CREATE INDEX idx_books_title_lower ON "Books" (LOWER("Title"));
```

### 3. Statistics and Maintenance

**Update Statistics:**

```sql
-- PostgreSQL
ANALYZE "Books";
ANALYZE;  -- All tables

-- SQLite
ANALYZE;
```

**Check Table Bloat (PostgreSQL):**

```sql
SELECT 
    schemaname, tablename,
    pg_size_pretty(pg_total_relation_size(schemaname || '.' || tablename)) as size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname || '.' || tablename) DESC;
```

**Vacuum Tables (PostgreSQL):**

```sql
VACUUM ANALYZE "Books";
VACUUM FULL "Books";  -- Reclaims more space, but locks table
```

### 4. Query Hints (When Needed)

```sql
-- PostgreSQL: Force index scan
SET enable_seqscan = OFF;
SELECT * FROM "Books" WHERE "AuthorId" = 1;
SET enable_seqscan = ON;

-- PostgreSQL: Join order hint
SELECT /*+ Leading(b a) */ *
FROM "Books" b
JOIN "Authors" a ON b."AuthorId" = a."Id";
```

---

## DBeaver Productivity Tips

### Database Navigator Tricks

- **F4**: Open entity editor for selected table
- **Double-click column**: See column details
- **Ctrl+Click**: Open in new tab
- **Drag table to editor**: Insert table name

### Data Viewing

**Filter Data:**
1. Open table data (double-click table)
2. Click filter icon or press **Ctrl+F**
3. Enter condition: `AuthorId = 1`

**Sort Data:**
- Click column header to sort
- Ctrl+Click for multi-column sort

**Edit Data:**
1. Double-click cell to edit
2. **Ctrl+S** to save changes
3. Or click **Save** button

### Comparison Tools

**Compare Data:**
1. Select two tables
2. Right-click → **Compare** → **Compare Data**

**Compare Structures:**
1. Right-click table → **Compare** → **Compare Structure**

### Export/Import

**Export Results:**
1. Run query
2. Right-click results → **Export Data**
3. Choose format (CSV, JSON, SQL, Excel, etc.)

**Generate SQL:**
- Right-click table → **Generate SQL** → INSERT/UPDATE/DELETE

### ER Diagrams

1. Select tables (Ctrl+Click multiple)
2. Right-click → **View Diagrams**
3. Or: **Database → ER Diagram**

### Query History

- **Window → Query Manager** - See all executed queries
- Filter by database, status, time
- Re-run queries from history

### Bookmarks

1. Select code in editor
2. **Edit → Add Bookmark** (or Ctrl+Shift+B)
3. Access via **Window → Bookmarks**

---

## Database-Specific Features

### SQLite

**Execution Plan:**
```sql
EXPLAIN QUERY PLAN SELECT * FROM Books;
```

**Database Info:**
```sql
-- Tables
SELECT name FROM sqlite_master WHERE type='table';

-- Indexes
SELECT name, sql FROM sqlite_master WHERE type='index';

-- Table info
PRAGMA table_info(Books);

-- Index info
PRAGMA index_list(Books);
PRAGMA index_info(idx_books_authorid);
```

**Analyze Performance:**
```sql
-- Enable query stats
.timer on

-- Run query and see time
SELECT * FROM Books WHERE AuthorId = 1;
```

### PostgreSQL

**Detailed Execution Plan:**
```sql
-- Basic plan
EXPLAIN SELECT * FROM "Books";

-- With execution stats
EXPLAIN ANALYZE SELECT * FROM "Books";

-- Full details
EXPLAIN (ANALYZE, BUFFERS, COSTS, TIMING, VERBOSE, FORMAT TEXT)
SELECT * FROM "Books" WHERE "AuthorId" = 1;

-- JSON format (for tooling)
EXPLAIN (ANALYZE, FORMAT JSON) SELECT * FROM "Books";
```

**Performance Views:**
```sql
-- Active queries
SELECT pid, query, state, wait_event_type, query_start
FROM pg_stat_activity
WHERE state != 'idle';

-- Table statistics
SELECT relname, seq_scan, seq_tup_read, idx_scan, idx_tup_fetch
FROM pg_stat_user_tables;

-- Index usage
SELECT indexrelname, idx_scan, idx_tup_read, idx_tup_fetch
FROM pg_stat_user_indexes;

-- Cache hit ratio
SELECT 
    sum(heap_blks_hit) / (sum(heap_blks_hit) + sum(heap_blks_read)) as ratio
FROM pg_statio_user_tables;
```

**Blocking Queries:**
```sql
SELECT 
    blocked.pid AS blocked_pid,
    blocked.query AS blocked_query,
    blocking.pid AS blocking_pid,
    blocking.query AS blocking_query
FROM pg_stat_activity blocked
JOIN pg_stat_activity blocking ON blocking.pid = ANY(pg_blocking_pids(blocked.pid));
```

**Kill Long-Running Query:**
```sql
SELECT pg_cancel_backend(pid);  -- Graceful
SELECT pg_terminate_backend(pid);  -- Force
```

---

## Troubleshooting

### Query is Slow

**Checklist:**
1. ☐ Run EXPLAIN ANALYZE to see actual plan
2. ☐ Check for Seq Scans on large tables
3. ☐ Verify indexes exist on WHERE/JOIN columns
4. ☐ Update statistics (ANALYZE)
5. ☐ Check for lock contention
6. ☐ Review row estimates vs actuals

### Index Not Being Used

**Possible Causes:**
- Table too small (Seq Scan is faster)
- Selecting too many rows (> 10-20% of table)
- Function on indexed column
- Data type mismatch
- Outdated statistics

**Diagnostics:**
```sql
-- Force index usage to compare
SET enable_seqscan = OFF;
EXPLAIN ANALYZE SELECT ...;
SET enable_seqscan = ON;
```

### DBeaver Connection Issues

**Connection Timeout:**
- Increase timeout in connection settings
- Check network/firewall
- Verify database is running

**Driver Issues:**
- **Database → Driver Manager** → Update driver
- Download latest JDBC driver manually

**Memory Issues:**
- Edit `dbeaver.ini`:
  ```
  -Xms512m
  -Xmx2048m
  ```

### Execution Plan Not Showing

1. Ensure query is valid (runs without errors)
2. Try manual EXPLAIN statement
3. Check if database supports execution plans
4. Update DBeaver to latest version

---

## Quick Reference Card

### Essential Shortcuts

| Action | Shortcut |
|--------|----------|
| Execute Query | Ctrl+Enter |
| Explain Plan | Ctrl+Shift+E |
| Format SQL | Ctrl+Shift+F |
| New SQL Script | F3 |
| Auto-complete | Ctrl+Space |
| Find | Ctrl+F |
| Save | Ctrl+S |

### Common EXPLAIN Commands

```sql
-- SQLite
EXPLAIN QUERY PLAN SELECT ...;

-- PostgreSQL (quick)
EXPLAIN SELECT ...;

-- PostgreSQL (detailed)
EXPLAIN (ANALYZE, BUFFERS) SELECT ...;
```

### Performance Red Flags

| Warning Sign | Action |
|--------------|--------|
| Seq Scan on large table | Add index |
| High cost operation | Optimize query |
| Rows estimate ≠ actual | Update statistics |
| Nested Loop with many rows | Review join strategy |
| Sort with high memory | Add index for ORDER BY |

### Index Decision Guide

| Query Pattern | Index Type |
|---------------|------------|
| `WHERE col = value` | B-tree on col |
| `WHERE col1 = v1 AND col2 = v2` | Composite (col1, col2) |
| `WHERE col LIKE 'prefix%'` | B-tree on col |
| `WHERE col IN (...)` | B-tree on col |
| `ORDER BY col` | B-tree on col |
| `WHERE col @> jsonb` | GIN index (PostgreSQL) |
