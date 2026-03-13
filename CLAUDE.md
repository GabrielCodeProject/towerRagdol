# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**Ragdoll Realms** — 2D tower defense with ragdoll physics, RPG progression, building, and comedy features.

## Tech Stack

- Unity 6 (6000.3.11f1), URP 2D renderer, Input System (new)
- Coplay MCP plugin for AI-assisted Unity operations

## Development

- No CLI build — use Unity Editor for building, testing, running
- Tests: Unity Test Runner (Window > General > Test Runner)
- Solution: `towerRagdol.sln` for IDE/Rider

## Rules

- **CRITICAL**: Use `mcp__coplay-mcp__*` tools for Unity operations — NEVER manually edit serialized files (.unity, .prefab, .asset)
- **CRITICAL**: All architecture rules enforced in `.claude/rules/CODING_STANDARDS.md` — read before writing ANY script
