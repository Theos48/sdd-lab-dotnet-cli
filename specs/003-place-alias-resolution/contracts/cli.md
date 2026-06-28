# CLI Contract: Place Alias Resolution

## Command Shape

Existing CLI arguments remain unchanged:

```text
timezone-cli --place <place-input> [--compare <place-input>] [--working-hours-start <HH:mm> --working-hours-end <HH:mm>]
```

When run through the project workflow:

```bash
make dev ARGS="--place <place-input> --compare <place-input>"
```

## Accepted Place Inputs

`<place-input>` may be any of:

- A valid canonical IANA timezone identifier, such as
  `America/Mexico_City`.
- A supported alias from the local curated catalog, such as `mexico city`.
- A selected supported Mexican postal code from the local curated catalog:
  `01000`, `64000`, or `44100`.

Unsupported in v1:

- Five-digit Mexican postal codes not explicitly listed above.
- Fuzzy typo matching.
- Partial place-name search.
- External geocoding or remote lookup.
- Interactive disambiguation prompts.

## Normalization

Alias matching is deterministic. User input is normalized for matching by:

- trimming surrounding whitespace;
- lowercasing;
- removing accents;
- converting punctuation separators to spaces;
- collapsing repeated whitespace.

Postal-code inputs are treated as strings; leading zeroes are significant.

## Successful Lookup Output

Successful lookup output keeps the existing labels:

```text
Place: <catalog display name or IANA timezone>
Timezone: <canonical IANA timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
```

For supported aliases and supported postal codes:

- `Place` shows the catalog display name.
- `Timezone` shows the canonical IANA timezone.

Exit code: `0`

## Successful Comparison Output

Successful comparison output remains unchanged except that place inputs may now
be supported aliases or selected postal codes:

```text
Place: <catalog display name or IANA timezone>
Timezone: <canonical IANA timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
Working hours: <within|outside>
Working hours window: <HH:mm>-<HH:mm>

Compared place: <catalog display name or IANA timezone>
Compared timezone: <canonical IANA timezone id>
Compared local date: <YYYY-MM-DD>
Compared local time: <HH:mm>
Compared UTC offset: <+HH:mm|-HH:mm>
Compared working hours: <within|outside>
Time difference: <+H:mm|-H:mm|0:00>
Meeting suitability: <suitable|not suitable>
```

Exit code: `0`

## Supported Mexican Postal Codes

Required v1 mappings:

```text
01000 -> America/Mexico_City
64000 -> America/Monterrey
44100 -> America/Mexico_City
```

## Error Output

Unsupported Mexican postal code:

```text
Error: Mexican postal code '<input>' is not supported in v1.
Supported Mexican postal codes: 01000, 64000, 44100
Use an IANA timezone such as America/Mexico_City.
```

Unknown place:

```text
Error: unknown place '<input>'.
Use an IANA timezone such as America/Mexico_City.
```

Ambiguous place:

```text
Error: ambiguous place '<input>'.
Known matches: <display name> (<timezone>), <display name> (<timezone>)
Use a more specific IANA timezone.
```

Invalid timezone identifier:

```text
Error: invalid timezone identifier '<input>'.
Use an IANA timezone such as America/Mexico_City.
```

Working-hours argument errors:

- Existing working-hours error messages and exit code `1` remain unchanged.
- Invalid working-hours arguments continue to take precedence over alias,
  postal-code, unknown, unsupported, or ambiguous place-resolution errors.

## Exit Codes

- `0`: success.
- `1`: invalid input, including malformed timezone identifiers, missing
  required arguments, too many comparison places, malformed working-hours
  values, and invalid working-hours ranges.
- `2`: unsupported input, including unsupported Mexican postal codes.
- `3`: unknown input, including unrecognized place aliases.
- `4`: ambiguous input, including catalog matches that cannot be resolved
  deterministically.

## Stability Requirements

- No CLI arguments are added, removed, or renamed.
- Existing successful lookup labels remain unchanged.
- Existing successful comparison labels remain unchanged.
- `Place` remains the user-facing display label and `Timezone` remains the
  canonical IANA timezone label.
- Output remains human-readable, with stable labels and field presence for light
  automation.
- The CLI must not prompt interactively.
