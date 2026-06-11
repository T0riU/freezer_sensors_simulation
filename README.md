# Freezer Sensors Simulation

A C# WinForms desktop application that simulates a sensor monitoring system for an industrial freezer. Sensors report electrical signal values; the app evaluates each reading and determines the appropriate control action.

---

## Project Structure

```
RGR.sln               Visual Studio solution file
RGR/
  Form1.cs            Main form — sensor state management and response logic
  SetValue.cs         Dialog for manually setting a sensor value via trackbar
  Enums.cs            Sensor types enum and shared constants
  Program.cs          Entry point
  Properties/         Assembly and resource metadata
```

---

## How It Works

The system monitors 26 sensors across three types and maps each reading to a corrective action. All sensor values are electrical signals in the range 0.0–12.0 V, with a normal operating band of 5.3–6.7 V.

### Sensor Types and Counts

| Type | Count | Monitors |
|------|-------|---------|
| TEMPERATURE | 14 | Freezer temperature zones |
| CONCENTRATION | 8 | Coolant concentration |
| CURRENTDENSITY | 4 | Electrical current density |

### Signal Thresholds

| Constant | Value | Meaning |
|----------|-------|---------|
| MIN_SENSOR_VAL | 0.0 V | Absolute minimum |
| MIN_CR_SENSOR_VAL | 4.2 V | Lower critical threshold |
| MIN_N_SENSOR_VAL | 5.3 V | Lower normal threshold |
| MAX_N_SENSOR_VAL | 6.7 V | Upper normal threshold |
| MAX_CR_SENSOR_VAL | 7.8 V | Upper critical threshold |
| MAX_SENSOR_VAL | 12.0 V | Absolute maximum |
| DF_VAL_SENSOR | 6.0 V | Default (startup) value |

### Response Logic

| Condition | TEMPERATURE action | CONCENTRATION action | CURRENTDENSITY action |
|-----------|-------------------|---------------------|----------------------|
| Value < 4.2 V or > 7.8 V | EMERGENCY STATE | EMERGENCY STATE | — |
| 4.2–5.3 V (low) | Heater on | Concentrate feed on | Voltage reduced |
| 6.7–7.8 V (high) | Refrigerator on | Water feed on | Voltage increased |
| 5.3–6.7 V | Normal — no action | Normal — no action | Normal — no action |

For each triggered sensor the log outputs: the sensor number, the action taken, the executive device number, and the RES2 binary code used to address that device.

### Executive Device Addressing

Devices are addressed by adding a type-specific offset to the sensor index. The resulting integer is also encoded as a binary RES2 code displayed in the output log.

---

## Requirements

- Windows
- .NET Framework 4.x
- Visual Studio 2019 or later (or MSBuild)

## Build & Run

```
# Open solution in Visual Studio:
RGR.sln

# Or build from command line:
msbuild RGR.sln /p:Configuration=Release
```

The output executable will be in `RGR/bin/Release/`.

---

## Example Output

```
In: 1
Reason: sensor №1;
Action: The refrigerator is on;
Executive device: №47;
RES2 code: 101111;

Reason: sensor №16;
Action: The concentrate feed is on;
Executive device: №62;
RES2 code: 111110;
```
