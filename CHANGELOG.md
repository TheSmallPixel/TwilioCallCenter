# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

## [0.1.0] — 2026-05-05

### Added
- Initial public release as TwilioCallCenter reference implementation
- Outbound call origination via `POST /api/calls/start`
- IVR + bridging via `/api/dtm` and `/api/connect`
- Status callback handling via `/api/event`
- SMS sending via `POST /api/sms`
- Twilio webhook signature validation on every callback endpoint
- xUnit test project covering TwiML generation and signature validation
- GitHub Actions CI workflow

### Changed
- Migrated from `netcoreapp2.1` (EOL) to `net8.0`
- Replaced static `Auth` class with `IOptions<TwilioOptions>` and `IOptions<CallCenterOptions>`
- Switched to ASP.NET Core minimal hosting
- Renamed party fields from Pro/User to Caller/Callee for neutrality

### Removed
- Hardcoded `https://giupiter.com` status callback (now configurable)
- Italian voice prompts (replaced with neutral, configurable English defaults)
- GPL-3.0 license (replaced with MIT)
- Tracked build artifacts (`.vs/`, `bin/`, `obj/`, `*.user`)
