# WebGL Performance Budget

## Targets
- First load < 15–20 MB compressed (Brotli)
- 60 fps mobile mid-tier where possible
- No threads (COOP/COEP not guaranteed on platforms)

## Player Settings
Use menu: **Dumpling67 → Apply WebGL Build Settings**
- Compression: Brotli
- Managed stripping: High
- IL2CPP + Wasm
- Template: PROJECT:Dumpling67

## Art
- Low-poly, atlases, few materials
- Limit real-time lights, prefer baked
- Avoid high-res textures on mobile WebGL
