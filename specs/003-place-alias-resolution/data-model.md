# Data Model: Place Alias Resolution

## Alias Catalog

Represents the versioned local repository file that declares all supported
non-IANA place inputs.

### Fields

- `version`: Required catalog version string.
- `aliases`: Required collection of catalog entries.

### Validation Rules

- `version` must be present and non-empty.
- `aliases` must be present.
- Every entry must pass Supported Catalog Entry validation.
- No two entries may share the same normalized input.

## Supported Catalog Entry

Represents one supported user input form that maps to one canonical IANA
timezone.

### Fields

- `input`: Required user-facing input value, such as `mexico city` or `01000`.
- `normalizedInput`: Required deterministic normalized form of `input`.
- `displayName`: Required name shown in the existing `Place` output field.
- `timeZoneId`: Required canonical IANA timezone identifier.
- `category`: Required support category.

### Categories

- `alias`: Common place-name alias.
- `mexican-postal-code`: Selected supported Mexican postal code.

### Validation Rules

- `input` must be present and non-empty.
- `normalizedInput` must equal the deterministic normalization of `input`.
- `displayName` must be present and non-empty.
- `timeZoneId` must resolve as a valid IANA timezone identifier.
- `category` must be one of the supported categories.
- `mexican-postal-code` entries must use exactly five digits.
- `alias` entries must not be five-digit postal-code-like values.

## Supported Alias

Represents a catalog entry with category `alias`.

### Examples

- `mexico city` -> `America/Mexico_City`
- `london` -> `Europe/London`
- `new york` -> `America/New_York`
- `tokyo` -> `Asia/Tokyo`

### Validation Rules

- Must follow Supported Catalog Entry validation.
- Must participate in normalization and duplicate detection.

## Supported Mexican Postal Code

Represents a catalog entry with category `mexican-postal-code`.

### Required v1 Entries

- `01000` -> `America/Mexico_City`
- `64000` -> `America/Monterrey`
- `44100` -> `America/Mexico_City`

### Validation Rules

- Must follow Supported Catalog Entry validation.
- Input must be treated as a string so leading zeroes remain significant.
- Unsupported five-digit postal-code-like inputs remain unsupported.

## Place Resolution

Represents the result of resolving one raw place input.

### Fields

- `displayName`: Name used in the existing `Place` output field.
- `timeZoneId`: Canonical IANA timezone identifier.
- `source`: Resolution source.

### Sources

- `timezone-identifier`: Input was a valid IANA timezone identifier.
- `supported-alias`: Input resolved from a supported alias.
- `supported-mexican-postal-code`: Input resolved from a selected Mexican
  postal code.

## Ambiguous Place Input

Represents a normalized user input that matches more than one catalog entry.

### Fields

- `input`: Original user input.
- `knownMatches`: Known display names and canonical timezones that matched.

### Behavior

- Must fail with the ambiguous-input category.
- Must not select a timezone automatically.
- Should list known matches when available.

## Unsupported Postal Code Input

Represents a five-digit numeric input that is not explicitly listed as a
supported Mexican postal code.

### Behavior

- Must fail with the unsupported-input category.
- Must preserve leading zeroes in the user-facing message.
- Must not attempt geocoding or inference.

## Resolution Flow

1. Validate the raw input is not empty.
2. Try resolving the raw input as an IANA timezone identifier.
3. Normalize the input for catalog matching.
4. If the normalized input matches one catalog entry, resolve to that entry's
   canonical IANA timezone.
5. If the normalized input matches multiple entries, return ambiguous input.
6. If the raw input is a five-digit numeric value and no supported catalog entry
   matched, return unsupported postal code.
7. Otherwise return unknown input, or invalid timezone identifier when the input
   uses timezone-identifier syntax but does not resolve.
