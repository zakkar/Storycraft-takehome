# Unity 6.2 UI/UX Improvement Spike

**Take-Home – Fourat**

---

## 1. What I Chose to Improve

After reviewing the provided Figma, two areas stood out as strong opportunities for UX enhancement:

### Photo Mode → Share Flow
An important moment where players capture and share something they created. The existing flow lacked hierarchy, and offered no feedback or celebration.

### Inventory (Concept Redesign)
The current inventory shows a very large grid of tiles, which can feel overwhelming on mobile. I explored a more compact, category-based structure with paging.

Most of the implementation work focuses on **Photo Mode**, while the Inventory update is presented as a structured, mobile-friendly concept.

---

## 2. Outcome

### Photo Mode – Improvements
- Clean screenshot capture (UI hidden for one frame)
- Framed photo card with visual hierarchy
- Clear share modal with caption input
- Mock share sheet (WhatsApp / Messenger / Copy / Save)
- Reward overlay after sharing
- Toast notifications for quick feedback

**Result:** A smoother, more celebratory flow that matches modern mobile UX patterns.

### Inventory – Concept
- Category tabs
- Paged grid (e.g., 4×2 slots)
- Bottom-sheet layout for mobile
- Data-driven structure ready for extensions like drag & drop or item details

---

## 3. Technical Decisions

- Used **UI Toolkit** (UXML + USS) for clean separation of layout, styling, and logic.
- MVC pattern for Photoshare mode : User Action → UI Event → Controller Handler → Game Logic → Update Views
- Modal workflow implemented for share modal → share sheet → reward overlay.
- Cached element references (query once)
- Coroutines stop cleanly on disable

---

## 4. How to Run

- **Unity Version:** 6.2
- **Scene:** `Scenes/PictureShareDemo.unity`
    - Press the button in the middle of the screen to trigger the Photo Mode flow.
- **Scene:** `Scenes/InventoryDemo.unity`
     - The size of the inventory can be modified 

---

## 5. AI Acceleration

AI was used selectively to scaffold the inventory boilerplate (data structure and UI layout). Architecture, interaction flow, naming, and UX decisions were implemented and refined manually.

---

## 6. What I'd Do With More Time

### Photo Mode
Platform sharing, richer animations, camera options, sticker/overlay editing.

### Inventory
Drag-and-drop interactions, item details panel, animated transitions, filtering and sorting.

---

## 7. Time Spent

Because I implemented two UX improvements instead of one, I exceeded the recommended 8-hour timebox.

| Task | Time |
|------|------|
| Figma review & planning | 20m |
| Photo Mode UI (UXML/USS) | 3h 40m |
| Screenshot capture flow | 3h 25m|
| polish | 40m |
| Inventory concept (tabs + paging) | 1h 15m |
| Debug tools & testing | 55m |
| README + Loom prep | 25m |
| **Total** | **~10h 40m** |
