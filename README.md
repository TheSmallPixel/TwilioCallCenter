# TwilioCallCenter

**Turn a Twilio number into a programmable cloud call center with one .NET service.**

[![Build](https://github.com/TheSmallPixel/TwilioCallCenter/actions/workflows/ci.yml/badge.svg)](https://github.com/TheSmallPixel/TwilioCallCenter/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Stars](https://img.shields.io/github/stars/TheSmallPixel/TwilioCallCenter?style=social)](https://github.com/TheSmallPixel/TwilioCallCenter/stargazers)

> _TODO: add a short demo GIF showing `POST /api/calls/start` triggering a two-leg call flow with the resulting TwiML response inline._
>
> `![Demo](docs/demo.gif)`

## Why

Building a programmable call center on top of Twilio usually means buying a packaged solution or stitching together half a dozen webhook handlers by hand. This is the smallest .NET 8 service that proves you don't need either — one ASP.NET Core app handles call origination, IVR, status callbacks, and SMS, with signed Twilio webhooks and DI-friendly configuration out of the box.

It's a **reference implementation**, not a product. Read it, fork it, lift the parts you need.

## 30-second example

Place a call between two phone numbers — Twilio dials the callee first, asks them to press a key to accept, then bridges the caller in.

```bash
curl -X POST http://localhost:5000/api/calls/start \
  -H "Content-Type: application/json" \
  -d '{
    "correlationId": "demo-001",
    "callerNumber": "+15555550001",
    "calleeNumber": "+15555550002",
    "maxDurationSeconds": 600
  }'
```

Behind the scenes:

1. The service asks Twilio to call `calleeNumber` from your Twilio number.
2. When the callee picks up, Twilio hits `/api/dtm` for IVR — a `<Gather>` plays a prompt and waits for a keypress.
3. On a keypress, `/api/connect` returns TwiML that `<Dial>`s `callerNumber`, bridging the call.
4. `/api/event` receives status callbacks (`initiated`, `ringing`, `answered`, `completed`) and forwards them to your configured webhook.

## Install & run

```bash
git clone https://github.com/TheSmallPixel/TwilioCallCenter.git
cd TwilioCallCenter

# configure (use user-secrets, env vars, or appsettings.Development.json)
dotnet user-secrets init   --project src/TwilioCallCenter
dotnet user-secrets set "Twilio:AccountSid"        "ACxxxxxxxx"                       --project src/TwilioCallCenter
dotnet user-secrets set "Twilio:AuthToken"         "xxxxxxxxxx"                       --project src/TwilioCallCenter
dotnet user-secrets set "Twilio:FromNumber"        "+15551234567"                     --project src/TwilioCallCenter
dotnet user-secrets set "CallCenter:PublicBaseUrl" "https://your-tunnel.ngrok.io"     --project src/TwilioCallCenter

dotnet run --project src/TwilioCallCenter
```

Then point your Twilio number's webhook to `https://<your-public-url>/api/dtm` and you're live.

## Links

- **Live demo** — none; this is a reference implementation
- **Changelog** — [CHANGELOG.md](CHANGELOG.md)
- **Contributing** — [CONTRIBUTING.md](CONTRIBUTING.md)

## Features

- ASP.NET Core minimal hosting; DI'd configuration via `IOptions<TwilioOptions>` and `IOptions<CallCenterOptions>`
- Twilio webhook signature validation enforced on every callback (`X-Twilio-Signature`)
- IVR (`<Gather>`), bridging (`<Dial>`), status callbacks, and SMS in five controllers
- `IHttpClientFactory`-based status forwarding — async, no socket exhaustion
- xUnit test suite covering TwiML output and signature validation

## Architecture

```text
POST /api/calls/start    →  Twilio originates outbound call
                            └─→ /api/dtm        (TwiML: Gather + prompt)
                                └─→ /api/connect (TwiML: Dial caller)
                            └─→ /api/event       (status callbacks)

POST /api/sms            →  Twilio sends an SMS
```

## Configuration

| Key                            | Required | Description                                                                |
|--------------------------------|----------|----------------------------------------------------------------------------|
| `Twilio:AccountSid`            | yes      | Twilio Account SID                                                         |
| `Twilio:AuthToken`             | yes      | Twilio auth token (used both for the REST client and signature validation) |
| `Twilio:FromNumber`            | yes      | E.164 number that calls and SMS originate from                             |
| `CallCenter:PublicBaseUrl`     | yes      | Public HTTPS base URL Twilio can hit (ngrok in dev, your domain in prod)   |
| `CallCenter:StatusWebhookUrl`  | optional | URL the service POSTs call status updates to                               |
| `CallCenter:GreetingText`      | optional | Voice prompt played to the callee before the keypress                      |
| `CallCenter:ConnectingText`    | optional | Voice prompt played right before bridging                                  |
| `CallCenter:Voice`             | optional | Twilio TTS voice (default `alice`)                                         |
| `CallCenter:Language`          | optional | TTS language (default `en-US`)                                             |

## Development

```bash
dotnet build
dotnet test
dotnet run --project src/TwilioCallCenter
```

## Roadmap

- Persist call state to Redis or SQL (`IMemoryCache` is fine for a single-instance reference)
- Recording + transcription endpoints
- OpenAPI / Swagger UI for the public surface

## License

[MIT](LICENSE) © Lorenzo Longiave
