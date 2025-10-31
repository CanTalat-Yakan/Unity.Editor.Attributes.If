# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Show If Attribute

> Quick overview: Conditionally show or disable fields and methods in the Inspector based on another field’s value. Use `[ShowIf("fieldName", value)]`, `[ShowIf(Visibility.Disable, "fieldName", value)]`, or the inverse `[ShowIfNot(...)]`.

Make inspectors adaptive without custom editors. Annotate a field (or method) with `[ShowIf]` to display it only when a referenced field matches the specified value(s). Choose whether non‑matching items are hidden entirely or shown but disabled.

![screenshot](Documentation/Screenshot.png)

## Features
- Works on serialized fields and on methods rendered by other tooling (e.g., a button method)
- Compare another field by name against one or more values
- Two behaviors when the condition is not met:
  - Hide (default): don’t draw the field/method at all
  - Disable: draw it but make it non‑editable (greyed‑out)
- Inverse logic with `[ShowIfNot]`
- Inspector‑only; zero runtime overhead

## Requirements
- Unity Editor 6000.0+ (Editor‑only; attributes live in Runtime for convenience)
- Depends on the Unity Essentials Inspector Hooks module (underpins conditional drawing, enable/disable)

Tip: Use top‑level serialized field names for the condition source. Nested paths may not be reliably supported.

## Usage
Show a field only when a boolean is true

```csharp
using UnityEngine;
using UnityEssentials;

public class Example : MonoBehaviour
{
    public bool advanced;

    [ShowIf(nameof(advanced), true)]
    public float advancedSpeed;
}
```

Disable instead of hiding when condition is false

```csharp
public enum Mode { Basic, Advanced }

public class Example2 : MonoBehaviour
{
    public Mode mode;

    // Visible for both modes, but disabled unless Advanced
    [ShowIf(Visibility.Disable, nameof(mode), Mode.Advanced)]
    public int iterations;
}
```

Inverse condition with ShowIfNot

```csharp
public class Example3 : MonoBehaviour
{
    public string profile = "Default";

    // Show only when profile is not "Default"
    [ShowIfNot(nameof(profile), "Default")]
    public string customProfilePath;
}
```

Multiple allowed values

```csharp
public enum State { Off, Idle, Active }

public class Example4 : MonoBehaviour
{
    public State state;

    // Note: The current implementation requires the source to equal ALL provided values to show.
    // Passing multiple different values will effectively hide the field. Prefer a single value.
    [ShowIf(nameof(state), State.Idle /*, State.Active */)]
    public float refreshRate;
}
```

Conditionally show a method button

```csharp
public class Example5 : MonoBehaviour
{
    public bool canReset;

    [ShowIf(nameof(canReset), true)]
    [Button]
    public void ResetNow() { /* ... */ }
}
```

## How It Works
- At inspector time, the system reads attributes on fields and methods:
  - `[ShowIf(fieldName, values...)]` → compare the referenced field’s current serialized value to the provided value(s)
  - `[ShowIfNot(fieldName, values...)]` → apply the inverse comparison
- When the condition is not met:
  - Hide: the item is marked as handled and skipped (not drawn)
  - Disable: the item is drawn with GUI disabled (for fields). For methods, disabling requires a renderer that honors disabled state
- Comparison uses the serialized property’s current value; use simple types like bools, enums, numbers, strings for best results

## Notes and Limitations
- Source field name: use top‑level serialized field names. Deep property paths may not be consistently resolved
- Supported comparisons: best with primitives, enums, strings. Complex types may not compare as intended
- Multiple values: due to current implementation, ShowIf matches only when the source equals ALL provided values; prefer a single value. ShowIfNot hides/disabled when the source equals ANY provided value.
- Methods: hide works uniformly; disable requires the specific method renderer to check the disabled flag
- Multi‑object editing: conditions are evaluated per‑inspected target
- Editor‑only: no impact at runtime

## Files in This Package
- `Runtime/ShowIfAttribute.cs` – `[ShowIf]` and `[ShowIfNot]` attribute markers plus `Visibility` enum
- `Editor/ShowIfDrawer.cs` – Conditional handling (hide/disable) for properties and methods via Inspector Hooks
- `Runtime/UnityEssentials.ShowIfAttribute.asmdef` – Runtime assembly definition
- `Editor/UnityEssentials.ShowIfAttribute.Editor.asmdef` – Editor assembly definition

## Tags
unity, unity-editor, attribute, conditional, showif, hide, disable, inspector, ui, tools, workflow
