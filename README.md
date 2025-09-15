# Interpolation Application

A professional .NET MAUI mobile application for performing linear and bilinear interpolation calculations, designed for engineers and technical professionals.

![Interpolation App](https://img.shields.io/badge/Platform-.NET%20MAUI-blue) ![License](https://img.shields.io/badge/License-MIT-green) ![Version](https://img.shields.io/badge/Version-1.0-orange)

## 🚀 Features

### Single (1-D) Linear Interpolation
- Calculate intermediate values between two known points
- Perfect for interpolating single-variable data from tables
- Visual 3-row layout showing input/output relationships

### Bilinear (2-D) Interpolation  
- Calculate values within a grid of four known corner points
- Ideal for interpolating from 2D tables and charts
- Interactive grid interface with real-time calculations

### User Interface
- **Clean, intuitive design** with visual grid layouts
- **Mode switching** between Single and Double interpolation
- **Real-time validation** - results only show when all required inputs are complete
- **Professional styling** optimized for mobile devices
- **Error handling** with clear user feedback

## 📱 Platforms

- ✅ **Android** (Primary target)

## 🔧 How It Works

### Single Linear Interpolation

Given two known points (x₁, y₁) and (x₂, y₂), find the y-value for a target x-value:

```
y = y₁ + (x - x₁) × (y₂ - y₁) / (x₂ - x₁)
```

**Example Usage:**
1. Enter **x₁** and corresponding **y₁** value
2. Enter **x₂** and corresponding **y₂** value  
3. Enter target **x** value
4. App calculates and displays the interpolated **y** result

### Bilinear Interpolation

For a 2D grid with four corner points, calculate the value at any point within:

```
Known Points: (x₁,y₁,z₁₁), (x₂,y₁,z₂₁), (x₁,y₂,z₁₂), (x₂,y₂,z₂₂)
Target: (x,y) → Calculate z
```

**Example Usage:**
1. Define the **X-axis** bounds (Left, Target, Right)
2. Define the **Y-axis** bounds (Low, Target, High)
3. Enter the **four corner Z-values**
4. App calculates intermediate values and final result

## 🏗️ Technical Details

### Built With
- **.NET MAUI** - Cross-platform framework
- **C#** - Core application logic
- **XAML** - User interface markup
- **Custom Math Library** - Interpolation algorithms

### Architecture
```
src/
├── Interpolation.App/          # Main MAUI application
│   ├── MainPage.xaml          # UI layout and controls
│   ├── MainPage.xaml.cs       # UI logic and event handling
│   └── AppShell.xaml          # Navigation shell
├── Interpolation.Core/         # Core math library
│   └── InterpolationMath.cs   # Interpolation algorithms
└── tests/
    └── Interpolation.Tests/    # Unit tests
```

### Key Classes
- **`InterpolationMath`** - Static methods for calculations
- **`MainPage`** - UI controller with input validation
- **`AppShell`** - Application navigation structure

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Android SDK (for Android development)

### Building the Project

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/interpolation-app.git
   cd interpolation-app
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Run on Android:**
   ```bash
   dotnet build src/Interpolation.App/Interpolation.App.csproj -f net9.0-android -t:Run
   ```

### Creating Release Builds

**For Android APK:**
```bash
dotnet publish src/Interpolation.App/Interpolation.App.csproj -f net9.0-android -c Release
```

**For Android App Bundle (Play Store):**
```bash
dotnet publish src/Interpolation.App/Interpolation.App.csproj -f net9.0-android -c Release -p:AndroidPackageFormat=aab
```

## 📊 Use Cases

### Engineering Applications
- Technical calculations and data analysis
- Performance characteristic analysis
- Scientific computations and modeling
- Data interpolation for research

### General Applications
- Data analysis and visualization
- Table lookups and interpolations
- Scientific calculations
- Educational demonstrations

### Example Applications
- **Engineering Design**: Interpolating design parameters between known points
- **Performance Analysis**: Finding performance metrics at specific operating conditions
- **Data Processing**: Calculating intermediate values from experimental data
- **Charts & Tables**: Reading values from printed tables and reference charts

## 🧪 Testing

Run the test suite:
```bash
dotnet test
```

Tests cover:
- ✅ Single interpolation accuracy
- ✅ Bilinear interpolation correctness
- ✅ Edge cases and boundary conditions
- ✅ Input validation scenarios

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Open Source Engineering Tools**
- Professional engineering solutions
- Mobile applications for technical professionals

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 Changelog

### Version 1.0.0 (Initial Release)
- ✅ Single linear interpolation
- ✅ Bilinear interpolation
- ✅ Cross-platform MAUI implementation
- ✅ Professional UI design
- ✅ Input validation and error handling
- ✅ Android Play Store ready

## 🔮 Roadmap

- [ ] Cubic spline interpolation
- [ ] Data import/export capabilities
- [ ] Graphical visualization of interpolation
- [ ] Advanced mathematical functions
- [ ] Cloud sync and data storage

---

**Built for engineers, by engineers.** �⚙️

For support or questions, please open an issue in this repository.