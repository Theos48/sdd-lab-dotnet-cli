# Data Model: Timezone Meeting Planner

## PlaceInput

Represents one user-provided place value.

Fields:

- `RawValue`: original text supplied by the user.
- `NormalizedValue`: trimmed, case-normalized value used for matching.
- `InputKind`: `TimezoneIdentifier`, `SupportedAlias`, `MexicanPostalCode`,
  `Unknown`, or `Invalid`.
- `Role`: `RequestedPlace` or `ComparisonPlace`.

Validation rules:

- `RawValue` is required for the requested place.
- At most one comparison place is allowed.
- Mexican postal-code-shaped values are unsupported in v1 and must not resolve
  to a timezone.
- Invalid or unknown values produce a non-zero exit path.

## SupportedAlias

A documented convenience name stored in the fixed curated repository alias file.

Fields:

- `Alias`: user-facing alias value.
- `NormalizedAlias`: normalized lookup key.
- `DisplayName`: label shown in successful output.
- `TimeZoneId`: canonical IANA timezone identifier.

Validation rules:

- `TimeZoneId` must be a valid IANA timezone identifier in the container
  environment.
- A supported alias must resolve to exactly one timezone.
- Alias support is determined only by canonical IANA timezone identifiers or
  normalized aliases present in the repository alias file.
- If an input maps to multiple known aliases or timezones, resolution returns an
  ambiguous result rather than selecting one.

Relationships:

- Resolves to one `ResolvedPlace` when valid and unambiguous.

## AliasCatalog

The versioned local data set for supported aliases.

Fields:

- `Version`: data version string.
- `Aliases`: collection of `SupportedAlias` records.

Validation rules:

- Every alias data change must update the catalog version.
- Alias keys must be unique after normalization unless intentionally represented
  as an ambiguous match set.
- Each alias record must include an alias, normalized alias key, display name,
  and valid IANA timezone identifier.
- The catalog is loaded from the repository data file and does not use network
  access.

## ResolvedPlace

A successfully resolved place at an evaluation moment.

Fields:

- `DisplayName`: user-facing place label.
- `TimeZoneId`: canonical IANA timezone identifier.
- `LocalDate`: date at the evaluation moment in the place timezone.
- `LocalTime`: time at the evaluation moment in the place timezone.
- `UtcOffset`: offset from UTC at the evaluation moment.
- `Source`: `TimezoneIdentifier` or `SupportedAlias`.

Validation rules:

- `TimeZoneId` must resolve successfully before local date/time and offset are
  calculated.
- Offset must be the offset that applies at the evaluation moment.

## TimeComparison

Represents the relationship between the requested place and comparison place at
the same evaluation moment.

Fields:

- `RequestedPlace`: resolved requested place.
- `ComparisonPlace`: resolved comparison place.
- `SignedTimeDifference`: difference between comparison local time and requested
  local time.
- `RequestedWorkingHours`: working-hours assessment for the requested place.
- `ComparisonWorkingHours`: working-hours assessment for the comparison place.
- `CombinedSuitability`: suitable only when both per-place assessments are
  within normal working hours.

Validation rules:

- Both places must resolve before comparison.
- Same-timezone comparisons are valid and should report zero time difference.
- Different local calendar dates are valid and must be visible in output.

## WorkingHoursAssessment

Result of applying the v1 meeting-hours policy to a resolved place.

Fields:

- `IsWithinWorkingHours`: boolean.
- `Reason`: concise reason such as `weekday-within-hours`,
  `weekday-after-hours`, or `weekend`.

Validation rules:

- Normal working hours are Monday through Friday, 09:00 inclusive through before
  17:00, local to the resolved place.
- Exactly 09:00 on a weekday is within working hours.
- Exactly 17:00 on a weekday is outside working hours.

## CliResult

Output-ready result produced by the application boundary.

Fields:

- `ExitCode`: `0` for success, `1` for invalid input, `2` for unsupported
  input, `3` for unknown input, or `4` for ambiguous input.
- `StdoutLines`: labeled result lines for successful lookup or comparison.
- `StderrLines`: clear error lines for invalid, unknown, unsupported, or
  ambiguous input.

Validation rules:

- Successful lookup and comparison results use stable labels.
- Successful output is human-readable only, with stable labels and required
  field presence for light automation.
- Error results do not silently fall back to another timezone.
- Ambiguous input lists known matching aliases or timezones when available.
