# Data Model: Working Hours Window Configuration

## WorkingHoursWindow

Represents the active local-time window used to determine whether a resolved
place is within working hours.

Fields:

- `Start`: local `HH:mm` time of day.
- `End`: local `HH:mm` time of day.

Validation rules:

- `Start` and `End` must both be present for a custom window.
- Values must parse strictly as two-digit `HH:mm` 24-hour local-clock times.
- `Start` is inclusive.
- `End` is exclusive.
- `End` must be later than `Start`.
- Equal start/end and overnight windows are invalid in v1.

Relationships:

- Used by `WorkingHoursPolicy` to assess each `ResolvedPlace`.
- Stored on or associated with `TimeComparison` so output can include the
  active `Working hours window` value.

## CustomWorkingHoursInput

Represents the raw CLI inputs before domain validation.

Fields:

- `WorkingHoursStart`: value from `--working-hours-start`, if provided.
- `WorkingHoursEnd`: value from `--working-hours-end`, if provided.

Validation rules:

- Both values must be absent for default behavior or present together for a
  custom window.
- Providing only one value is invalid input.
- Providing either value without `--compare` is invalid input.
- Argument validation occurs before place resolution and invalid input exits
  with code `1`.

## TimeComparison

Existing comparison result extended to include the active working-hours window.

Fields:

- `RequestedPlace`: resolved requested place.
- `ComparisonPlace`: resolved comparison place.
- `SignedTimeDifference`: comparison offset minus requested offset.
- `RequestedWorkingHours`: assessment for requested place.
- `ComparisonWorkingHours`: assessment for compared place.
- `CombinedSuitability`: suitable only when both assessments are within hours.
- `WorkingHoursWindow`: active window used for both assessments.

Relationships:

- `WorkingHoursWindow` is evaluated separately against each place's local date
  and local time.
- Existing place resolution, UTC offset calculation, and time difference
  behavior remain unchanged.

## WorkingHoursAssessment

Existing per-place assessment result.

Fields:

- `IsWithinWorkingHours`: true when local weekday and local time are within the
  active working-hours window.
- `Reason`: diagnostic reason for tests and domain clarity.

Validation and state rules:

- Saturday and Sunday remain outside working hours.
- Weekday local time equal to `Start` is within working hours.
- Weekday local time equal to `End` is outside working hours.
- Weekday local time between `Start` and before `End` is within working hours.
