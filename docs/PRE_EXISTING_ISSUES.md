# Pre-Existing Issues

Issues discovered during migration quality gates that were **not introduced by the migration**. These are tracked here for future resolution and are out of scope for the current migration work.

## CRITICAL

### `StreamDeckConnection.SendAsync(IMessage)` returns null on serialization failure
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs` (lines ~170-180)
- **Description**: When `JsonConvert.SerializeObject` throws, the method returns `null`. Callers `await` the result, so `await null` throws `NullReferenceException`, hiding the original serialization error.
- **Impact**: Any serialization failure crashes the plugin with a misleading exception.

## HIGH

### `Tools.GenerateKeyImage` leaks `SolidBrush`
- **File**: `barraider-sdtools/Tools/Tools.cs` (lines ~198-207)
- **Description**: `SolidBrush` created for the background fill is never disposed. GDI+ brush handles are a limited resource.
- **Impact**: Repeated key image generation can exhaust GDI+ handles over time.

### `Tools.AutoLoadPluginActions` does not null-check `Assembly.GetEntryAssembly()`
- **File**: `barraider-sdtools/Tools/Tools.cs` (line ~467)
- **Description**: `Assembly.GetEntryAssembly()` can return `null` in hosted or test scenarios. Calling `.GetTypes()` on null causes `NullReferenceException`.
- **Impact**: Plugin crashes in non-standard hosting environments.

## MEDIUM

### `SDConnection.previousImageHash` is not thread-safe
- **File**: `barraider-sdtools/Backend/SDConnection.cs` (lines ~24, 240-263)
- **Description**: `previousImageHash` field is read and written without synchronization. Concurrent `SetImageAsync` calls can race.
- **Impact**: Possible duplicate image sends or skipped updates under concurrent access.

### `SDConnection.Dispose()` does not null-check `StreamDeckConnection`
- **File**: `barraider-sdtools/Backend/SDConnection.cs` (lines ~143-152)
- **Description**: If `StreamDeckConnection` were null after partial construction, unsubscribing events would throw.
- **Impact**: Low probability but possible crash during error-path disposal.

### `StreamDeckConnection.OpenUrlAsync(string)` does not validate input
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs` (lines ~241-243)
- **Description**: `new Uri(uri)` is called without checking for null, throwing `ArgumentNullException`.
- **Impact**: Unhelpful exception when plugin passes null URI.

### `SDConnection` title-change retry has no limit
- **File**: `barraider-sdtools/Backend/SDConnection.cs` (lines ~456-464)
- **Description**: When `OnTitleParametersDidChange` is null, a 1-second delayed retry is scheduled with no retry cap or disposal check.
- **Impact**: Can schedule unbounded retries after connection disposal.

### `SystemDrawingImageCodec.DecodeFromBytes` catch block does not log
- **File**: `barraider-sdtools/Internal/SystemDrawingImageCodec.cs` (lines ~45-48)
- **Description**: The catch block disposes and rethrows but does not log the failure. This is new code but the logging gap is cosmetic and not a correctness issue introduced by the migration (the original `Image.FromStream` had no logging either).
- **Note**: Tracked here as low priority; not blocking migration.

## LOW

### Unused `using NLog.Layouts` in `SDConnection.cs`
- **File**: `barraider-sdtools/Backend/SDConnection.cs` (line 13)
- **Description**: Import is unused. `layout` parameter in `SetFeedbackLayoutAsync` is a `string`, not an NLog type.
- **Impact**: No runtime impact; cosmetic.

### `CancellationTokenSource` not disposed in `StreamDeckConnection`
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs` (line ~24)
- **Description**: `cancelTokenSource` is never disposed. Minor resource leak.
- **Impact**: Negligible in practice.

### `Tools.AutoPopulateSettings` lacks type-conversion error handling
- **File**: `barraider-sdtools/Tools/Tools.cs` (line ~389)
- **Description**: `Convert.ChangeType` can throw for incompatible types with no try/catch around individual property sets.
- **Impact**: One bad property value can abort population of remaining properties.

### `Tools.FilenameFromPayload` does not validate JToken type
- **File**: `barraider-sdtools/Tools/Tools.cs` (line ~227)
- **Description**: Casting a non-string `JToken` to `string` may throw.
- **Impact**: Edge case with malformed PI payloads.

### `Tools.BytesToSHA512` does not null-check input
- **File**: `barraider-sdtools/Tools/Tools.cs` (lines ~340-355)
- **Description**: `sha512.ComputeHash(null)` would throw `ArgumentNullException`. The exception is caught and logged, but a null guard would be cleaner.
- **Impact**: Low; callers generally pass non-null, and the catch block handles it.

### `ExtensionMethods.AddTextPath` leaks `Pen`, `GraphicsPath`, and `SolidBrush`
- **File**: `barraider-sdtools/Tools/ExtensionMethods.cs` (lines ~248-256)
- **Description**: `Pen`, `GraphicsPath`, and `SolidBrush` are created but never disposed. All three implement `IDisposable`.
- **Impact**: GDI+ handle leak on repeated calls.

### `ExtensionMethods.SplitToFitKey` leaks `Font`
- **File**: `barraider-sdtools/Tools/ExtensionMethods.cs` (line ~311)
- **Description**: `Font font = new Font(...)` is never disposed. `Font` implements `IDisposable`.
- **Impact**: Minor GDI+ handle leak.
