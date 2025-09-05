DECKMASTER MARINE LOGO PLACEHOLDER

To complete the splash screen setup:

1. Save your Deckmaster Marine logo as "deckmaster-logo.png" in this folder
2. Recommended size: 1200x600 pixels (or similar 2:1 ratio to match the original)
3. Use PNG format with transparent background for best results
4. The logo should be high resolution for crisp display on all devices

The project is already configured to use:
- File: deckmaster-logo.png
- Background color: #233973 (your app's primary blue)
- Base size: 300x150 (will scale appropriately)

Current configuration in Interpolation.App.csproj:
<MauiSplashScreen Include="Resources\Splash\deckmaster-logo.png" Color="#233973" BaseSize="300,150" />
