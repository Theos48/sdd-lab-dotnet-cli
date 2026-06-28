# CLI Contract: Working Hours Window Configuration

## Command Shape

```text
timezone-cli --place <place-input> [--compare <place-input>] [--working-hours-start <HH:mm> --working-hours-end <HH:mm>]
```

When run through the existing project workflow:

```bash
make dev ARGS="--place <place-input> --compare <place-input> --working-hours-start <HH:mm> --working-hours-end <HH:mm>"
```

## Inputs

`--place <place-input>`:

- Required.
- Existing behavior unchanged.
- Accepts canonical IANA timezone identifiers or documented aliases from the
  local alias catalog.

`--compare <place-input>`:

- Optional for lookup.
- Required when custom working-hours flags are used.
- Existing comparison place resolution behavior unchanged.

`--working-hours-start <HH:mm>`:

- Optional, but only valid together with `--working-hours-end`.
- Only valid when `--compare` is present.
- Strict two-digit 24-hour local-clock format.

`--working-hours-end <HH:mm>`:

- Optional, but only valid together with `--working-hours-start`.
- Only valid when `--compare` is present.
- Strict two-digit 24-hour local-clock format.
- Must be later than `--working-hours-start`.

Supported forms:

```text
--working-hours-start 08:30 --working-hours-end 16:45
--working-hours-start=08:30 --working-hours-end=16:45
```

Unsupported in v1:

- One working-hours flag without the other.
- Working-hours flags without `--compare`.
- Flexible time strings such as `9am`, `9:00`, or `0900`.
- `24:00`, invalid minutes, zero-length ranges, and overnight ranges.
- Persistent working-hours preferences.

## Successful Lookup Output

Successful single-place lookup output is unchanged:

```text
Place: <display name>
Timezone: <iana timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
```

Exit code: `0`

## Successful Comparison Output

Successful comparison output includes the active working-hours window for both
default and custom-window comparisons:

```text
Place: <display name>
Timezone: <iana timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
Working hours: <within|outside>
Working hours window: <HH:mm>-<HH:mm>

Compared place: <display name>
Compared timezone: <iana timezone id>
Compared local date: <YYYY-MM-DD>
Compared local time: <HH:mm>
Compared UTC offset: <+HH:mm|-HH:mm>
Compared working hours: <within|outside>
Time difference: <+H:mm|-H:mm|0:00>
Meeting suitability: <suitable|not suitable>
```

Default comparison value:

```text
Working hours window: 09:00-17:00
```

Custom comparison example:

```text
Working hours window: 08:30-16:45
```

Exit code: `0`

## Error Output

Errors write clear messages to stderr and return a non-zero exit code.

Working-hours start missing end:

```text
Error: --working-hours-start and --working-hours-end must be provided together.
Example: --working-hours-start 09:00 --working-hours-end 17:00
```

Working-hours end missing start:

```text
Error: --working-hours-start and --working-hours-end must be provided together.
Example: --working-hours-start 09:00 --working-hours-end 17:00
```

Working-hours flag used without comparison:

```text
Error: working-hours options require --compare.
```

Malformed working-hours value:

```text
Error: invalid working-hours time '<input>'.
Use HH:mm 24-hour format, for example 09:00.
```

Invalid working-hours range:

```text
Error: --working-hours-end must be later than --working-hours-start.
Overnight and zero-length working-hours windows are not supported in v1.
```

Precedence:

- Working-hours argument errors are detected before place resolution.
- If working-hours arguments are invalid, exit code `1` is used even when
  place inputs are also unsupported, unknown, ambiguous, or invalid.
- If working-hours arguments are absent or valid, existing place-resolution
  error messages and exit codes remain unchanged.

## Exit Codes

- `0`: success.
- `1`: invalid input, including malformed working-hours values, missing paired
  working-hours flags, working-hours flags without comparison, zero-length
  ranges, overnight ranges, malformed timezone identifiers, missing required
  arguments, or too many comparison places.
- `2`: unsupported input, including Mexican postal codes, when
  working-hours arguments are absent or valid.
- `3`: unknown input, including unrecognized places or aliases, when
  working-hours arguments are absent or valid.
- `4`: ambiguous input, including supported aliases that match multiple known
  aliases or timezones, when working-hours arguments are absent or valid.

## Stability Requirements

- Existing successful lookup labels remain unchanged.
- Existing successful comparison labels remain present.
- `Working hours window` is a stable successful-comparison label.
- Output remains human-readable only, but label names and required field
  presence remain stable enough for light automation.
- Errors must not prompt interactively.
- Working-hours configuration must not change timezone resolution, alias
  resolution, UTC offset calculation, or signed time-difference calculation.
