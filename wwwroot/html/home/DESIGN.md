---
name: Tudor Noir
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#393939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1c1b1b'
  surface-container: '#20201f'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353535'
  on-surface: '#e5e2e1'
  on-surface-variant: '#c0c9c1'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#313030'
  outline: '#8a938c'
  outline-variant: '#404943'
  surface-tint: '#9cd2b5'
  primary: '#9cd2b5'
  on-primary: '#003825'
  primary-container: '#06402b'
  on-primary-container: '#77ac90'
  inverse-primary: '#356850'
  secondary: '#e9c349'
  on-secondary: '#3c2f00'
  secondary-container: '#af8d11'
  on-secondary-container: '#342800'
  tertiary: '#ffb3ac'
  on-tertiary: '#5d1714'
  tertiary-container: '#671e1b'
  on-tertiary-container: '#eb847b'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#b8efd0'
  primary-fixed-dim: '#9cd2b5'
  on-primary-fixed: '#002114'
  on-primary-fixed-variant: '#1b503a'
  secondary-fixed: '#ffe088'
  secondary-fixed-dim: '#e9c349'
  on-secondary-fixed: '#241a00'
  on-secondary-fixed-variant: '#574500'
  tertiary-fixed: '#ffdad6'
  tertiary-fixed-dim: '#ffb3ac'
  on-tertiary-fixed: '#400204'
  on-tertiary-fixed-variant: '#7b2d28'
  background: '#131313'
  on-background: '#e5e2e1'
  surface-variant: '#353535'
typography:
  headline-xl:
    fontFamily: Noto Serif
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Noto Serif
    fontSize: 32px
    fontWeight: '600'
    lineHeight: '1.3'
  headline-md:
    fontFamily: Noto Serif
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.4'
  body-lg:
    fontFamily: Manrope
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Manrope
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.6'
  label-sm:
    fontFamily: Manrope
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1.2'
    letterSpacing: 0.1em
rounded:
  sm: 0.125rem
  DEFAULT: 0.25rem
  md: 0.375rem
  lg: 0.5rem
  xl: 0.75rem
  full: 9999px
spacing:
  base: 8px
  container-padding: 32px
  gutter: 24px
  element-gap: 16px
---

## Brand & Style

The design system is rooted in the "Modern Noir" aesthetic, blending the atmospheric tension of a mid-century mystery with the tactile luxury of a British manor. It targets an audience seeking an immersive, high-stakes intellectual challenge. The emotional response is one of suspense, exclusivity, and gravity.

The visual style is **Tactile / Skeuomorphic** with a modern restraint. It leverages the depth of physical materials—polished mahogany, heavy velvet, and brushed brass—to ground the digital experience in a physical space. Every interaction should feel like handling a piece of evidence or moving a heavy piece on a mahogany board.

## Colors

The palette is anchored in deep, "Library" tones that evoke a sense of history and shadow.

- **Primary (Emerald):** A deep, saturated British Racing Green used for primary surfaces and high-value interactive states.
- **Secondary (Metallic Gold):** Used sparingly for borders, highlights, and critical CTA accents to denote "Premium" or "Clue" status.
- **Tertiary (Burgundy):** Reserved for "Danger," "Accusation," or "Murderer" states, providing a high-contrast warmth against the greens.
- **Neutrals:** A range of charcoal blacks and dark chocolate browns provide the foundation, mimicking the shadows and woodwork of the mansion.

## Typography

The typographic system balances "The Novel" with "The Ledger." 

- **Headings:** Use **Noto Serif** to evoke the literary nature of a classic mystery. Large headlines should feel authoritative and slightly intimidating.
- **UI & Data:** Use **Manrope** for all functional elements. Its refined, geometric clarity ensures that complex game data—clues, suspect lists, and timestamps—remains legible under low-light color conditions.
- **Labels:** Small labels use Manrope with increased letter spacing and uppercase styling to mimic typed evidence or classified dossiers.

## Layout & Spacing

This design system utilizes a **Fixed Grid** model to maintain the feel of a physical board game laid out on a table.

- **Grid:** A 12-column centered grid with wide 24px gutters.
- **Rhythm:** Spacing follows an 8px base unit. Larger gaps (32px+) are encouraged between disparate UI sections (e.g., the map vs. the notepad) to simulate physical distance between items on a desk.
- **Composition:** Asymmetric layouts are preferred for "Evidence" screens to create a noir-style "cluttered desk" aesthetic, while game controls remain strictly aligned and symmetrical.

## Elevation & Depth

Hierarchy is established through **Tonal Layers** and **Physical Texturing**.

- **Surfaces:** The lowest layer is the "Wood" background. Cards and panels sit on top of this with deep, soft-edge shadows (`offset-y: 8px, blur: 16px, opacity: 0.5`) to suggest they are physically lifted off the table.
- **Materials:** Use subtle CSS gradients to simulate light hitting velvet (Primary) or polished wood (Surface). 
- **Metallic Borders:** Elements of high importance feature a 1px solid Gold border with a linear-gradient to simulate a brass inlay.

## Shapes

The shape language is conservative and structural. 

- **Corners:** A "Soft" radius (0.25rem) is used for almost all components. This avoids the friendliness of rounded pills while softening the harshness of perfect squares, mimicking hand-cut cardstock or planed wood.
- **Frames:** Use "Picture Frame" styling for character portraits, utilizing double-borders (a dark inner stroke and a gold outer stroke).

## Components

### Buttons
- **Primary:** Deep Emerald background with a subtle inner-glow at the top edge. Text is Gold. On hover, the gold border intensifies.
- **Secondary:** Transparent with a thin Brass/Gold border. Text is White.

### Cards & Evidence
- **Suspect Cards:** Use a vertical aspect ratio. The image area should have a slight "film grain" overlay. The footer of the card uses the Surface Wood color.
- **The Ledger (Notepad):** A specialized component mimicking a grid-paper texture. It uses the `body-md` font with checkbox inputs that look like handwritten "X" marks.

### Input Fields
- Inputs should appear as recessed slots in the UI. Use a dark background with an inset shadow (`box-shadow: inset 0 2px 4px rgba(0,0,0,0.5)`).

### Chips & Tags
- Used for "Status" (e.g., *Confirmed*, *Suspected*). These should look like wax seals or embossed dymo-tape labels.

### Mystery Elements
- **The Dossier:** A modal component that opens with an animation mimicking a folder being unfolded. It uses high-contrast typography and "Redacted" bars for locked information.