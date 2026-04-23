# Project Roadmap: Virtual Assistant "Trojanski konj" (Educational Simulation)

## 1) Project Goal

Build a desktop virtual assistant that is useful for daily tasks (notes, calendar, gallery, fun content), while also teaching users how "Trojan-like" behavior can hide inside friendly software.

The security part must remain an **ethical simulation**: no real damage, no credential theft, no destructive actions.

## 2) Scope by Priority

### Core (MVP)

- Notes module (create, edit, save, timestamp)
- Calendar + reminders (create events, recurring reminders, notifications)
- Main assistant UI (avatar, command buttons, speech bubble)
- Basic command system (e.g., "add note", "add reminder", "tell joke")

### Extended

- Image gallery (browse folders, preview, simple collage flow)
- Jokes/facts module with random delivery
- Better UX (light/dark theme, improved navigation, onboarding hints)

### Advanced (Educational Security Layer)

- Safe "threat simulation" scenarios (system info awareness, behavior warnings)
- End-of-session analysis popup (what data *could* be exposed in real malware)
- Educational explanations (Trojan, ransomware concept, warning signs, prevention tips)

## 3) Suggested Timeline (10 Weeks)

### Phase 0 - Planning and Safety Rules (Week 1)

- Confirm stack (recommended: C# + WPF + MVVM)
- Define architecture and module boundaries
- Write security guardrails (what is explicitly forbidden)
- Create wireframes for assistant, notes, calendar, gallery

**Deliverable:** approved technical plan + UI mockups + safety policy.

### Phase 1 - Foundation (Weeks 2-3)

- Set up project structure, navigation shell, shared models/services
- Implement local persistence (SQLite or JSON)
- Build assistant core UI (avatar, speech bubble, action buttons)

**Deliverable:** running app shell with persistent storage and base UI.

### Phase 2 - Core Features (Weeks 4-5)

- Implement Notes end-to-end
- Implement Calendar + reminders (one-time + recurring)
- Connect command system to notes/calendar actions

**Deliverable:** stable MVP with core productivity flow.

### Phase 3 - Experience Features (Weeks 6-7)

- Implement Gallery module (folder load, preview, navigation)
- Implement Jokes/Facts module (randomized content)
- Improve UX polish, error handling, empty states

**Deliverable:** feature-complete assistant for daily-use demo.

### Phase 4 - Security Education Simulation (Weeks 8-9)

- Implement non-destructive simulation scenarios
- Add user-facing warnings and explanation panels
- Build final analysis report popup summarizing simulated findings

**Deliverable:** educational cyber-awareness experience integrated into app.

### Phase 5 - Final Testing and Presentation (Week 10)

- Functional and UX testing across all modules
- Security compliance check against guardrails
- Prepare demo script and project documentation

**Deliverable:** release candidate + presentation-ready demo.

## 4) Team Responsibilities

- **Frontend/UI:** WPF views, interactions, assistant visuals, navigation, themes
- **Backend/Logic:** command handling, data management, reminder timing, module integration
- **Security Researcher:** safe simulation design, ethics checks, educational content
- **UX/UI Designer:** visual language, flow consistency, usability improvements

## 5) Milestones and Quality Gates

- **M1 (end Week 3):** architecture + base app shell approved
- **M2 (end Week 5):** MVP (notes + calendar + reminders) demoable
- **M3 (end Week 7):** extended features integrated, UX pass completed
- **M4 (end Week 9):** educational simulation complete and validated as non-destructive
- **M5 (Week 10):** final QA passed, documentation complete, final demo ready

## 6) Risk Management

- **Scope creep:** keep MVP fixed; move extras to backlog
- **Safety risk:** enforce strict simulation boundaries and review checklist
- **Integration risk:** weekly integration checkpoints across modules
- **UX complexity:** run quick usability tests each phase

## 7) Definition of Done

- All MVP features work reliably with saved data
- Extended modules are functional and connected to the assistant
- Security component is educational, transparent, and harmless
- Documentation includes architecture, usage, and safety disclaimer
- Team can run a full end-to-end demo without manual fixes

