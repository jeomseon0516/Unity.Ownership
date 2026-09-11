# Ownership Basic Usage

Open `OwnershipBasicUsage.unity` and enter Play Mode. **Replace** disposes the previous handle exactly once,
and **Clear** releases the current handle. The generated `Status` getter exposes only the borrowed value while
`SetStatus` performs the ownership exchange. Stopping Play Mode or deleting the sample object lets the attached
`OwnershipLifetimeHost` dispose the remaining handle.
