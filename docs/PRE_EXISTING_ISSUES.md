# Pre-Existing Issues

Issues discovered during migration quality gates that were **not introduced by the migration**. These were tracked here for future resolution and were out of scope for the migration work.

## Status: All Resolved

All issues listed below have been fixed. Each fix was independently cross-reviewed by verification agents.

---

### ~~`StreamDeckConnection.SendAsync(IMessage)` returns null on serialization failure~~ **FIXED**
- **Severity**: CRITICAL
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs`
- **Fix**: Returns `Task.CompletedTask` instead of `null` so callers can safely `await`.

### ~~`Tools.GenerateKeyImage` leaks `SolidBrush`~~ **FIXED**
- **Severity**: HIGH
- **File**: `barraider-sdtools/Tools/Tools.cs`
- **Fix**: Wrapped `SolidBrush` in a `using` block.

### ~~`Tools.AutoLoadPluginActions` does not null-check `Assembly.GetEntryAssembly()`~~ **FIXED**
- **Severity**: HIGH
- **File**: `barraider-sdtools/Tools/Tools.cs`
- **Fix**: Added null-check with logging; returns empty array when entry assembly is null.

### ~~`SDConnection.previousImageHash` is not thread-safe~~ **FIXED**
- **Severity**: MEDIUM
- **File**: `barraider-sdtools/Backend/SDConnection.cs`
- **Fix**: Added `_imageHashLock` with `lock()` around hash comparison/assignment in all `SetImageAsync` overloads. `await` calls remain outside the lock.

### ~~`SDConnection.Dispose()` does not null-check `StreamDeckConnection`~~ **FIXED**
- **Severity**: MEDIUM
- **File**: `barraider-sdtools/Backend/SDConnection.cs`
- **Fix**: Added null-check guard at the start of `Dispose()`.

### ~~`StreamDeckConnection.OpenUrlAsync(string)` does not validate input~~ **FIXED**
- **Severity**: MEDIUM
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs`
- **Fix**: Added null/empty check that throws `ArgumentNullException`.

### ~~`SDConnection` title-change retry has no limit~~ **FIXED**
- **Severity**: MEDIUM
- **File**: `barraider-sdtools/Backend/SDConnection.cs`
- **Fix**: Added `MAX_TITLE_RETRY_ATTEMPTS = 5` constant and `_titleRetryCount` field. Counter-based limit replaces the fragile `sender != this` pattern.

### ~~`SystemDrawingImageCodec.DecodeFromBytes` catch block does not log~~ **FIXED**
- **Severity**: MEDIUM
- **File**: `barraider-sdtools/Internal/SystemDrawingImageCodec.cs`
- **Fix**: Added `Logger.Instance.LogMessage` call in the catch block before rethrow.

### ~~Unused `using NLog.Layouts` in `SDConnection.cs`~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Backend/SDConnection.cs`
- **Fix**: Removed unused import.

### ~~`CancellationTokenSource` not disposed in `StreamDeckConnection`~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Communication/StreamDeckConnection.cs`
- **Fix**: Added `cancelTokenSource.Dispose()` in `DisconnectAsync()` after cleanup.

### ~~`Tools.AutoPopulateSettings` lacks type-conversion error handling~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Tools/Tools.cs`
- **Fix**: Wrapped `Convert.ChangeType` in try/catch; logs error and continues to next property.

### ~~`Tools.FilenameFromPayload` does not validate JToken type~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Tools/Tools.cs`
- **Fix**: Added null/type checks; uses `payload.ToString()` instead of explicit cast.

### ~~`Tools.BytesToSHA512` does not null-check input~~ **ALREADY FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Tools/Tools.cs`
- **Note**: The null-check already exists in the current code. No change needed.

### ~~`ExtensionMethods.AddTextPath` leaks `Pen`, `GraphicsPath`, and `SolidBrush`~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Tools/ExtensionMethods.cs`
- **Fix**: Wrapped `Font`, `Pen`, `GraphicsPath`, and `SolidBrush` in `using` blocks.

### ~~`ExtensionMethods.SplitToFitKey` leaks `Font`~~ **FIXED**
- **Severity**: LOW
- **File**: `barraider-sdtools/Tools/ExtensionMethods.cs`
- **Fix**: Wrapped `Font` in a `using` block.
