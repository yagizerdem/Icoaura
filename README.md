# Icoaura

Icoaura is an advanced icon pack manager and processor for Windows.  
It allows you to create, edit, and export icon packs with features like opacity, corner radius adjustment, dynamic theming, and a polished modern interface.

![Icoaura Logo](./Logo.png)

---

## Overview

Icoaura focuses on providing a simple and efficient way to build consistent icon themes for applications and directories.  
It combines strong image manipulation features with an extensible architecture and a refined UI.

**Core capabilities include:**

- Icon pack generation for `.ico`, `.png`, `.lnk`, and folder icons
- Batch image processing with opacity and corner radius control
- Adjustable theme colors with support for light and dark modes
- Real-time preview and dynamic updates
- Export-ready packs with metadata and version control
- Localized interface (English / Turkish)

---

## Technology Stack

| Component        | Technology                                      |
| ---------------- | ----------------------------------------------- |
| Frontend         | React, TypeScript, TailwindCSS, Framer Motion   |
| Desktop Host     | .NET 9 WPF + WebView2                           |
| Build System     | Vite                                            |
| Image Processing | ImageMagick (via external `.exe` process spawn) |
| Theming          | JSON-based configuration                        |
| Packaging        | ZIP builds                                      |

---
