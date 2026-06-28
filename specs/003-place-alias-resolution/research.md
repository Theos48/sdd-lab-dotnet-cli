# Research: Place Alias Resolution

## Decision: Keep IANA timezones as canonical values

**Decision**: Every supported alias and Mexican postal code resolves to one
canonical IANA timezone identifier. The resolver may accept simpler inputs, but
all local date/time, UTC offset, time-difference, and working-hours behavior
continues to use the resolved IANA timezone.

**Rationale**: The existing CLI already uses IANA timezone identifiers as the
stable source of truth. Preserving that model avoids geocoding ambiguity and
keeps comparison behavior deterministic.

**Alternatives considered**:

- Store offsets directly: rejected because offsets change with daylight saving
  and date-specific timezone rules.
- Store geographic coordinates: rejected because the feature does not require
  geocoding and would add unnecessary complexity.

## Decision: Extend the local versioned catalog

**Decision**: Continue using `src/TimezoneCli/Data/place-aliases.json` as the
only source of curated aliases and selected Mexican postal codes. Add explicit
metadata to distinguish city aliases from Mexican postal-code entries.

**Rationale**: The repository already has a local catalog and loader. Extending
that file preserves offline reproducibility, reviewable changes, and simple
test fixtures.

**Alternatives considered**:

- External geocoding or postal-code APIs: rejected because v1 must remain
  offline and deterministic.
- Embedded hardcoded entries in source code: rejected because a versioned data
  file is easier to review and validate.
- Database-backed catalog: rejected as extra infrastructure outside scope.

## Decision: Selected Mexican postal codes for v1

**Decision**: Support exactly `01000`, `64000`, and `44100` in v1:

- `01000` -> `America/Mexico_City`
- `64000` -> `America/Monterrey`
- `44100` -> `America/Mexico_City`

All other five-digit Mexican postal-code-like inputs remain unsupported.

**Rationale**: A small fixed seed set demonstrates the feature without implying
complete postal-code coverage. It is also easy to test and maintain.

**Alternatives considered**:

- Support only `01000`: rejected as too narrow for validating multiple timezone
  mappings.
- Support a broad postal-code list: rejected because it would imply geocoding
  coverage and require a larger data maintenance policy.
- Defer postal-code choices to implementation: rejected because tests and user
  expectations need an exact supported set.

## Decision: Deterministic normalization without fuzzy matching

**Decision**: Normalize alias input by trimming surrounding whitespace,
lowercasing, removing accents, converting punctuation separators to spaces, and
collapsing repeated whitespace. Do not use fuzzy typo matching, partial search,
or nearest-match inference.

**Rationale**: This gives a forgiving non-expert input experience while keeping
resolution predictable and testable. Fuzzy matching would create ambiguous
behavior and hidden ranking rules.

**Alternatives considered**:

- Case and trim only: rejected because punctuation and accents are common in
  place-name input.
- Fuzzy matching: rejected because it expands scope and could resolve user
  input incorrectly.

## Decision: Preserve existing output labels

**Decision**: Successful output keeps existing labels. `Place` displays the
catalog display name and `Timezone` displays the canonical IANA timezone. No
new `Input`, `Resolved from`, or alias-specific label is added in v1.

**Rationale**: Existing output is already stable enough for light automation.
Adding conditional labels would require more contract and compatibility work.

**Alternatives considered**:

- Add original input label: rejected to avoid expanding output shape.
- Put original input in `Place`: rejected because the display name is the
  curated user-facing value.

## Decision: Catalog validation is mandatory

**Decision**: Automated validation must reject duplicate normalized inputs,
invalid canonical timezones, missing required fields, malformed Mexican postal
codes, and unsupported catalog entry categories.

**Rationale**: The catalog becomes part of product behavior. Validation prevents
future edits from silently introducing ambiguity, unsupported forms, or broken
timezone references.

**Alternatives considered**:

- Manual review only: rejected because defects would appear at runtime.
- Freeze catalog after v1: rejected because future curated additions are likely.
