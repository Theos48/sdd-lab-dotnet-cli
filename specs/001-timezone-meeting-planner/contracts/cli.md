# CLI Contract: Timezone Meeting Planner

## Command Shape

```text
timezone-cli --place <place-input> [--compare <place-input>]
```

When run through the existing project workflow:

```bash
dotnet run --project src/TimezoneCli -- --place <place-input> [--compare <place-input>]
```

## Inputs

`--place <place-input>`:

- Required.
- Accepts canonical IANA timezone identifiers such as `America/Mexico_City`.
- May accept a documented supported alias from the local alias catalog.

`--compare <place-input>`:

- Optional.
- Accepts the same input forms as `--place`.
- Only one comparison place is supported in v1.

Unsupported in v1:

- Mexican postal codes.
- More than one comparison place.
- Interactive disambiguation.
- Calendar event, attendee, reminder, recurrence, or availability arguments.

## Successful Lookup Output

Successful lookup uses stable human-readable labels on stdout:

```text
Place: <display name>
Timezone: <iana timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
```

Exit code: `0`

## Successful Comparison Output

Successful comparison uses stable human-readable labels on stdout:

```text
Place: <display name>
Timezone: <iana timezone id>
Local date: <YYYY-MM-DD>
Local time: <HH:mm>
UTC offset: <+HH:mm|-HH:mm>
Working hours: <within|outside>

Compared place: <display name>
Compared timezone: <iana timezone id>
Compared local date: <YYYY-MM-DD>
Compared local time: <HH:mm>
Compared UTC offset: <+HH:mm|-HH:mm>
Compared working hours: <within|outside>
Time difference: <+H:mm|-H:mm|0:00>
Meeting suitability: <suitable|not suitable>
```

Exit code: `0`

## Error Output

Errors write clear messages to stderr and return a non-zero exit code.

Missing required place:

```text
Error: --place is required.
Example: --place America/Mexico_City
```

Invalid timezone identifier:

```text
Error: invalid timezone identifier '<input>'.
Use an IANA timezone such as America/Mexico_City.
```

Unknown input:

```text
Error: unknown place '<input>'.
Use an IANA timezone such as America/Mexico_City.
```

Unsupported Mexican postal code:

```text
Error: Mexican postal codes are not supported in v1.
Use an IANA timezone such as America/Mexico_City.
```

Ambiguous alias with known matches:

```text
Error: ambiguous place '<input>'.
Known matches: <alias or timezone>, <alias or timezone>
Use a more specific IANA timezone.
```

Ambiguous alias without safe match list:

```text
Error: ambiguous place '<input>'.
Use a more specific IANA timezone.
```

Too many comparison places:

```text
Error: only one comparison place is supported in v1.
```

## Exit Codes

- `0`: success.
- `1`: invalid input, including malformed timezone identifiers, missing
  required arguments, or too many comparison places.
- `2`: unsupported input, including Mexican postal codes.
- `3`: unknown input, including unrecognized places or aliases.
- `4`: ambiguous input, including supported aliases that match multiple known
  aliases or timezones.

## Stability Requirements

- Labels in successful output are part of the v1 contract.
- Successful output is human-readable only, but label names and required field
  presence are stable enough for light automation.
- Errors must not prompt interactively.
- Errors must not automatically choose among ambiguous matches.
- Timezone identifiers in output must be canonical IANA identifiers.
