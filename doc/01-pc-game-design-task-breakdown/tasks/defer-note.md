Scope Notes deferred

task E02 ragdoll

- **E02-T09 (Network Sync)**: Requires networking library (Mirror/Netcode/FishNet). Deferred to E22.

task E03 game phase

- E03-T03 (Ready-up voting): Stubbed — depends on E22 (Networking) which doesn't exist yet. IPhaseManager will expose PlayerReady(int playerId) but PhaseManager will have a simple
  implementation that auto-skips when all tracked players are ready.
- E03-T05 (Player down/revive): Stubbed — depends on E02 (Player Controller). States will subscribe to OnPlayerDowned but no player controller publishes it yet.
