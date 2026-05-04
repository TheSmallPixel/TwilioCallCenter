# Contributing

Thanks for considering a contribution.

## Setup

```bash
git clone https://github.com/TheSmallPixel/TwilioCallCenter.git
cd TwilioCallCenter
dotnet build
dotnet test
```

## Workflow

1. Fork and create a topic branch from `master`.
2. Make focused commits — one concern per commit.
3. Run `dotnet test` before pushing.
4. Open a PR using the template; link the issue you're addressing.

## Code style

- ASP.NET Core conventions, async-all-the-way.
- Twilio credentials never go in code or commits — use `dotnet user-secrets` or environment variables.
- Public method changes need a test.
- One concern per commit; commit messages explain the *why*, not the *what*.
