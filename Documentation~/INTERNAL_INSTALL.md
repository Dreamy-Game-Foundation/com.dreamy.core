# Internal Install - com.dreamy.core

Use Git URL imports for internal team projects. Do not use OpenUPM for this private package.

## Team Setup

1. Push `Packages/com.dreamy.core` to its own private GitHub repository.
2. Tag stable releases with SemVer, for example `v1.1.0`.
3. Give every teammate read access to the repository.
4. In each consuming Unity project, add `com.dreamy.core` by Git URL and tag.

## Manifest Template

```json
{
  "dependencies": {
    "com.dreamy.core": "https://github.com/Dreamy-Game-Foundation/com.dreamy.core.git#v1.1.0"
  }
}
```

Replace the `com.dreamy.core` URL with the team's real private repository URL if different.

## Optional Project Dependencies

`com.dreamy.core` itself has no third-party runtime package dependency.

Install project-level dependencies such as UniTask, DOTween, Addressables, and LeanPool in the consuming project's `Packages/manifest.json` or through a private scoped registry. Unity Package Manager can install a package from a Git URL, but Git URL dependencies inside that package are not a reliable distribution boundary for team projects.

## Release Checklist

- `package.json` version matches the Git tag.
- `CHANGELOG.md` has an entry for the version.
- All Runtime and Samples files include `.meta` files.
- Import into a clean Unity 6000 project using the manifest template.
- Open Unity once and confirm there are no console compile errors.
